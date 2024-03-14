namespace mumarket.DataContracts.Request
{
    public class CommandRequest
    {
        public required string Raw { get; set; }

        public required string Msg { get; set; }

        public required string Author { get; set; }
    }
}
