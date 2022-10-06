using Domain.Models;

namespace Application.Extensions
{
    public static class TicketExtensions
    {
        // You do not need to know how these methods work
        public static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                                  + (distance > 0 ? $" ({distance} miles away)" : "")
                                  + (price.HasValue ? $" for ${price}" : ""));
        }

        public static int GetPrice(Event e)
        {
            return (AlphabeticalDistance(e.City, "") + AlphabeticalDistance(e.Name, "")) / 10;
        }

        public static int GetDistance(string fromCity, string toCity)
        {
            return AlphabeticalDistance(fromCity, toCity);
        }

        private static int AlphabeticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i
                   <
                   Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
    }
}