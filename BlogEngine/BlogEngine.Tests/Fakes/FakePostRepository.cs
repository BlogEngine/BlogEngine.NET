using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakePostRepository : IPostRepository
    {
        public IEnumerable<PostItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<PostItem>();
            items.Add(new PostItem());
            return items;
        }

        public PostDetail FindById(Guid id)
        {
            return new PostDetail() { Id = Guid.NewGuid() };
        }

        public PostDetail Add(PostDetail post)
        {
            return new PostDetail() { Id = Guid.NewGuid() };
        }

        public bool Update(PostDetail post, string action)
        {
            return true;
        }

        public bool Remove(Guid id)
        {
            return true;
        }
    }
}
