using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeTrashRepository : ITrashRepository
    {
        public TrashVM GetTrash(TrashType trashType, int take = 10, int skip = 0, string filter = "", string order = "")
        {
            return new TrashVM();
        }

        public bool Restore(string trashType, Guid id)
        {
            return true;
        }

        public bool Purge(string trashType, Guid id)
        {
            return true;
        }

        public bool PurgeAll()
        {
            return true;
        }

        public JsonResponse PurgeLogfile()
        {
            return new JsonResponse();
        }
    }
}
