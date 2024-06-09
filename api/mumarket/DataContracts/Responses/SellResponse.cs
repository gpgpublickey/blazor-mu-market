namespace mumarket.DataContracts.Responses
{
    public class SellResponse
    {
        public string? Post { get; set; }

        public string? Author { get; set; }

        public string PhoneLink { get; set; }

        public int Karma { get; set; }

        public string? Img { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
