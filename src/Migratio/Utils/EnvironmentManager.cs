using System;
using Migratio.Contracts;

namespace Migratio.Utils
{
    public class EnvironmentManager : IEnvironmentManager
    {
        public string GetEnvironmentVariable(string key) => Environment.GetEnvironmentVariable(key);
    }
}