using BlogEngine.Core.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class PingServicesController : ApiController
{
    public IEnumerable<PingItem> Get()
    {
        var pings = new List<PingItem>();

        foreach (var s in BlogService.LoadPingServices())
        {
            pings.Add(new PingItem { IsChecked = false, ServiceUrl = s });
        }
        return pings;
    }

    public HttpResponseMessage Post([FromBody]PingItem item)
    {
        var sc = BlogService.LoadPingServices();
        if (!sc.Contains(item.ServiceUrl))
        {
            sc.Add(item.ServiceUrl);
        }
        BlogService.SavePingServices(sc);

        var pings = new List<PingItem>();
        foreach (var s in sc)
        {
            pings.Add(new PingItem { IsChecked = false, ServiceUrl = s });
        }
        return Request.CreateResponse(HttpStatusCode.Created, pings);
    }

    [HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<PingItem> items)
    {
        if (items == null || items.Count == 0)
            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

        var sc = BlogService.LoadPingServices();

        foreach (var item in items)
        {
            if (item.IsChecked)
            {
                sc.Remove(item.ServiceUrl);
            }
        }
        BlogService.SavePingServices(sc);

        return Request.CreateResponse(HttpStatusCode.OK);
    }
}

public class PingItem
{
    public bool IsChecked { get; set; }
    public string ServiceUrl { get; set; }
}