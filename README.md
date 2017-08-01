<img src="http://raw.github.com/OmarMuscatello/SimplePatch/master/simplepatch.png" height="70" alt="SimplePatch">


A simple library for using the PATCH method in ASP.NET Web API.

**Help me!** Improve this translation with a pull request.

##### Summary
- [Introduction](#introduction)
- [Install](#install)
- [How to use](#how-to-use)
- [Integration with the Entity Framework](#integration-with-entity-framework)
- [Configuration](#configuration)

## Introduction

### The problem
Partial modification of entities is one of the common issues when implementing a RESTful service in ASP.NET Web API. The client, in fact, must specify the value for all entity properties, including those properties whose value has not been changed. Typically, to solve this problem, you use these solutions with their own problems:
- [`Delta<T>`](https://msdn.microsoft.com/en-us/library/jj890572(v=vs.118).aspx) (part of Microsoft ASP.NET WebAPI OData): it has some problems with numbers when using JSON (see [this answer](https://stackoverflow.com/a/14734273/7772490)). You also need to install the package with all its non-trivial dependencies;
- [JSON Patch](http://jsonpatch.com/): the client must organize the data per operation and the size of the request is not optimized.

##### Demonstrative example
The client must set the `Enabled` property of the `User` entity. The latter, however, also exposes the `Name` property. The client is forced to pass both the values of the `Enabled` and `Name` properties in the request body.

*Request body*
```   
{ "Enabled": true, "Name": "User1" }
```

In a real case, however, the properties of an entity are more than two, making the problem more pronounced.
```   
{ "Enabled": true, "Name": "User1", "Prop1": "Value1", "Prop2": "Value2", "Prop3": "Value3", ... }
```

### The solution
The ideal solution is to allow the client to make a request with the only properties to modify.
Returning to the example shown in the *[Problem](#the-problem)* section, the request body for changing the value of the `Enabled` property will be:
```   
{ "Enabled": true }
```
If the entity has more property, the request body will remain the same.

*SimplePatch* allows you to implement this solution in the ASP.NET Web API.

## Install
Launch the following command from *Package Manager Console*:
```
Install-Package SimplePatch
```

## How to use
##### Patching a single entity
    [HttpPatch]
    public IHttpActionResult PatchOne(int id, Delta<Person> person)
    {
        // Determines the entity to be updated according to the id parameter
        var personToPatch = TestData.People.FirstOrDefault(x => x.Id == id);
        if (personToPatch == null) return BadRequest("Person not found");

        // Apply the changes specified to the original entity
        person.Patch(personToPatch);

        // Now the personToPatch variable is updated

        return Ok(personToPatch);
    }
##### Patching multiple entities
    [HttpPatch]
    public IHttpActionResult PatchMultiple(DeltaCollection<Person> people)
    {
        foreach (var person in people)
        {
            // Try to get the value of the Id property
            if (person.TryGetPropertyValue(nameof(Person.Id), out var id))
            {
                // Determines the entity to be updated according to the specified id
                var personToPatch = TestData.People.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                if (personToPatch == null) return BadRequest("Person not found (Id = " + id + ")");

                // Apply the specified changes to the original entity       
                person.Patch(personToPatch);
            }
            else
            {
                // The Id property was not specified for the person represented by the person variable 
                return BadRequest("Id property not found for a person");
            }
        }

        return Ok();
    }

## Integration with Entity Framework
##### Patching a single entity
```
[HttpPatch]
public async Task<IHttpActionResult> PatchOne(int id, Delta<PersonEF> person)
{
    // Determines the entity to be updated according to the id parameter
    var personToPatch = await db.People.FindAsync(id);
    if (personToPatch == null) return BadRequest("Person not found");

    // Apply the specified changes to the original entity     
    person.Patch(personToPatch);

    // Mark the entity as modified
    db.Entry(personToPatch).State = EntityState.Modified;

    // Now the personToPatch variable is updated

    // Save the changes
    await db.SaveChangesAsync();

    return Ok(personToPatch);
}
```

##### Patching multiple entities
```
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

    return Ok();
}
```

## Configuration
You can exclude one or more properties of an entity while applying the changes to the original entity to preserve the original value of the property. This might be useful for properties used to uniquely identify the entity.

**Global.asax** or **Startup.cs**
```
DeltaConfig.Init((cfg) =>
{
    // Exclude the Id property of the Person entity.
    cfg.ExcludeProperties<Person>(x => x.Id);
});
```

**Note:** When a property is marked as *excluded* it will still be present in the `Delta <T>` object, but it will be ignored when the changes are applied (`Patch` method) to the original entity.
