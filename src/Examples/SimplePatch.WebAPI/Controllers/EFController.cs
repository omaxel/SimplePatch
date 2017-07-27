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
    //Determino l'entità da aggiornare in base al parametro id
    var personToPatch = await db.People.FindAsync(id);
    if (personToPatch == null) return BadRequest("Person not found");

    /*
        * Applico le modifiche specificate all'entità originale. Tuttavia, il parametro Id non viene mai aggiornato.
        * Vedi Global.asax
        */
    person.Patch(personToPatch);
    db.Entry(personToPatch).State = EntityState.Modified;

    //Adesso la variabile personToPatch è aggiornata

    //Salvo le modifiche
    await db.SaveChangesAsync();

    return Ok(personToPatch);
}

        [HttpPatch]
        public async Task<IHttpActionResult> PatchMultiple(DeltaCollection<PersonEF> people)
        {
            foreach (var person in people)
            {
                //Tento di ottenere il valore della proprietà Id
                if (person.TryGetPropertyValue(nameof(PersonEF.Id), out var id))
                {
                    //Determino l'entità da aggiornare in base all'id specificato
                    var personToPatch = await db.People.FindAsync(Convert.ToInt32(id));
                    if (personToPatch == null) return BadRequest("Person not found (Id = " + id + ")");

                    /*
                        * Applico le modifiche specificate all'entità originale. Tuttavia, il parametro Id non viene mai aggiornato.
                        * Vedi Global.asax
                        */
                    person.Patch(personToPatch);

                    //Contrassegno l'entità come modificata
                    db.Entry(personToPatch).State = EntityState.Modified;
                }
                else
                {
                    //La proprietà Id non è stata specificata per la persona rappresentata dalla variabile person
                    return BadRequest("Id property not found for a person");
                }
            }

            //Salvo le modifiche
            await db.SaveChangesAsync();

            return Ok(await db.People.ToListAsync());
        }
    }
}
