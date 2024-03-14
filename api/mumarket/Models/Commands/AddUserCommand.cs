using System.Buffers.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using mumarket.DataContracts.Request;
using mumarket.DataContracts.Responses;

namespace mumarket.Models.Commands
{
    public class AddUserCommand : BaseCommand
    {
        private const string PhoneNumberRegexPattern = "(?!\\=\")[\\d]+(?=\\@)";
        private readonly string _cmdMask = "/adduser ";

        public string Sender { get; set; }

        public AddUserCommand(CommandRequest request, MuMarketDbContext db) : base(db)
        {
            Parameters = request;
            var regex = new Regex(PhoneNumberRegexPattern);
            var phoneNumber = regex.Match(Parameters.Raw).Value;

            if (phoneNumber != default && Parameters.Msg.Contains(_cmdMask))
            {
                Type = CommandType.AddUser;
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

            if (Type is CommandType.AddUser)
            {
                // TODO add user
                return response;
            }

            return await base.Execute();
        }
    }
}
