using System;
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
    public class PostControllerTests
    {
        private const string testBlogId = SharedTestData.testBlogId;
        private PostsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new PostsController(new FakePostRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "posts");  
        }

        [TestMethod]
        public void PostControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void PostControllerGetById()
        {
            var item = _ctrl.Get(testBlogId);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void PostControllerPost()
        {
            var result = _ctrl.Post(new PostDetail { Id = Guid.NewGuid() });
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<PostDetail>(json);

            Assert.IsNotNull(item);
            Assert.IsTrue(item.Id != Guid.Empty && item.Id != null);
        }

        [TestMethod]
        public void PostControllerUpdate()
        {
            var result = _ctrl.Update(new PostDetail()
            {
                Id = Guid.NewGuid()
            });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void PostControllerDelete()
        {
            var result = _ctrl.Delete(SharedTestData.testBlogId);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void PostControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<PostDetail>
            {
                new PostDetail()
                {
                    IsChecked = true,
                    Id = Guid.NewGuid()
                }
            };

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
