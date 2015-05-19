using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.FileSystem;
using BlogEngine.Core.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class FileManagerController : ApiController
{
    readonly IFileManagerRepository repository;

    public FileManagerController(IFileManagerRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<FileInstance> Get(int take = 10, int skip = 0, string path = "", string order = "")
    {
        return repository.Find(take, skip, path, order);
    }

    [HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<FileInstance> items)
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
                    if(item.FileType == FileType.File || item.FileType == FileType.Image)
                        BlogService.DeleteFile(item.FullPath);

                    if (item.FileType == FileType.Directory)
                        BlogService.DeleteDirectory(item.FullPath);
                }
            }
        }
        return Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpPut]
    public HttpResponseMessage AddFolder(FileInstance folder)
    {
        BlogService.CreateDirectory(folder.FullPath + "/" + folder.Name);
        return Request.CreateResponse(HttpStatusCode.OK);
    }

}