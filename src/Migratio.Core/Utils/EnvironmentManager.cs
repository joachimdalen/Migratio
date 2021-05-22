using System;
using Migratio.Core.Contracts;

namespace Migratio.Core.Utils
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