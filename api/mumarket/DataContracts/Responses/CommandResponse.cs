using mumarket.Models;

namespace mumarket.DataContracts.Responses
{
    public class CommandResponse
    {
        public bool IsSuccessful { get; set; }

        public CommandType CommandType { get; set; }

        public string Message { get; set; }
    }
}
