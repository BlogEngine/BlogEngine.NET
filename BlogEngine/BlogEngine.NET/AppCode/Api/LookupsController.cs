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
}
