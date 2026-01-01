using CriticalEvents.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Persistence
{
    public class AccidentStorageFactory : IAccidentStorageFactory
    {
        public Task<IAccidentStorage> CreateAccidentStorage()
        {
            var storage = new AccidentStorage();
            return storage;
        }
    }
}
