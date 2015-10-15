using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Tests.Fakes
{
    class FakeLookupsRepository : ILookupsRepository
    {
        public Lookups GetLookups()
        {
            return new Lookups();
        }

        public void SaveEditorOptions(EditorOptions options)
        {

        }
    }
}
