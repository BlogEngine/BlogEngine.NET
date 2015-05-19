using BlogEngine.Core.Data.Models;
using BlogEngine.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace BlogEngine.Tests.WebApi
{
    [TestClass]
    public class StatsControllerTests
    {
        private StatsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new StatsController(new FakeStatsRepository());
        }

        [TestMethod]
        public void StatsControllerGet()
        {
            var results = _ctrl.Get();
            Assert.IsNotNull(results);
        }
    }
}
