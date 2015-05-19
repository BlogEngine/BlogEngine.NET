using BlogEngine.Core;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

public class LogsController : ApiController
{
    [HttpGet]
    public IEnumerable<SelectOption> GetLog()
    {
        if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
            throw new System.UnauthorizedAccessException();

        var action = Request.GetRouteData().Values["id"].ToString();

        switch (action)
        {
            case "file":
                return GetLogFile();
            default:
                break;
        }
        return new List<SelectOption>();
    }

    [HttpPut]
    public HttpResponseMessage PurgeLog()
    {
        if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
            throw new System.UnauthorizedAccessException();

        string fileLocation = HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "logger.txt"));
        try
        {
            File.Delete(fileLocation);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            Utils.Log("Error purging log file", ex);
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    IEnumerable<SelectOption> GetLogFile()
    {
        string fileLocation = HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "logger.txt"));
        var items = new List<SelectOption>();
    
        if (File.Exists(fileLocation))
        {
            using (var sw = new StreamReader(fileLocation))
            {
                string line;
                string logItem = "";
                int count = 1;
                while ((line = sw.ReadLine()) != null)
                {
                    if (line.Contains("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*"))
                    {
                        // new log item
                        if (!string.IsNullOrEmpty(logItem))
                        {
                            var item = new SelectOption();
                            item.OptionName = "Line" + count.ToString();
                            item.OptionValue = logItem;
                            items.Add(item);
                            logItem = "";
                            count++;
                        }
                    }
                    else
                    {
                        // append line to log item
                        logItem = logItem + line + "<br/>";
                    }
                    
                }
                sw.Close();
                return items;
            }
        }
        else
        {
            return new List<SelectOption>();
        }
    }
}
