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
    public class BarTenderRepository : FileRepository, IBarTenderRepository
    {
        private string _patronsFile => Path.Combine(BasePath, "patrons.repo.json");

        public BarTenderRepository(IConfiguration configuration) : base(configuration, "BarTender:RepositoryBaseFolder", Path.Combine(Environment.CurrentDirectory, "BarRepository"))
        {
        }

        public async Task SavePatrons(IEnumerable<Patron> patrons)
        {
            await Save(patrons, _patronsFile).ConfigureAwait(false);
        }

        public async Task<Patron[]> LoadPatrons()
        {
            return await Load<Patron[]>(_patronsFile).ConfigureAwait(false);
        }
    }
}
