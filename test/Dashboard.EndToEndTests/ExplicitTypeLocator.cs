using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace Dashboard.EndToEndTests
{
    public class ExplicitTypeLocator : ITypeLocator
    {
        private readonly Type[] _types;

        public ExplicitTypeLocator(params Type[] types)
        {
            _types = types;
        }

        public IReadOnlyList<Type> GetTypes()
        {
            return _types;
        }
    }
}
