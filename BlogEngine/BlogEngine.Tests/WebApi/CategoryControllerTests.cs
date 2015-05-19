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
    public class CategoryControllerTests
    {
        private CategoriesController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new CategoriesController(new FakeCategoryRepository());

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "categories" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        [TestMethod]
        public void CategoryControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Count() > 0);
        }

        [TestMethod]
        public void CategoryControllerGetById()
        {
            var blog = _ctrl.Get("96d5b379-7e1d-4dac-a6ba-1e50db561b04");
            Assert.IsNotNull(blog);
        }

        [TestMethod]
        public void CategoryControllerPost()
        {
            var result = _ctrl.Post(new CategoryItem { Id = Guid.NewGuid() });
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<CategoryItem>(json);

            Assert.IsNotNull(item);
            Assert.IsTrue(item.Id != Guid.Empty && item.Id != null);
        }

        [TestMethod]
        public void CategoryControllerUpdate()
        {
            var result = _ctrl.Update(new CategoryItem()
            {
                Id = Guid.NewGuid()
            });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void CategoryControllerDelete()
        {
            var result = _ctrl.Delete("96d5b379-7e1d-4dac-a6ba-1e50db561b04");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void CategoryControllerProcessChecked()
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "id", "delete" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            var items = new List<CategoryItem>();
            items.Add(new CategoryItem()
            {
                IsChecked = true,
                Id = Guid.NewGuid()
            });

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}