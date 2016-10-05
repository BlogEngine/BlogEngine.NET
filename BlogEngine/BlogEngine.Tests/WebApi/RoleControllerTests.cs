using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
    public class RoleControllerTests
    {
        private RolesController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new RolesController(new FakeRolesRepository());

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "roles" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        [TestMethod]
        public void RoleControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void RoleControllerGetById()
        {
            var item = _ctrl.Get("test");
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void RoleControllerPost()
        {
            var result = _ctrl.Post(new RoleItem { RoleName = "test" });
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<RoleItem>(json);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void RoleControllerUpdate()
        {
            var items = new List<RoleItem>();
            items.Add(new RoleItem());
            var result = _ctrl.Put(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerDelete()
        {
            var result = _ctrl.Delete("test");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerProcessChecked()
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/be/api");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "id", "delete" } });

            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            var items = new List<RoleItem>();
            items.Add(new RoleItem());

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerGetRights()
        {
            var result = _ctrl.GetRights("test");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerGetUserRiles()
        {
            var result = _ctrl.GetUserRoles("test");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerSaveRights()
        {
            var rights = new List<Group>();
            var result = _ctrl.SaveRights(rights, "test");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
