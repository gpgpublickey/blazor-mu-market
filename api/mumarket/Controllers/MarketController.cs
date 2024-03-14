using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mumarket.DataContracts.Request;
using mumarket.DataContracts.Responses;
using mumarket.Models;
using mumarket.Models.Commands;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace mumarket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly ILogger<MarketController> _logger;
        private readonly MuMarketDbContext _db;
        private readonly OpenAIAPI _chatGpt;
        private readonly Conversation _chat;


        public MarketController(
            ILogger<MarketController> logger,
            MuMarketDbContext db)
        {
            _logger = logger;
            _db = db;
            _chatGpt = new OpenAI_API.OpenAIAPI("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");
            _chat = _chatGpt.Chat.CreateConversation();
            _chat.Model = Model.GPT4;
            _chat.RequestParameters.Temperature = 1;
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
            using (var trx = _db.Database.BeginTransaction())
            {
                try
                {
                    bool sellExists;

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
                        var sell = await _db.Sells.AddAsync(new Sell
                        {
                            Author = request.Author,
                            Post = request.Post,
                            CreatedAt = DateTime.UtcNow,
                            Img = request.Img
                        });
                        await _db.SaveChangesAsync();
                        SendToChatGpt(sell.Entity, sell.Entity.Id);
                    }

                    trx.Commit();
                }
                catch (ChatGptException)
                {
                    trx.Commit();
                }
                catch
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
                    _chat.AppendUserInput($"{req} y filtra sobre pedidos de venta unicamente incluidos a continuacion: {string.Join(";", pedidos)}");
                    return await _chat.GetResponseFromChatbotAsync();
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
                    _chat.AppendUserInput($"{req} y filtra sobre pedidos de compra unicamente incluidos a continuacion: {string.Join(";", request)}");
                    return await _chat.GetResponseFromChatbotAsync();
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
                    _chat.AppendUserInput($"Esta es parte de una lista de pedidos de venta, asignale la fecha y hora actual, el id es: {id} el contenido es: {request.Post}, la info de autor es: {request.Author}");
                }
                else if (request?.Post?.ToLowerInvariant().Contains("b>") ?? false)
                {
                    _chat.AppendUserInput($"Esta es parte de una lista de pedidos de compra, asignale la fecha y hora actual, el id es: {id} el contenido es: {request.Post}, la info de autor es: {request.Author}");
                }
            }
            catch
            {
                throw new ChatGptException("Chat gpt error");
            }
        }
    }
}