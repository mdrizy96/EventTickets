using Application.Extensions;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

var events = new List<Event>{
    new Event{ Name = "Phantom of the Opera", City = "New York"},
    new Event{ Name = "Metallica", City = "Los Angeles"},
    new Event{ Name = "Metallica", City = "New York"},
    new Event{ Name = "Metallica", City = "Boston"},
    new Event{ Name = "LadyGaGa", City = "New York"},
    new Event{ Name = "LadyGaGa", City = "Boston"},
    new Event{ Name = "LadyGaGa", City = "Chicago"},
    new Event{ Name = "LadyGaGa", City = "San Francisco"},
    new Event{ Name = "LadyGaGa", City = "Washington"}
};

var customer = new Customer { Name = "Mr. Fake", City = "New York" };

IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

EmailEventsToCustomer();
EmailEventsToCustomer();
EmailEventsToCustomer();

void EmailEventsToCustomer()
{
    // Find out all events that are in city of customer and add to email
    var closeEvents = events.Where(e => e.City.Contains(customer.City)).Take(5).ToList(); // get 5 events in the customer's city
    if (closeEvents.Count < 5)
    {
        // Add events from neighboring cities
        var citiesInEvents = events.Where(e => !e.City.Contains(customer.City)).Select(e => e.City).Distinct().ToList();
        var cityDistances = new Dictionary<string, int>();
        foreach (var cityInEvent in citiesInEvents)
        {
            var distance = GetOrCreateCityDistanceInCache(cache, customer.City, cityInEvent);
            if (distance > 0) // Because the event is in a different city, distance is expected to be 0. 0 would in this instance signify there was an error calling the distance api
            {
                cityDistances.Add(cityInEvent, distance);
            }
        }

        cityDistances = cityDistances.OrderBy(c => c.Value).ToDictionary(c => c.Key, c => c.Value);
        foreach (var city in cityDistances.Keys)
        {
            var eventsInCloseCity = events.Where(e => e.City.Contains(city)).Take(5 - closeEvents.Count).ToList();
            closeEvents.AddRange(eventsInCloseCity);
            if (closeEvents.Count == 5)
            {
                break;
            }
        }
    }

    // Send emails to customer for all events they should get
    foreach (var item in closeEvents)
    {
        TicketExtensions.AddToEmail(customer, item);
    }
}
//3. Improvements to solution
// Cache city distances to reduce wait time if the get distance api is slow
// If the distance api call fails, Send events in customer city only
// If allowed and possible, send events using other factors like price, or events based on history of events attended - common cities outside home city

//4. If get distance fails
// Handle failures to calls to the get distance api
// Send only events in customer city, select random events outside home city to add up to 5

static string GetDistanceCacheKey(string fromCity, string toCity)
{
    return
        $"distance-{fromCity.Trim().ToLowerInvariant().Replace(" ", "")}-{toCity.Trim().ToLowerInvariant().Replace(" ", "")}";
}

static int GetOrCreateCityDistanceInCache(IMemoryCache cache, string fromCity, string toCity)
{
    if (cache == null) throw new ArgumentNullException(nameof(cache), "cache cannot be null");
    if (string.IsNullOrEmpty(fromCity)) throw new Exception("From city is required");
    if (string.IsNullOrEmpty(toCity)) throw new Exception("To city is required");

    var found = cache.TryGetValue(GetDistanceCacheKey(fromCity, toCity), out int distance);
    if (!found)
    {
        found = cache.TryGetValue(GetDistanceCacheKey(toCity, fromCity), out distance);
    }

    if (found) return distance;
    try
    {
        using (var entry = cache.CreateEntry(GetDistanceCacheKey(fromCity, toCity)))
        {
            distance = TicketExtensions.GetDistance(fromCity, toCity);
            entry.Value = distance;
            entry.AbsoluteExpiration = DateTime.UtcNow.AddYears(20);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.Message} {ex.InnerException?.Message}");
        distance = 0;
    }
    return distance;
}

Console.ReadKey();