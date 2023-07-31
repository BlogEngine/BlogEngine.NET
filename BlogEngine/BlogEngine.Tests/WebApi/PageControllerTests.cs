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
    public class PageControllerTests
    {
        private const string testBlogId = SharedTestData.testBlogId;
        private PagesController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new PagesController(new FakePageRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "pages");
        }

        [TestMethod]
        public void PageControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void PageControllerGetById()
        {
            var item = _ctrl.Get(testBlogId);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void PageControllerPost()
        {
            var result = _ctrl.Post(new PageDetail { Id = Guid.NewGuid() });
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<PageDetail>(json);

            Assert.IsNotNull(item);
            Assert.IsTrue(item.Id != Guid.Empty && item.Id != null);
        }

        [TestMethod]
        public void PageControllerUpdate()
        {
            var result = _ctrl.Update(new PageDetail()
            {
                Id = Guid.NewGuid()
            });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void PageControllerDelete()
        {
            var result = _ctrl.Delete(testBlogId);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void PageControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<PageDetail>
            {
                new PageDetail()
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
