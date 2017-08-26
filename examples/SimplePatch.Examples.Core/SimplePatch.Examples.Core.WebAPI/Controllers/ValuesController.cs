using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplePatch.Examples.Core.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimplePatch.Examples.Core.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        AppDbContext appDbContext;

        public ValuesController(AppDbContext context)
        {
            appDbContext = context;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            return await appDbContext.People.ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<Person> Get(int id)
        {
            return await appDbContext.People.FindAsync(id);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Person value)
        {
            await appDbContext.People.AddAsync(value);
            await appDbContext.SaveChangesAsync();
            return Ok(value);
        }

        // PATCH api/values/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody]Delta<Person> person)
        {
            // Determines the entity to be updated according to the id parameter
            var personToPatch = await appDbContext.People.FindAsync(id);
            if (personToPatch == null) return BadRequest("Person not found");

            // Apply the specified changes to the original entity     
            person.Patch(personToPatch);

            // Mark the entity as modified
            appDbContext.Entry(personToPatch).State = EntityState.Modified;

            // Now the personToPatch variable is updated

            // Save the changes
            await appDbContext.SaveChangesAsync();

            return Ok(personToPatch);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var personToDelete = await appDbContext.People.FindAsync(id);
            if (personToDelete == null) return BadRequest("Person not found");
            appDbContext.People.Remove(personToDelete);
            await appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
