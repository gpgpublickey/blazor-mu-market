using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mumarket.DataContracts.Responses;

namespace mumarket
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
            var sells = await _db.Sells.Where(x => x.Author != null).OrderByDescending(x => x.CreatedAt).Take(50).ToListAsync();
            var response = _mapper.Map<IEnumerable<SellResponse>>(sells);
            foreach (var sell in response)
            {
                var numberParts = sell.Author!.Split("+", StringSplitOptions.RemoveEmptyEntries);

                if (numberParts.Count() > 1)
                {
                    var phone = numberParts[1]
                        .Replace(":", string.Empty)
                        .Replace("-", string.Empty)
                        .Replace(" ", string.Empty)
                        .Trim();
                    sell.PhoneLink = $"https://wa.me/{phone}";
                }
            }

            return response;
        }
    }
}
