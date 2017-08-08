using SimplePatch.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SimplePatch.WebAPI.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public IEnumerable<Person> GetAll()
        {
            return TestData.People;
        }

        [HttpPatch]
        public IHttpActionResult PatchOne(int id, Delta<Person> person)
        {
            // Determines the entity to be updated according to the id parameter
            var personToPatch = TestData.People.FirstOrDefault(x => x.Id == id);

            var c = person.GetSpecifiedPropertyNames();

            if (personToPatch == null) return BadRequest("Person not found");
            
            // Apply the changes specified to the original entity
            person.Patch(personToPatch);

            // Now the personToPatch variable is updated

            return Ok(personToPatch);
        }

        [HttpPatch]
        public IHttpActionResult PatchMultiple(DeltaCollection<Person> people)
        {
            foreach (var person in people)
            {
                // Try to get the value of the Id property
                if (person.TryGetPropertyValue(nameof(Person.Id), out var id))
                {
                    // Determines the entity to be updated according to the specified id
                    var entityToPatch = TestData.People.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                    if (entityToPatch == null) return BadRequest("Person not found (Id = " + id + ")");

                    // Apply the specified changes to the original entity       
                    person.Patch(entityToPatch);
                }
                else
                {
                    // The Id property was not specified for the person represented by the person variable 
                    return BadRequest("Id property not found for a person");
                }
            }

            return Ok(TestData.People);
        }
    }
}
