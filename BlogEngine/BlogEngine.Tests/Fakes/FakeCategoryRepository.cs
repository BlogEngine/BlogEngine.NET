using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeCategoryRepository : ICategoryRepository
    {
        private const string testTitle = "test";

        public IEnumerable<CategoryItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<CategoryItem>
            {
                new CategoryItem()
                {
                    Id = Guid.NewGuid(),
                    Title = testTitle
                }
            };

            return items;
        }

        public CategoryItem FindById(Guid id)
        {
            return new CategoryItem()
            {
                Id = Guid.NewGuid(),
                Title = testTitle
            };
        }

        public CategoryItem Add(CategoryItem item)
        {
            return new CategoryItem()
            {
                Id = Guid.NewGuid(),
                Title = testTitle
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
