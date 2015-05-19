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
    public class CommentControllerTests
    {
        private CommentsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new CommentsController(new FakeCommentRepository());

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "comments" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        [TestMethod]
        public void CommentsControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Count() > 0);
        }

        [TestMethod]
        public void CommentsControllerGetById()
        {
            var item = _ctrl.Get("96d5b379-7e1d-4dac-a6ba-1e50db561b04");
            Assert.IsNotNull(item);
        }

        //[TestMethod]
        //public void CategoryControllerPost()
        //{
        //    var result = _ctrl.Post(new CommentItem { Id = Guid.NewGuid() });
        //    Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

        //    var json = result.Content.ReadAsStringAsync().Result;
        //    var item = JsonConvert.DeserializeObject<CategoryItem>(json);

        //    Assert.IsNotNull(item);
        //    Assert.IsTrue(item.Id != Guid.Empty && item.Id != null);
        //}

        [TestMethod]
        public void CommentsControllerUpdate()
        {
            var result = _ctrl.Put(new CommentItem() { Id = Guid.NewGuid() });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void CommentsControllerDelete()
        {
            var result = _ctrl.Delete("96d5b379-7e1d-4dac-a6ba-1e50db561b04");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void CommentsControllerProcessChecked()
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "id", "delete" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            var items = new List<CommentItem>();
            items.Add(new CommentItem() { IsChecked = true, Id = Guid.NewGuid() });

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
