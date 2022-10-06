namespace Domain.Models
{
    public class EventFilters
    {
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int MaxEventCount { get; set; }
    }
}