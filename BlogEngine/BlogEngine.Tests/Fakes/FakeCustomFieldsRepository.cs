using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeCustomFieldsRepository : ICustomFieldRepository
    {
        public IEnumerable<CustomField> Find(string filter = "")
        {
            var items = new List<CustomField>();
            items.Add(new CustomField { CustomType = "test", BlogId = new Guid() });
            return items;
        }

        public CustomField FindById(string type, string id, string key)
        {
            return new CustomField { CustomType = "test", BlogId = new Guid() };
        }

        public CustomField Add(CustomField item)
        {
            return new CustomField { CustomType = "test", BlogId = new Guid() };
        }

        public bool Update(CustomField item)
        {
            return true;
        }

        public bool Remove(string type, string id, string key)
        {
            return true;
        }

        public void ClearCustomFields(string customType, string objectType)
        {

        }
    }
}
