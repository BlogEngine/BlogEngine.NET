using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Net;
using System.Web.Http;

public class StatsController : ApiController
{
    readonly IStatsRepository repository;

    public StatsController(IStatsRepository repository)
    {
        this.repository = repository;
    }

    public Stats Get()
    {
        return repository.Get();
    }
}
