using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class UsersController : ApiController
{
    readonly IUsersRepository repository;

    public UsersController(IUsersRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<BlogUser> Get(int take = 10, int skip = 0, string filter = "1 == 1", string order = "UserName")
    {
        return repository.Find(take, skip, filter, order);
    }

    public HttpResponseMessage Get(string id)
    {
        var result = repository.FindById(id);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    public HttpResponseMessage Post([FromBody]BlogUser item)
    {
        var result = repository.Add(item);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotModified);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]BlogUser item)
    {
        repository.Update(item);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpPut]
    public HttpResponseMessage SaveProfile([FromBody]BlogUser item)
    {
        repository.SaveProfile(item);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<BlogUser> items)
    {
        if (items == null || items.Count == 0)
            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

        var action = Request.GetRouteData().Values["id"].ToString().ToLowerInvariant();

        if (action == "delete")
        {
            foreach (var item in items)
            {
                if (item.IsChecked)
                {
                    repository.Remove(item.UserName);
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete(string id)
    {
        repository.Remove(id);
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}