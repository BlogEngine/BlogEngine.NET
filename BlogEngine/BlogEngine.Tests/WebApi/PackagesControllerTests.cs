using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class PackagesControllerTests
    {
        private PackagesController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new PackagesController(new FakePackagesRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "packages");
        }

        [TestMethod]
        public void PackagesControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void PackagesControllerGetById()
        {
            var item = _ctrl.Get("test");
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void PackageControllerUpdate()
        {
            var result = _ctrl.Update(new Package() { Id = "test" });
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PackagesControllerProcessChecked()
        {
            SharedTestData.InitializeController(_ctrl, "id", "install");

            var items = new List<Package>()
            {
                new Package() { Id = "test" }
            };

            var result = _ctrl.ProcessChecked(items);
            Assert.IsTrue(result.IsSuccessStatusCode);
        }
    }
}
