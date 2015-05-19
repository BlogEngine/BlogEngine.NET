using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class CommentFilterController : ApiController
{
    readonly ICommentFilterRepository repository;

    public CommentFilterController(ICommentFilterRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<CommentFilterItem> Get(int take = 10, int skip = 0, string filter = "", string order = "Filter")
    {
        return repository.Find(take, skip, filter, order);
    }

    public HttpResponseMessage Post([FromBody]CommentFilterItem item)
    {
        var result = repository.Add(item);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    [HttpPut]
    public HttpResponseMessage DeleteAll()
    {
        repository.RemoveAll();
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
