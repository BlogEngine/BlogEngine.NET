using BlogEngine.Core;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Packaging;
using BlogEngine.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class GalleryFeedsController : ApiController
{
    const string _metaExtension = "MetaExtension";
    const string _galleryFeeds = "GalleryFeeds";
    
    public IEnumerable<SelectOption> Get()
    {
        var feeds = new List<SelectOption>();
        var feedSets = ExtensionManager.GetSettings(_metaExtension, _galleryFeeds);
        var feedService = "http://dnbe.net/v01/nuget";

        if (!string.IsNullOrEmpty(BlogSettings.Instance.GalleryFeedUrl))
            feedService = BlogSettings.Instance.GalleryFeedUrl;

        if (feedSets == null)
        {
            var settings = new ExtensionSettings(_galleryFeeds);
            settings.AddParameter("OptionName", "Title", 150, true, true);
            settings.AddParameter("OptionValue");

            settings.AddValues(new[] { "dnbe.net", feedService });

            feedSets = ExtensionManager.InitSettings(_metaExtension, settings);
            ExtensionManager.SaveSettings(_metaExtension, feedSets);
        }

        var table = feedSets.GetDataTable();
        foreach (DataRow row in table.Rows)
        {
            feeds.Add(new SelectOption { OptionName = row[0].ToString(), OptionValue = row[1].ToString() });
        }

        return feeds;
    }

    [HttpPost]
    public HttpResponseMessage Post([FromBody]SelectOption item)
    {
        if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminSettingsPages))
            throw new System.UnauthorizedAccessException();

        var feedSets = ExtensionManager.GetSettings(_metaExtension, _galleryFeeds);

        if (feedSets == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        feedSets.AddValues(new[] { item.OptionName, item.OptionValue });
        ExtensionManager.SaveSettings(feedSets);

        return Request.CreateResponse(HttpStatusCode.Created);
    }

    [HttpPut]
    public HttpResponseMessage Update([FromBody]SelectOption item)
    {
        if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminSettingsPages))
            throw new System.UnauthorizedAccessException();

        var bs = BlogSettings.Instance;
        bs.GalleryFeedUrl = item.OptionValue;
        bs.Save();

        // remove gallery packages from the cache
        BlogEngine.Core.Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

        // remove resources from the cache to force refresh
        var cacheKey = "admin.resource.axd - " + BlogSettings.Instance.Culture;
        BlogEngine.Core.Blog.CurrentInstance.Cache.Remove(cacheKey);

        return Request.CreateResponse(HttpStatusCode.OK);
    }

    public HttpResponseMessage Delete(string id)
    {
        if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminSettingsPages))
            throw new System.UnauthorizedAccessException();

        var feedSets = ExtensionManager.GetSettings(_metaExtension, _galleryFeeds);
        if (feedSets == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);

        var table = feedSets.GetDataTable();
        var indx = 0;
        foreach (DataRow row in table.Rows)
        {
            if (row[0].ToString() == id)
            {
                foreach (var par in feedSets.Parameters)
                {
                    par.DeleteValue(indx);
                }
                ExtensionManager.SaveSettings(_metaExtension, feedSets);
                break;
            }
            indx++;
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

}
