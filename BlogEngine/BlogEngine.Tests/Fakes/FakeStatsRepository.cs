using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Tests.Fakes
{
    class FakeStatsRepository : IStatsRepository
    {
        public Stats Get()
        {
            return new Stats();
        }
    }
}
