using BlogEngine.Core;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class RolesController : ApiController
{
    readonly IRolesRepository repository;

    public RolesController(IRolesRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<RoleItem> Get(int take = 10, int skip = 0, string filter = "", string order = "")
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

    public HttpResponseMessage Post([FromBody]RoleItem role)
    {
        var result = repository.Add(role);
        if (result == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        return Request.CreateResponse(HttpStatusCode.Created, result);
    }

    public HttpResponseMessage Put([FromBody]List<RoleItem> items)
    {
        if (items == null || items.Count == 0)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        foreach (var item in items)
        {
            repository.Remove(item.RoleName);
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete(string id)
    {
        repository.Remove(id);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<RoleItem> items)
    {
        if (items == null || items.Count == 0)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        var action = Request.GetRouteData().Values["id"].ToString();

        if (action.ToLower() == "delete")
        {
            foreach (var item in items)
            {
                if (item.IsChecked)
                {
                    repository.Remove(item.RoleName);
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpGet]
    public HttpResponseMessage GetRights(string id)
    {
        var result = repository.GetRoleRights(id);
        return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    [HttpGet]
    public HttpResponseMessage GetUserRoles(string id)
    {
        var result = repository.GetUserRoles(id);
        return Request.CreateResponse(HttpStatusCode.OK, result);
    }

    [HttpPut]
    public HttpResponseMessage SaveRights([FromBody]List<Group> rights, string id)
    {
        repository.SaveRights(rights, id); 

        return Request.CreateResponse(HttpStatusCode.OK);
    }
}