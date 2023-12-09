using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace mumarket.Models
{
    public class Sell
    {
        public int Id { get; set; }

        public string? Post { get; set; }

        public string? Author { get; set; }
    }
}