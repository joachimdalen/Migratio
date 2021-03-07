using System;
using Migratio.Contracts;

namespace Migratio.Utils
{
    public class EnvironmentManager : IEnvironmentManager
    {
        /// <inheritdoc /> 
        public string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}