using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class CustomFieldsControllerTests
    {
        private CustomFieldsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new CustomFieldsController(new FakeCustomFieldsRepository());

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "customfields" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        [TestMethod]
        public void CustomFieldsControllerGet()
        {
            var results = _ctrl.Get();
            Assert.IsTrue(results.Any());
        }

        //[TestMethod]
        //public void CustomFieldsControllerPost()
        //{
        //    var result = _ctrl.Post(new CustomField { CustomType = "test", BlogId = new Guid() });
        //    Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        //}

        [TestMethod]
        public void CustomFieldsControllerUpdate()
        {
            var items = new List<CustomField>();
            items.Add(new CustomField { CustomType = "test", BlogId = new Guid() });
            var result = _ctrl.Put(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
