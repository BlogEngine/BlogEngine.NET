using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class CustomFieldsController : ApiController
{
    readonly ICustomFieldRepository repository;

    public CustomFieldsController(ICustomFieldRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<CustomField> Get(string filter = "")
    {
        // filter example: 'CustomType == "THEME" && ObjectId == "standard"'
        return repository.Find(filter);
    }

    public HttpResponseMessage Post([FromBody]CustomField item)
    {
        if (item == null) throw new ApplicationException("Custom field is required");

        item.BlogId = BlogEngine.Core.Blog.CurrentInstance.Id;

        if (item.CustomType == "PROFILE")
        {
            item.ObjectId = BlogEngine.Core.Security.CurrentUser.Identity.Name;
        }

        var result = repository.Add(item);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    public HttpResponseMessage Put([FromBody]List<CustomField> items)
    {
        if (items != null && items.Count > 0)
        {
            repository.ClearCustomFields(items[0].CustomType, items[0].ObjectId);

            foreach (var item in items)
            {
                repository.Add(item);
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete([FromBody]CustomField item)
    {
        repository.Remove(item.CustomType, item.ObjectId, item.Key);
        return Request.CreateResponse(HttpStatusCode.OK);
    }
}