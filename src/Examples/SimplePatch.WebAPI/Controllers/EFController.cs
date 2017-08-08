using SimplePatch.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace SimplePatch.WebAPI.Controllers
{
    public class EFController : ApiController
    {
        private AppDbContext db = new AppDbContext();

        [HttpGet]
        public async Task<IEnumerable<PersonEF>> GetAll()
        {
            return await db.People.ToListAsync();
        }

        [HttpPatch]
        public async Task<IHttpActionResult> PatchOne(int id, Delta<PersonEF> person)
        {
            // Determines the entity to be updated according to the id parameter
            var personToPatch = await db.People.FindAsync(id);
            if (personToPatch == null) return BadRequest("Person not found");

            // Apply the specified changes to the original entity. The Id property won't be changed. See why in Global.asax.   
            person.Patch(personToPatch);

            // Mark the entity as modified
            db.Entry(personToPatch).State = EntityState.Modified;

            // Now the personToPatch variable is updated

            // Save the changes
            await db.SaveChangesAsync();

            return Ok(personToPatch);
        }

        [HttpPatch]
        public async Task<IHttpActionResult> PatchMultiple(DeltaCollection<PersonEF> people)
        {
            foreach (var person in people)
            {
                // Try to get the value of the Id property
                if (person.TryGetPropertyValue(nameof(PersonEF.Id), out var id))
                {
                    // Determines the entity to be updated according to the id parameter
                    var personToPatch = await db.People.FindAsync(Convert.ToInt32(id));
                    if (personToPatch == null) return BadRequest("Person not found (Id = " + id + ")");
                  
                    // Apply the specified changes to the original entity
                    person.Patch(personToPatch);

                    // Mark the entity as modified
                    db.Entry(personToPatch).State = EntityState.Modified;
                }
                else
                {
                    // The Id property was not specified for the person represented by the person variable 
                    return BadRequest("Id property not found for a person");
                }
            }

            // Save the changes
            await db.SaveChangesAsync();

            return Ok(await db.People.ToListAsync());
        }
    }
}
