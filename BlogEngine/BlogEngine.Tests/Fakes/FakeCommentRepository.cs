using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeCommentRepository : ICommentsRepository
    {
        public IEnumerable<CommentItem> GetComments(CommentType commentType = CommentType.All, int take = 10, int skip = 0, string filter = "", string order = "")
        {
            var items = new List<CommentItem>();
            items.Add(new CommentItem() { Id = Guid.NewGuid() });
            return items;
        }

        public CommentItem FindById(Guid id)
        {
            return new CommentItem() { Id = Guid.NewGuid() };
        }

        public CommentItem Add(CommentItem item)
        {
            return new CommentItem() { Id = Guid.NewGuid() };
        }

        public bool Update(CommentItem item, string action)
        {
            return true;
        }

        public bool Remove(Guid id)
        {
            return true;
        }

        public bool DeleteAll(string commentType)
        {
            return true;
        }
    }
}