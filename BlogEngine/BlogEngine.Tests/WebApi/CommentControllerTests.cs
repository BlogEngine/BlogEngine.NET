using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            SharedTestData.InitializeController(_ctrl, "controller", "comments");
        }

        [TestMethod]
        public void CommentsControllerGet()
        {
            var results = _ctrl.Get();
            Assert.IsTrue(results.Items.Any());
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
            var result = _ctrl.Delete(SharedTestData.testBlogId);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void CommentsControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "delete");

            var items = new List<CommentItem>
            {
                new CommentItem() { IsChecked = true, Id = Guid.NewGuid() }
            };

            var result = _ctrl.ProcessChecked(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
