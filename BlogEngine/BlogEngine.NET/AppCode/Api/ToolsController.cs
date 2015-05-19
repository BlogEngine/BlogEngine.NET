using App_Code;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.UI.HtmlControls;

public class ToolsController : ApiController
{
    private static object _SyncRoot = new object();

    public HttpResponseMessage Get(string id)
    {
        if (id == "check1")
        {
            var result = string.Format("ASP.NET application Identity is: {0}", WindowsIdentity.GetCurrent().Name);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        else if (id == "trust")
        {
            var trust = GetCurrentTrustLevel();
            if (trust == AspNetHostingPermissionLevel.High || trust == AspNetHostingPermissionLevel.Unrestricted)
            {
                var data = new { success = true, msg = "Trust level: " + trust.ToString() };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                var data = new { success = false, msg = "Trust level: " + trust.ToString() };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
        }
        else
        {
            string fileName = GetFileName(id);
            var processed = Process(fileName);

            if (string.IsNullOrEmpty(processed))
            {
                var data = new { success = true, msg = "Can create and delete file " + fileName };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                var data = new { success = false, msg = "Error creating file " + fileName };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }             
        }
    }

    #region Private methods

    private string Process(string file)
    {
        string processed = WriteToFile(file);

        if (!string.IsNullOrEmpty(processed))
            return processed;
        
        return DeleteFile(file);
    }

    private string WriteToFile(string file)
    {
        lock (_SyncRoot)
        {
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(@"*-*-*");
                        sw.Close();
                        fs.Close();

                        return "";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    private string DeleteFile(string file)
    {
        try
        {
            File.Delete(file);
            return "";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private static string GetFileName(string dir)
    {
        if(dir == "root")
            return HostingEnvironment.MapPath("~/tmp.txt");

        if (dir == "data")
            return HostingEnvironment.MapPath("~/App_Data/tmp.txt");

        return HostingEnvironment.MapPath(string.Format("~/{0}/tmp.txt", dir));
    }

    AspNetHostingPermissionLevel GetCurrentTrustLevel()
    {
        foreach (AspNetHostingPermissionLevel trustLevel in new AspNetHostingPermissionLevel[] 
        {
            AspNetHostingPermissionLevel.Unrestricted,
            AspNetHostingPermissionLevel.High,
            AspNetHostingPermissionLevel.Medium,
            AspNetHostingPermissionLevel.Low,
            AspNetHostingPermissionLevel.Minimal 
        })
        {
            try
            {
                new AspNetHostingPermission(trustLevel).Demand();
            }
            catch (System.Security.SecurityException)
            {
                continue;
            }
            return trustLevel;
        }
        return AspNetHostingPermissionLevel.None;
    }

    #endregion
}