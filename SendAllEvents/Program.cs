using System.Collections.Immutable;
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

//1. find out all events that are in city of customer and add to email
var customer = new Customer { Name = "Mr. Fake", City = "New York" };
var validEvents = events.Where(e => e.City.Contains(customer.City)).ToImmutableArray();

// Send emails to customer for all events they should get
foreach (var item in validEvents)
{
    TicketExtensions.AddToEmail(customer, item);
}