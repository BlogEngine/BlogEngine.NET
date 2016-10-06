using BlogEngine.Core.FileSystem;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    public interface IFileManagerRepository
    {
        IEnumerable<FileInstance> Find(int take = 10, int skip = 0, string path = "", string order = "");
    }
}
