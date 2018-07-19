using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplePatch.Examples.Core2.WebAPI.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimplePatch.Examples.Core2.WebAPI.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly AppDbContext dbContext;

        public PersonsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET /persons
        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            return await dbContext.People.ToListAsync();
        }

        // GET /persons/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var person = await dbContext.People.FindAsync(id);
            if (person == null) return NotFound();
            return Ok(person);
        }

        // POST /persons
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Person value)
        {
            await dbContext.People.AddAsync(value);
            await dbContext.SaveChangesAsync();
            return Ok(value);
        }

        // PATCH /persons/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody]Delta<Person> person)
        {
            // Determines the entity to be updated according to the id parameter
            var personToPatch = await dbContext.People.FindAsync(id);
            if (personToPatch == null) return BadRequest("Person not found");

            if (person != null)
            {
                // Apply the specified changes to the original entity     
                person.Patch(personToPatch);

                // Now the personToPatch variable is updated

                // Mark the entity as modified
                dbContext.Entry(personToPatch).State = EntityState.Modified;

                // Save the changes
                await dbContext.SaveChangesAsync();
            }

            return Ok(personToPatch);
        }

        // DELETE /persons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var personToDelete = await dbContext.People.FindAsync(id);
            if (personToDelete == null) return BadRequest("Person not found");
            dbContext.People.Remove(personToDelete);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
