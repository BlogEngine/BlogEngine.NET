using BlogEngine.Core;
using BlogEngine.Core.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class BlogRollController : ApiController
{
    public BlogRollController()
    {
    }

    public IEnumerable<BlogRollItem> Get()
    {
        var vm = new BlogRollVM();
        return vm.BlogRolls;
    }

    public HttpResponseMessage Get(string id)
    {
        Guid gId;
        if(Guid.TryParse(id, out gId))
        {
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        else
        {
            var vm = new BlogRollVM();
            var result = vm.BlogRolls.Find(b => b.Id == gId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }

    public HttpResponseMessage Post(BlogRollItem item)
    {
        BlogEngine.Core.Providers.BlogService.InsertBlogRoll(item);
        return Request.CreateResponse(HttpStatusCode.Created, item);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]BlogRollItem item)
    {
        BlogEngine.Core.Providers.BlogService.UpdateBlogRoll(item);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    private string getUrl(string url)
    {
        if (!string.IsNullOrEmpty(url) && !url.Contains("://"))
            url = "http://" + url;
        return url;
    }

    //private void Resort()
    //{
    //    int sortIndex = -1;
    //    // Re-sort remaining items starting from zero to eliminate any possible gaps.
    //    // Need to cast BlogRollItem.BlogRolls to an array to
    //    // prevent errors with modifying a collection while enumerating it.
    //    foreach (BlogRollItem brItem in BlogRollItem.BlogRolls.ToArray())
    //    {
    //        brItem.SortIndex = ++sortIndex;
    //        brItem.Save();
    //    }
    //}
}