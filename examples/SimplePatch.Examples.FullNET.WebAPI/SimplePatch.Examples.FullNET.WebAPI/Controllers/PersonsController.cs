using SimplePatch.Examples.FullNET.WebAPI.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace SimplePatch.Examples.FullNET.WebAPI.Controllers
{
    [RoutePrefix("persons")]
    public class PersonsController : ApiController
    {
        private readonly AppDbContext dbContext = new AppDbContext();

        // GET api/persons
        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            return await dbContext.People.ToListAsync();
        }

        // GET api/persons/5
        [HttpGet(), Route("{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var person = await dbContext.People.FindAsync(id);

            if (person == null) return NotFound();

            return Ok(person);
        }

        // POST api/persons
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Person value)
        {
            dbContext.People.Add(value);
            await dbContext.SaveChangesAsync();
            return Ok(value);
        }

        // PATCH api/persons/5
        [HttpPatch, Route("{id}")]
        public async Task<IHttpActionResult> Patch(int id, [FromBody]Delta<Person> person)
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

        // DELETE api/persons/5
        [HttpDelete(), Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var personToDelete = await dbContext.People.FindAsync(id);
            if (personToDelete == null) return BadRequest("Person not found");
            dbContext.People.Remove(personToDelete);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
