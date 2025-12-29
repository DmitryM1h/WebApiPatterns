using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CriticalEvents.Domain.Interfaces
{
    public interface IAccidentStorageFactory
    {
        public Task<IAccidentStorage> CreateAccidentStorage();
    }
}
