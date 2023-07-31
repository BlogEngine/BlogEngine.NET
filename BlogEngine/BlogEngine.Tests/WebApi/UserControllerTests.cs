using System.Collections.Generic;
using System.Net;
using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class UserControllerTests
    {
        private UsersController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new UsersController(new FakeUsersRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "users");
        }

        [TestMethod]
        public void UsersControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void UsersControllerGetById()
        {
            var item = _ctrl.Get("test");
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void UsersControllerPost()
        {
            var result = _ctrl.Post(new BlogUser());
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<BlogUser>(json);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void UsersControllerUpdate()
        {
            var result = _ctrl.Update(new BlogUser());
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void UsersControllerDelete()
        {
            var result = _ctrl.Delete("test");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void UsersControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<BlogUser>
            {
                new BlogUser()
            };

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
