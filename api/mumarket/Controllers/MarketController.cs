using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mumarket.DataContracts.Request;
using mumarket.Models;
using mumarket.Models.Commands;
using OpenAI;
using OpenAI.Chat;

namespace mumarket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly ILogger<MarketController> _logger;
        private readonly MuMarketDbContext _db;
        private readonly OpenAIClient _chatGpt;
        private readonly ChatClient _chat;
        private readonly ChatMessage _conversationSetup;


        public MarketController(
            ILogger<MarketController> logger,
            MuMarketDbContext db,
            IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            var key = configuration.GetValue<string>("openai");
            _chatGpt = new OpenAIClient(key);
            _chat = _chatGpt.GetChatClient("gpt-5-mini");
            _conversationSetup = ChatMessage.CreateSystemMessage("You are Mu online Market Bot, a bot that helps users to manage their buy and sell orders in the MuMarket platform, your major labor is identify when a message is a sell, buy or just spam. You are friendly and helpful.");
        }

        [HttpPost("commands")]
        public async Task<string> PostCommand(CommandRequest request)
        {
            var cmdFactory = new CommandsChainFactory();
            var chain = cmdFactory.CreateGeneralChain(request, _db);
            var result = await chain.Execute();

            if (!result.IsSuccessful)
            {
                _logger.LogWarning($"{result.CommandType} command failed");
            }

            return result.Message;
        }

        [HttpPost("sell")]
        public async Task PostSell(AddSellRequest request)
        {
            _logger.LogInformation("Empezando a registrar venta");
            using (var trx = _db.Database.BeginTransaction())
            {
                try
                {
                    bool sellExists = false;
                    bool isSell = false;

                    if (request.Img != default)
                    {
                        sellExists = await _db.Sells.AnyAsync(x => x.Author == request.Author && x.Img == request.Img);
                    }
                    else
                    {
                        sellExists = await _db.Sells.AnyAsync(x => x.Author == request.Author);
                    }

                    if (!sellExists)
                    {
                        _logger.LogInformation("Revisando si es venta");

                        var question = $"Answer with 'yes' or 'no' if the next message is a buy or sell offer having considering additionally that B<, S<, B>, S>, B> , S> ; means BUY or SELL: '{request.Post}'";
                        var answer = await _chat.CompleteChatAsync([_conversationSetup, ChatMessage.CreateUserMessage(question)]);
                        isSell = answer.Value.Content[0].Text.Contains("yes", StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        _logger.LogInformation("La venta ya existia");
                    }

                    _logger.LogInformation("Revisando si es comando");

                    if (!sellExists && !request.Post!.StartsWith("/$"))
                    {
                        _logger.LogInformation("Insertando venta en db");

                        var sell = await _db.Sells.AddAsync(new Sell
                        {
                            Author = request.Author,
                            Post = request.Post,
                            CreatedAt = DateTime.UtcNow,
                            Img = request.Img,
                            IsSell = isSell
                        });
                        await _db.SaveChangesAsync();
                        //SendToChatGpt(sell.Entity, sell.Entity.Id);
                    }
                    trx.Commit();
                    _logger.LogInformation("La venta ya se guardo en la db");
                }
                catch (ChatGptException e)
                {
                    trx.Commit();
                }
                catch(Exception e)
                {
                    trx.Rollback();
                }
            }
        }

        [HttpGet("sell")]
        public async Task<string> GetSell(string req)
        {
            using (var trx = _db.Database.BeginTransaction())
            {
                try
                {
                    var pedidos = _db.Sells.Select(x => $"Pedido de venta id: {x.Id}, contenido: {x.Post}, autor: {x.Author}");
                    // _chat.AppendUserInput($"{req} y filtra sobre pedidos de venta unicamente incluidos a continuacion: {string.Join(";", pedidos)}");
                    //return await _chat.GetResponseFromChatbotAsync();
                    return string.Empty;
                }
                catch
                {
                    return "Error";
                }
            }
        }

        [HttpGet("buy")]
        public async Task<string> GetBuy(string req)
        {
            using (var trx = _db.Database.BeginTransaction())
            {
                try
                {
                    var request = _db.Sells.Where(x => x.CreatedAt.Date == DateTime.Today.Date).Select(x => $"Pedido de compra id: {x.Id}, contenido: {x.Post}, autor: {x.Author}");
                    //_chat.AppendUserInput($"{req} y filtra sobre pedidos de compra unicamente incluidos a continuacion: {string.Join(";", request)}");
                    //return await _chat.GetResponseFromChatbotAsync();
                    return string.Empty;
                }
                catch
                {
                    return "Error";
                }
            }
        }

        private void SendToChatGpt(Sell request, int id)
        {
            try
            {
                if (request?.Post?.ToLowerInvariant().Contains("s>") ?? false)
                {
                    //_chat.AppendUserInput($"Esta es parte de una lista de pedidos de venta, asignale la fecha y hora actual, el id es: {id} el contenido es: {request.Post}, la info de autor es: {request.Author}");
                }
                else if (request?.Post?.ToLowerInvariant().Contains("b>") ?? false)
                {
                   // _chat.AppendUserInput($"Esta es parte de una lista de pedidos de compra, asignale la fecha y hora actual, el id es: {id} el contenido es: {request.Post}, la info de autor es: {request.Author}");
                }
            }
            catch
            {
                throw new ChatGptException("Chat gpt error");
            }
        }
    }
}