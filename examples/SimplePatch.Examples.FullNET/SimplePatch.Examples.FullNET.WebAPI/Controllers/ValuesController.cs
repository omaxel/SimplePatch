using SimplePatch.Examples.FullNET.DAL;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace SimplePatch.Examples.FullNET.WebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        AppDbContext appDbContext;

        public ValuesController()
        {
            appDbContext = new AppDbContext();
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            return await appDbContext.People.ToListAsync();
        }

        // GET api/values/5
        [HttpGet]
        public async Task<Person> Get(int id)
        {
            return await appDbContext.People.FindAsync(id);
        }

        // POST api/values
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Person value)
        {
            appDbContext.People.Add(value);
            await appDbContext.SaveChangesAsync();
            return Ok(value);
        }

        // PATCH api/values/5
        [HttpPatch]
        public async Task<IHttpActionResult> PatchAsync(int id, [FromBody]Delta<Person> person)
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
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var personToDelete = await appDbContext.People.FindAsync(id);
            if (personToDelete == null) return BadRequest("Person not found");
            appDbContext.People.Remove(personToDelete);
            await appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
