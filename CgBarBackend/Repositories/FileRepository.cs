using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CgBarBackend.Repositories
{
    public class FileRepository
    {
        protected readonly string BasePath;
        
        public FileRepository(IConfiguration configuration, string basePathConfigurationItem, string basePathFallback)
        {
            var basePath = configuration[basePathConfigurationItem];
            basePath = basePath.Replace("[CurrentDirectory]", Environment.CurrentDirectory);
            BasePath = basePath.Trim().Length > 0 ? basePath : basePathFallback;
            Directory.CreateDirectory(BasePath);
        }

        protected async Task Save(object saveObject, string path)
        {
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(saveObject)).ConfigureAwait(false);
        }

        protected async Task<T> Load<T>(string path)
        {
            if (File.Exists(path) == false)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(
                await File.ReadAllTextAsync(path).ConfigureAwait(false)
            );
        }
    }
}
