using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeRolesRepository : IRolesRepository
    {
        public IEnumerable<RoleItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<RoleItem>();
            items.Add(new RoleItem());
            return items;
        }

        public RoleItem FindById(string id)
        {
            return new RoleItem();
        }

        public RoleItem Add(RoleItem role)
        {
            return new RoleItem();
        }

        public bool Remove(string id)
        {
            return true;
        }

        public IEnumerable<Group> GetRoleRights(string role)
        {
            return new List<Group>();
        }

        public IEnumerable<RoleItem> GetUserRoles(string id)
        {
            return new List<RoleItem>();
        }

        public bool SaveRights(List<Group> rights, string id)
        {
            return true;
        }
    }
}
