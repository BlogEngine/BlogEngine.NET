using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class PagesController : ApiController
{
    readonly IPageRepository repository;

    public PagesController(IPageRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<PageItem> Get(int take = 10, int skip = 0, string filter = "", string order = "DateCreated descending")
    {
        return repository.Find(take, skip, filter, order);
    }

    public HttpResponseMessage Get(string id)
    {
        var result = repository.FindById(new Guid(id));
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    public HttpResponseMessage Post([FromBody]PageDetail item)
    {
        var result = repository.Add(item);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotModified);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]PageDetail item)
    {
        repository.Update(item, "update");
        return Request.CreateResponse(HttpStatusCode.OK);
    }
	
	[HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<PageDetail> items)
    {
        if (items == null || items.Count == 0)
            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

        var action = Request.GetRouteData().Values["id"].ToString().ToLowerInvariant();

        foreach (var item in items)
        {
            if (item.IsChecked)
            {
                if (action == "delete")
                {
                    repository.Remove(item.Id);
                }
                else
                {
                    repository.Update(item, action);
                }	
            }
        }

        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete(string id)
    {
        Guid gId;
        if (Guid.TryParse(id, out gId))
            repository.Remove(gId);
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}
