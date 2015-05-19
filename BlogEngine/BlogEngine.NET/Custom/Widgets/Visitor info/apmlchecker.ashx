<%@ WebHandler Language="C#" Class="apmlchecker" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Xml;
using BlogEngine.Core;

public class apmlchecker : IHttpHandler
{

  public void ProcessRequest(HttpContext context)
  {
    context.Response.ContentType = "text/plain";
    string website = context.Request.QueryString["url"];

    Uri apml = RetrieveApmlUrl(context, website);
    if (apml != null)
    {
      context.Response.Write(apml);
    }

    BlogEngine.Core.Web.HttpModules.CompressionModule.CompressResponse(context);
    context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
    context.Response.Cache.VaryByParams["url"] = true;
    context.Response.Cache.SetValidUntilExpires(true);
    context.Response.Cache.SetMaxAge(new TimeSpan(0, 10, 0));
  }

  private static Uri RetrieveApmlUrl(HttpContext context, string website)
  {
    Uri url;
    if (Uri.TryCreate(website, UriKind.Absolute, out url))
    {
      Dictionary<Uri, XmlDocument> docs = Utils.FindSemanticDocuments(url, "apml");
      if (docs.Count > 0)
      {
        foreach (Uri key in docs.Keys)
        {
          return key;
        }
      }
    }

    return null;
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }

}