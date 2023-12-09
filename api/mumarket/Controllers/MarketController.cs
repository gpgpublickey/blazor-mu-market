using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mumarket.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;

namespace mumarket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WhatsappController : ControllerBase
    {
        private readonly ILogger<WhatsappController> _logger;
        private readonly MuMarketDbContext _db;
        private readonly OpenAIAPI _chatGpt;
        private readonly Conversation _chat;


        public WhatsappController(
            ILogger<WhatsappController> logger,
            MuMarketDbContext db)
        {
            _logger = logger;
            _db = db;
            _chatGpt = new OpenAI_API.OpenAIAPI("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");
            _chat = _chatGpt.Chat.CreateConversation();
        }

        [HttpPost("sell")]
        public async Task PostSell(Sell request)
        {
            using (var trx = _db.Database.BeginTransaction())
            {
                try
                {
                    var sellExists = await _db.Sells.AnyAsync(x => x.Author == request.Author);
                    if (!sellExists)
                    {
                        var sell = await _db.Sells.AddAsync(request);
                        await _db.SaveChangesAsync();
                        SendToChatGpt(request, sell.Entity.Id);
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
                    var request = _db.Sells.Select(x => $"Pedido de compra id: {x.Id}, contenido: {x.Post}, autor: {x.Author}");
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