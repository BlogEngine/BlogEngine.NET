using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakePageRepository : IPageRepository
    {
        public IEnumerable<PageItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<PageItem>();
            items.Add(new PageItem());
            return items;
        }

        public PageDetail FindById(Guid id)
        {
            return new PageDetail() { Id = Guid.NewGuid() };
        }

        public PageDetail Add(PageDetail page)
        {
            return new PageDetail() { Id = Guid.NewGuid() };
        }

        public bool Update(PageDetail page, string action)
        {
            return true;
        }

        public bool Remove(Guid id)
        {
            return true;
        }
    }
}
