using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace BlogEngine.Tests.WebApi
{
    internal class SharedTestData
    {
        /// <summary>
        /// ID for blog used in tests (matches ID in FakeBlogRepository)
        /// </summary>
        public const string testBlogId = "96d5b379-7e1d-4dac-a6ba-1e50db561b04";

        static internal void InitializeController(ApiController initController,
                                            string whichController,
                                            string whichAction)
        {
            if (initController is null)
            {
                throw new ArgumentNullException(nameof(initController));
            }

            if (string.IsNullOrEmpty(whichController))
            {
                throw new ArgumentException($"'{nameof(whichController)}' cannot be null or empty.", nameof(whichController));
            }

            if (string.IsNullOrEmpty(whichAction))
            {
                throw new ArgumentException($"'{nameof(whichAction)}' cannot be null or empty.", nameof(whichAction));
            }

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { whichController, whichAction } });
            initController.ControllerContext = new HttpControllerContext(config, routeData, request);
            initController.Request = request;
            initController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }
    }
}