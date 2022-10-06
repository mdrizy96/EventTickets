using Application.Extensions;
using Domain.Models;
using System.Collections.Immutable;

// Store event details for a city in a temporary location including the cost and distance
// Send emails to all customers in the city by filtering from the list
// Loop through all distinct customer cities to send events

var events = new List<Event>
{
    new Event {Name = "Phantom of the Opera", City = "New York"},
    new Event {Name = "Metallica", City = "Los Angeles"},
    new Event {Name = "Metallica", City = "New York"},
    new Event {Name = "Metallica", City = "Boston"},
    new Event {Name = "LadyGaGa", City = "New York"},
    new Event {Name = "LadyGaGa", City = "Boston"},
    new Event {Name = "LadyGaGa", City = "Chicago"},
    new Event {Name = "LadyGaGa", City = "San Francisco"},
    new Event {Name = "LadyGaGa", City = "Washington"}
};

var customers = new List<Customer>
{
    new Customer {Name = "Nathan", City = "New York"},
    new Customer {Name = "Bob", City = "Boston"},
    new Customer {Name = "Cindy", City = "Chicago"},
    new Customer {Name = "Lisa", City = "Los Angeles"},
    new Customer {Name = "Mr. Fake", City = "New York"}
};

var eventFilters = new EventFilters
{
    MinDistance = 0,
    MaxDistance = 350,
    MinPrice = 0,
    MaxPrice = 350,
    MaxEventCount = 2
};

var customerCities = customers.Select(c => c.City).Distinct().ToImmutableArray();

// For each customer city evaluate event details and store in temporary location
List<EventDetail> cityEventDetails;

foreach (var city in customerCities)
{
    // Evaluate customer event details
    EvaluateCityEventDetails(city);

    // Filter and send emails to customers in the city
    var customersInCity = customers.Where(c => c.City == city).ToList();
    var customerEvents = cityEventDetails.Where(c =>
            c.Distance >= eventFilters.MinDistance && c.Distance <= eventFilters.MaxDistance &&
            c.Price >= eventFilters.MinPrice && c.Price <= eventFilters.MaxPrice)
        .OrderBy(e => e.Distance)
        .ThenBy(e => e.Price)
        .Take(eventFilters.MaxEventCount)
        .ToList();
    foreach (var customer in customersInCity)
    {
        EmailEventToCustomer(customer, customerEvents);
    }
}

void EvaluateCityEventDetails(string city)
{
    cityEventDetails = events.Select(e => new EventDetail
    {
        Event = e,
        Distance = TicketExtensions.GetDistance(city, e.City),
        Price = TicketExtensions.GetPrice(e)
    }).ToList();
}

void EmailEventToCustomer(Customer customer, List<EventDetail> eventDetails)
{
    // Send emails to customer for all events they should get
    foreach (var eventDetail in eventDetails)
    {
        TicketExtensions.AddToEmail(customer, eventDetail.Event);
    }
}

Console.WriteLine("Done sending events");
Console.ReadKey();