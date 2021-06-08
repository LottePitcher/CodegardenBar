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
        private string _bannedPatronsFile => Path.Combine(BasePath, "bannedPatrons.repo.json");
        private string _drinksFile => Path.Combine(BasePath, "drinks.repo.json");
        private string _politeWordsFile => Path.Combine(BasePath, "politeWords.repo.json");

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

        public async Task SaveBannedPatrons(IEnumerable<string> bannedPatronScreenNames)
        {
            await Save(bannedPatronScreenNames, _bannedPatronsFile).ConfigureAwait(false);
        }

        public async Task<string[]> LoadBannedPatrons()
        {
            return await Load<string[]>(_bannedPatronsFile).ConfigureAwait(false);
        }

        public async Task SaveDrinks(IEnumerable<string> drinks)
        {
            await Save(drinks, _drinksFile).ConfigureAwait(false);
        }

        public async Task<string[]> LoadDrinks()
        {
            return await Load<string[]>(_drinksFile).ConfigureAwait(false);
        }

        public async Task SavePoliteWords(IEnumerable<string> politeWords)
        {
            await Save(politeWords, _politeWordsFile).ConfigureAwait(false);
        }

        public async Task<string[]> LoadPoliteWords()
        {
            return await Load<string[]>(_politeWordsFile).ConfigureAwait(false);
        }
    }
}
