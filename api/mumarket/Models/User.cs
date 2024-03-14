namespace mumarket.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Alias { get; set; }

        public DateTime AddedAt { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsOwner { get; set; }
    }
}
