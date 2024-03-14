using mumarket.DataContracts.Request;
using mumarket.DataContracts.Responses;

namespace mumarket.Models.Commands
{
    public abstract class BaseCommand(MuMarketDbContext db)
    {
        public CommandRequest Parameters { get; set; }

        public BaseCommand? Next { get; set; }

        public CommandType Type { get; set; }

        protected MuMarketDbContext _db = db ?? throw new ArgumentNullException();

        public virtual async Task<CommandResponse> Execute()
        {
            if (Next is not null)
            {
                Next.Parameters = Parameters;
                return await Next.Execute();
            }

            return new CommandResponse
            {
                IsSuccessful = false,
                Message = string.Empty
            };
        }

        public void SetNext(BaseCommand cmd)
        {
            Next = cmd;
        }
    }
}
