using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;

namespace BlogEngine.Tests.Fakes
{
    class FakeLookupsRepository : ILookupsRepository
    {
        public Lookups GetLookups()
        {
            return new Lookups();
        }
    }
}
