using System.Linq;
using Microsoft.EntityFrameworkCore;
using mumarket.DataContracts.Request;
using mumarket.DataContracts.Responses;

namespace mumarket.Models.Commands
{
    public class PositiveKarmaCommand : BaseCommand
    {
        public CommandRequest Parameters { get; set; }
        private readonly string _positiveCmdMask = "/$";

        public PositiveKarmaCommand(CommandRequest request, MuMarketDbContext db) : base(db)
        {
            Parameters = request;

            if (Parameters.Msg.Contains(_positiveCmdMask, StringComparison.InvariantCultureIgnoreCase))
            {
                Type = CommandType.PositiveKarma;
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

            if (Type is CommandType.PositiveKarma)
            {
                var receiver = GetCleanReceiver();
                bool isValidReceiver = await IsValidReceiver(receiver);
                bool isValidSender = await IsValidSender(Parameters.Author, receiver);
                bool isFlood = await IsFlood(Parameters.Author);

                if (isValidReceiver && isValidSender && !isFlood)
                {
                    await _db.Karmas.AddAsync(new Karma
                    {
                        Receiver = receiver,
                        Sender = Parameters.Author,
                        ReceivedAt = DateTime.UtcNow,
                        Metadata = _positiveCmdMask
                    });
                    await _db.SaveChangesAsync();
                    response.IsSuccessful = true;
                    response.Message = $"Karma Increased! your current karma is: {await _db.Karmas.Where(x => x.Receiver == receiver
                    && x.Metadata == _positiveCmdMask).CountAsync()}";
                }

                if (isFlood)
                {
                    response.Message = "Stop flooding pally or an admin will warn you, pls wait to give karma again";
                }

                return response;
            }

            return await base.Execute();
        }

        private async Task<bool> IsFlood(string author)
        {
            var floodOffset = DateTime.UtcNow - TimeSpan.FromMinutes(1);
            var lastMinKarmas = await (from x in _db.Karmas.Where(x => x.ReceivedAt >= floodOffset)
                                       select x).ToListAsync(CancellationToken.None);

            return lastMinKarmas.Any(x => author.Contains(x.Sender));
        }

        private async Task<bool> IsValidSender(string author, string receiver)
        {
            // TODO performance is horrible, but easy for early delivery, replace this by cache
            return await _db.Users.AnyAsync(x => author.Contains(x.Id) && !x.Id.Contains(receiver), CancellationToken.None);
        }

        private async Task<bool> IsValidReceiver(string receiver)
        {
            return await _db.Users.AnyAsync(x => x.Alias == receiver, CancellationToken.None);
        }

        private string GetCleanReceiver()
        {
            return Parameters.Msg
                .Replace("@", string.Empty)
                .Replace(_positiveCmdMask, string.Empty).Trim();
        }
    }
}
