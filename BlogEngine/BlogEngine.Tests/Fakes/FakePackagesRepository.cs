using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakePackagesRepository : IPackageRepository
    {
        public IEnumerable<Package> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<Package>();
            items.Add(new Package() { Id = "test" });
            return items;
        }

        public Package FindById(string id)
        {
            return new Package() { Id = "test" };
        }

        public bool Install(string id)
        {
            return true;
        }

        public bool Uninstall(string id)
        {
            return true;
        }

        public bool Update(Package item)
        {
            return true;
        }
    }
}
