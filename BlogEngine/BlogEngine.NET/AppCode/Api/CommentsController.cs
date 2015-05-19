using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class CommentsController : ApiController
{
    readonly ICommentsRepository repository;

    public CommentsController(ICommentsRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<CommentItem> Get(int type = 1, int take = 0, int skip = 0, string filter = "IsDeleted == false", string order = "DateCreated descending")
    {
        return repository.GetComments((CommentType)type, take, skip, filter, order);
    }

    public HttpResponseMessage Get(string id)
    {
        var result = repository.FindById(new System.Guid(id));
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    public HttpResponseMessage Put([FromBody]CommentItem item)
    {
        repository.Update(item, "update");
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
    public HttpResponseMessage ProcessChecked([FromBody]List<CommentItem> items)
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

    [HttpPut]
    public HttpResponseMessage DeleteAll([FromBody]CommentItem item)
    {
        var action = Request.GetRouteData().Values["id"].ToString();

        if (action.ToLower() == "pending" || action.ToLower() == "spam")
        {
            repository.DeleteAll(action);
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}
