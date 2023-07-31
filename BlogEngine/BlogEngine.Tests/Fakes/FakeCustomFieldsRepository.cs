using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeCustomFieldsRepository : ICustomFieldRepository
    {
        private const string testType = "test";

        public IEnumerable<CustomField> Find(string filter = "")
        {
            var items = new List<CustomField>
            {
                new CustomField { CustomType = testType, BlogId = new Guid() }
            };
            return items;
        }

        public CustomField FindById(string type, string id, string key)
        {
            return new CustomField { CustomType = testType, BlogId = new Guid() };
        }

        public CustomField Add(CustomField item)
        {
            return new CustomField { CustomType = testType, BlogId = new Guid() };
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
