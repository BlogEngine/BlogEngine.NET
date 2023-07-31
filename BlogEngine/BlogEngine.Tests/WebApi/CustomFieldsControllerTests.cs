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
    public class CustomFieldsControllerTests
    {
        private CustomFieldsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new CustomFieldsController(new FakeCustomFieldsRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "customfields");
        }

        [TestMethod]
        public void CustomFieldsControllerGet()
        {
            var results = _ctrl.Get();
            Assert.IsTrue(results.Any());
        }

        //[TestMethod]
        //public void CustomFieldsControllerPost()
        //{
        //    var result = _ctrl.Post(new CustomField { CustomType = "test", BlogId = new Guid() });
        //    Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        //}

        [TestMethod]
        public void CustomFieldsControllerUpdate()
        {
            var items = new List<CustomField>
            {
                new CustomField { CustomType = "test", BlogId = new Guid() }
            };
            var result = _ctrl.Put(items);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
