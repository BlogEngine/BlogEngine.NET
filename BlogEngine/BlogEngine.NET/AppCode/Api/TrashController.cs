using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class TrashController : ApiController
{
    readonly ITrashRepository repository;

    public TrashController(ITrashRepository repository)
    {
        this.repository = repository;
    }

    public TrashVM Get(int type = 1, int take = 10, int skip = 0, string filter = "1==1", string order = "DateCreated descending")
    {
        return repository.GetTrash((TrashType)type, take, skip, filter, order);
    }

    [HttpPut]
    public HttpResponseMessage Purge(TrashItem item)
    {
        repository.Purge(item.ObjectType, item.Id);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpPut]
    public HttpResponseMessage PurgeAll()
    {
        repository.PurgeAll();
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpPut]
    public HttpResponseMessage Restore(TrashItem item)
    {
        repository.Restore(item.ObjectType, item.Id);
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}
