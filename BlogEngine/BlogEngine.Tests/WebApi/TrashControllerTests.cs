using System.Net;
using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class TrashControllerTests
    {
        private TrashController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new TrashController(new FakeTrashRepository());

            SharedTestData.InitializeController(_ctrl, "controller", "trash");
        }

        [TestMethod]
        public void TrashControllerGet()
        {
            var results = _ctrl.Get(0, 0);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TrashControllerPurge()
        {
            var result = _ctrl.Purge(new TrashItem());
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void TrashControllerPurgeAll()
        {
            var result = _ctrl.PurgeAll();
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void TrashControllerRestore()
        {
            var result = _ctrl.Restore(new TrashItem());
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
