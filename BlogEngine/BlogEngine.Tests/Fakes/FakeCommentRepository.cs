using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.ViewModels;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeCommentRepository : ICommentsRepository
    {
        public CommentsVM Get()
        {
            var vm = new CommentsVM();
            vm.Items = new List<CommentItem>();
            vm.Items.Add(new CommentItem() { Id = Guid.NewGuid() });
            return vm;
        }

        public CommentDetail FindById(Guid id)
        {
            return new CommentDetail() { Id = Guid.NewGuid() };
        }

        public CommentItem Add(CommentDetail item)
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