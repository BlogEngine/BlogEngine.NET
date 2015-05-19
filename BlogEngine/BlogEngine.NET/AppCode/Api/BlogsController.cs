using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class BlogsController : ApiController
{
    readonly IBlogRepository repository;

    public BlogsController(IBlogRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<BlogEngine.Core.Data.Models.Blog> Get(int take = 10, int skip = 0, string filter = "", string order = "Name")
    {
        return repository.Find(take, skip, filter, order);
    }

    public HttpResponseMessage Get(string id)
    {
        var result = repository.FindById(Guid.Parse(id));
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    public HttpResponseMessage Post([FromBody]BlogItem item)
    {
        var result = repository.Add(item);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]BlogEngine.Core.Data.Models.Blog blog)
    {
        repository.Update(blog);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete(string id)
    {
        Guid gId;
        if (Guid.TryParse(id, out gId))
            repository.Remove(gId);
        return Request.CreateResponse(HttpStatusCode.OK);
    }
	
	[HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<BlogEngine.Core.Data.Models.Blog> items)
    {
        if (items == null || items.Count == 0)
            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

        var action = Request.GetRouteData().Values["id"].ToString();

        if (action.ToLower() == "delete")
        {
            foreach (var item in items)
            {
                if (item.IsChecked)
                {
					repository.Remove(item.Id);
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}