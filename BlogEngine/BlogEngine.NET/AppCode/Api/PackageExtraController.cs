using BlogEngine.Core.Packaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BlogEngine.NET.AppCode.Api
{
    public class PackageExtraController : ApiController
    {
        public IEnumerable<PackageExtra> Get()
        {
            return Gallery.GetPackageExtras();
        }

        public HttpResponseMessage Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);

            var result = Gallery.GetPackageExtra(id);
            if (result != null)
                return Request.CreateResponse(HttpStatusCode.OK, result);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
