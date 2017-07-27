<img src="http://raw.github.com/OmarMuscatello/SimplePatch/master/simplepatch.png" height="70" alt="SimplePatch">

Una semplice libreria per l'utilizzo del metodo PATCH in ASP.NET Web API

##### Sommario
- [Introduzione](#introduzione)
- [Installazione](#installazione)
- [Utilizzo](#utilizzo)
- [Integrazione con Entity Framework](#integrazione-con-entity-framework)
- [Configurazione](#configurazione)

## Introduzione

### Il problema
Uno dei problemi comuni nell'implementazione di un servizio RESTful in ASP.NET Web API è la modifica parziale delle entità. Il client, infatti, deve specificare il valore per tutte le proprietà dell'entità, comprese quelle proprietà il cui valore non è stato modificato. In genere, per risolvere questo problema si utilizzano queste soluzioni che portano con se le proprie problematiche:
- [`Delta<T>`](https://msdn.microsoft.com/en-us/library/jj890572(v=vs.118).aspx) (parte di Microsoft ASP.NET WebAPI OData): non supporta gli interi in JSON (vedi [questa risposta](https://stackoverflow.com/a/14734273/7772490)). Inoltre è necessario installare il pacchetto con tutte le sue dipendenze di dimensioni non banali;
- [JSON Patch](http://jsonpatch.com/): il client deve organizzare i dati per operazione e le dimensioni della richiesta non sono ottimizzate.

##### Esempio
Il client deve impostare la proprietà `Abilitato` dell'entità `Utente`. Quest'ultima, tuttavia, espone anche la proprietà `Nome`. Il client si trova costretto a passare nel corpo della richiesta sia il valore della proprietà `Abilitato` che il valore della proprietà `Nome`.

*Corpo della richiesta*
```   
{ "Abilitato": true, "Nome": "Utente1" }
```

In un caso reale, comunque, le proprietà di un'entità sono più di due rendendo il problema più accentuato.
```   
{ "Abilitato": true, "Nome": "Utente1", "Prop1": "Value1", "Prop2": "Value2", "Prop3": "Value3", ... }
```

### La soluzione
La soluzione ideale consiste nel consentire al client di effettuare una richiesta con le sole proprietà da modificare.
Ritornando all'esempio indicato nella sezione *[Il problema](#il-problema)*, il corpo della richiesta per la modifica del valore della proprietà `Abilitato` sarà:
```   
{ "Abilitato": true }
```
Nel caso in cui l'entità avesse più proprietà il corpo della richiesta rimarrà il medesimo.

*SimplePatch* consente di implementare esattamente questa soluzione in ASP.NET Web API.

## Installazione
Lancia il seguente comando da *Console di Gestione pacchetti*:
```
Install-Package SimplePatch
```

## Utilizzo
### Aggiornamento di una sola entità
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
### Aggiornamento di più entità  
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

## Integrazione con Entity Framework
### Aggiornamento di una sola entità
```
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
```

### Aggiornamento di più entità
```
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
```

## Configurazione
È possibile escludere una o più proprietà di un'entità durante l'applicazione delle modifiche all'entità originale così da preservare il valore originale della proprietà. Questo potrebbe essere utile per le proprietà utilizzate per identificare univocamente l'entità.

**Global.asax** o **Startup.cs**
```
DeltaConfig.Init((cfg) =>
{
    // Escludo la proprietà Id dell'entità Person.
    cfg.ExcludeProperties<Person>(x => x.Id);
});
```

**Nota:** Quando una proprietà viene contrassegnata come *esclusa* essa sarà ugualmente presente nell'oggetto `Delta<T>`, ma verrà ignorata in fase di applicazione delle modifiche (metodo `Patch`) all'entità originale.
