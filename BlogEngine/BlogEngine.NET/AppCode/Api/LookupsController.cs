using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Net;
using System.Web.Http;

public class LookupsController : ApiController
{
    readonly ILookupsRepository  repository;

    public LookupsController(ILookupsRepository repository)
    {
        this.repository = repository;
    }

    public Lookups Get()
    {
        return repository.GetLookups();
    }

    [HttpPut]
    public bool Update([FromBody]EditorOptions item)
    {
        try
        {
            repository.SaveEditorOptions(item);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            BlogEngine.Core.Utils.Log("Error updating", ex);
            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }
}
