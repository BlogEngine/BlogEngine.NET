using System.Collections.Generic;
using System.Net;
using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class TagControllerTests
    {
        private TagsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new TagsController(new FakeTagsRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "tags");
        }

        [TestMethod]
        public void TagsControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TagsControllerUpdate()
        {
            var result = _ctrl.Update(new TagToUpdate());
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void TagsControllerDelete()
        {
            var result = _ctrl.Delete("test");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void TagsControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<TagItem>
            {
                new TagItem()
                {
                    IsChecked = true,
                    TagName = "test"
                }
            };

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
