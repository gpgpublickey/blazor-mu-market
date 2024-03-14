using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using mumarket.Dtos;
using mumarket.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mumarket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(MuMarketDbContext db, IMapper mapper) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost("bulk")]
        public async Task Post([FromBody] UserDto[] request)
        {
            foreach (var user in request)
            {
                if (!db.Users.Any(x => x.Id == user.id._serialized))
                {
                    await db.Users.AddAsync(mapper.Map<User>(user));
                    await db.SaveChangesAsync();
                }
            }
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
