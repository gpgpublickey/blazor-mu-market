using System.Buffers.Text;
using System.Text;
using mumarket.DataContracts.Request;
using mumarket.DataContracts.Responses;

namespace mumarket.Models.Commands
{
    public class DeobCommand : BaseCommand
    {
        public DeobCommand(CommandRequest request, MuMarketDbContext db) : base(db)
        {
            Parameters = request;

            if (Base64.IsValid(Parameters.Raw))
            {
                Type = CommandType.Decode;
            }
        }

        public override async Task<CommandResponse> Execute()
        {
            var response = new CommandResponse
            {
                IsSuccessful = false,
                CommandType = Type,
                Message = string.Empty
            };

            if (Type is CommandType.Decode)
            {
                Parameters.Raw = Encoding.UTF8.GetString(Convert.FromBase64String(Parameters.Raw));
                response.IsSuccessful = true;
            }

            return await base.Execute();
        }
    }
}
