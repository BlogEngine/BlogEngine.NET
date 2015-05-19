using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeCategoryRepository : ICategoryRepository
    {

        public IEnumerable<CategoryItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<CategoryItem>();

            items.Add(new CategoryItem()
            {
                Id = Guid.NewGuid(),
                Title = "test"
            });

            return items;
        }

        public CategoryItem FindById(Guid id)
        {
            return new CategoryItem()
            {
                Id = Guid.NewGuid(),
                Title = "test"
            };
        }

        public CategoryItem Add(CategoryItem item)
        {
            return new CategoryItem()
            {
                Id = Guid.NewGuid(),
                Title = "test"
            };
        }

        public bool Update(CategoryItem item)
        {
            return true;
        }

        public bool Remove(Guid id)
        {
            return true;
        }
    }
}
