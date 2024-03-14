using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace mumarket.Models
{
    public class Sell
    {
        public int Id { get; set; }

        public string? Post { get; set; }

        public string? Author { get; set; }

        public string? Img { get; set; }

        public DateTime CreatedAt { get; set; }

        public Realms Realm { get; set; }
    }
}