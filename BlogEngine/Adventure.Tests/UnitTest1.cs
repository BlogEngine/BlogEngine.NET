using Adventure.Common.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adventure.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            dynamic profile = new DynamicProfile();
            profile.LastName = "Kratochvil";
            profile.FirstName = "Bill";

        }
    }
}
