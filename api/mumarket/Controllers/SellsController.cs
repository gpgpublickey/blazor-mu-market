using System.Numerics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mumarket.DataContracts.Responses;

namespace mumarket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SellsController : Controller
    {
        private readonly MuMarketDbContext _db;
        private readonly IMapper _mapper;

        public SellsController(MuMarketDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<SellResponse>> GetLast50Sells()
        {
            var sells = await _db.Sells.Where(x => x.Author != null && x.IsSell).OrderByDescending(x => x.CreatedAt).Take(50).ToListAsync();
            var response = _mapper.Map<IEnumerable<SellResponse>>(sells);
            foreach (var sell in response)
            {
                var phone = GetSellAuthorPhoneNumber(sell);
                TryAssignPhone(sell, phone);
                await TryAssignKarma(sell, phone);
            }

            return response.OrderByDescending(x => x.Karma);
        }

        private static void TryAssignPhone(SellResponse sell, string phone)
        {
            sell.PhoneLink = phone != null ? $"https://wa.me/{phone}" : null!;
        }

        private async Task TryAssignKarma(SellResponse sell, string sellAuthorPhone)
        {
            sell.Karma = await _db.Karmas.Where(x => x.Receiver == sellAuthorPhone).CountAsync(CancellationToken.None);
        }

        private string GetSellAuthorPhoneNumber(SellResponse sell)
        {
            var numberParts = sell.Author!.Split("+", StringSplitOptions.RemoveEmptyEntries);
            if (numberParts.Count() > 1)
            {
                return GetSanitizedPhoneNumber(numberParts);
            }

            return null!;
        }

        private static string GetSanitizedPhoneNumber(string[] numberParts)
        {
            return numberParts[1]
                .Replace(":", string.Empty)
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .Trim();
        }
    }
}
