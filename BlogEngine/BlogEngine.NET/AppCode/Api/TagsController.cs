using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class TagsController : ApiController
{
    readonly ITagRepository repository;

    public TagsController(ITagRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<TagItem> Get(int take = 10, int skip = 0, string postId = "", string order = "")
    {
        return repository.Find(take, skip, postId, order);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]TagToUpdate tag)
    {
        repository.Save(tag.OldTag, tag.NewTag);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete(string id)
    {
        repository.Delete(id);
        return Request.CreateResponse(HttpStatusCode.OK);
    }
	
	[HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<TagItem> items)
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
                    repository.Delete(item.TagName);
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}

public class TagToUpdate
{
    public string OldTag { get; set; }
    public string NewTag { get; set; }
}
