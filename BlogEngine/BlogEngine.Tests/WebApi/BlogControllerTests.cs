﻿using BlogEngine.Tests.Fakes;
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
    public class BlogControllerTests
    {
        private const string testBlogId = SharedTestData.testBlogId;
        private BlogsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new BlogsController(new FakeBlogRepository());

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "blogs" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        [TestMethod]
        public void BlogControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void BlogControllerGetById()
        {
            var blog = _ctrl.Get(id: testBlogId);
            Assert.IsNotNull(blog);
        }

        [TestMethod]
        public void BlogControllerPost()
        {
            var newBlog = new Core.Data.Models.BlogItem();

            var result = _ctrl.Post(newBlog);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var blog = JsonConvert.DeserializeObject<Core.Data.Models.Blog>(json);

            Assert.IsNotNull(blog);
            Assert.IsTrue(blog.Id != Guid.Empty && blog.Id != null);
        }

        [TestMethod]
        public void BlogControllerUpdate()
        {
            var result = _ctrl.Update(new BlogEngine.Core.Data.Models.Blog()
            {
                Id = Guid.NewGuid()
            });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void BlogControllerDelete()
        {
            var result = _ctrl.Delete(testBlogId);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void BlogControllerProcessChecked()
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "id", "delete" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            var items = new List<BlogEngine.Core.Data.Models.Blog>();
            items.Add(new BlogEngine.Core.Data.Models.Blog()
            {
                IsChecked = true,
                Id = Guid.NewGuid()
            });

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}