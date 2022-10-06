using Application.Extensions;
using Domain.Models;

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

var closeEvents = new List<Event>();
// Add events from neighboring cities
var citiesInEvents = events.Select(e => e.City).Distinct().ToList();
var cityDistances = new Dictionary<string, int>();
foreach (var cityInEvent in citiesInEvents)
{
    cityDistances.Add(cityInEvent, TicketExtensions.GetDistance(customer.City, cityInEvent));
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

// Send emails to customer for all events they should get
foreach (var item in closeEvents)
{
    TicketExtensions.AddToEmail(customer, item);
}