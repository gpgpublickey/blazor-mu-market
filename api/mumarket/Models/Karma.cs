namespace mumarket.Models
{
    public class Karma
    {
        public int Id { get; set; }

        public string Receiver { get; set; }

        public string Sender { get; set; }

        public string Metadata { get; set; }

        public DateTime ReceivedAt { get; set; }
    }
}
