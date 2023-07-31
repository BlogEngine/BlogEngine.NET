using System.Collections.Generic;
using System.Linq;
using System.Net;
using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class RoleControllerTests
    {
        private const string roleTest = "test";
        private RolesController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new RolesController(new FakeRolesRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "roles");
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
            var item = _ctrl.Get(roleTest);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void RoleControllerPost()
        {
            var result = _ctrl.Post(new RoleItem { RoleName = roleTest });
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
            var result = _ctrl.Delete(roleTest);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<RoleItem>
            {
                new RoleItem()
            };

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerGetRights()
        {
            var result = _ctrl.GetRights(roleTest);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerGetUserRiles()
        {
            var result = _ctrl.GetUserRoles(roleTest);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void RoleControllerSaveRights()
        {
            var rights = new List<Group>();
            var result = _ctrl.SaveRights(rights, roleTest);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
