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
            //Determino l'entità da aggiornare in base al parametro id
            var personToPatch = TestData.People.FirstOrDefault(x => x.Id == id);
            if (personToPatch == null) return BadRequest("Person not found");

            /*
             * Applico le modifiche specificate all'entità originale. Tuttavia, il parametro Id non viene mai aggiornato.
             * Vedi Global.asax
             */
            person.Patch(personToPatch);

            //Adesso la variabile personToPatch è aggiornata

            return Ok(personToPatch);
        }

        [HttpPatch]
        public IHttpActionResult PatchMultiple(DeltaCollection<Person> people)
        {
            foreach (var person in people)
            {
                //Tento di ottenere il valore della proprietà Id
                if (person.TryGetPropertyValue(nameof(Person.Id), out var id))
                {
                    //Determino l'entità da aggiornare in base all'id specificato
                    var entityToPatch = TestData.People.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                    if (entityToPatch == null) return BadRequest("Person not found (Id = " + id + ")");

                    /*
                     * Applico le modifiche specificate all'entità originale. Tuttavia, il parametro Id non viene mai aggiornato.
                     * Vedi Global.asax
                     */
                    person.Patch(entityToPatch);
                }
                else
                {
                    //La proprietà Id non è stata specificata per la persona rappresentata dalla variabile person
                    return BadRequest("Id property not found for a person");
                }
            }

            return Ok(TestData.People);
        }
    }
}
