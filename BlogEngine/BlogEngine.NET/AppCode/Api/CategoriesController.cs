using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class CategoriesController : ApiController
{
    readonly ICategoryRepository repository;

    public CategoriesController(ICategoryRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<CategoryItem> Get(int take = 10, int skip = 0, string filter = "", string order = "")
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

    public HttpResponseMessage Post([FromBody]CategoryItem item)
    {
        var result = repository.Add(item);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]CategoryItem item)
    {
        repository.Update(item);
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
    public HttpResponseMessage ProcessChecked([FromBody]List<CategoryItem> items)
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
