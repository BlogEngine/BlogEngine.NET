using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class CategoryControllerTests
    {
        private const string testBlogId = SharedTestData.testBlogId;
        private CategoriesController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new CategoriesController(new FakeCategoryRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "categories");
        }

        [TestMethod]
        public void CategoryControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void CategoryControllerGetById()
        {
            var blog = _ctrl.Get(testBlogId);
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
            var result = _ctrl.Delete(testBlogId);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void CategoryControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<CategoryItem>
            {
                new CategoryItem()
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