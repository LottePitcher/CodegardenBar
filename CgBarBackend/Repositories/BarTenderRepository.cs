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
        private string PatronsFile => Path.Combine(BasePath, "patrons.repo.json");
        private string BannedPatronsFile => Path.Combine(BasePath, "bannedPatrons.repo.json");
        private string DrinksFile => Path.Combine(BasePath, "drinks.repo.json");
        private string PoliteWordsFile => Path.Combine(BasePath, "politeWords.repo.json");
        private string OrdersFile => Path.Combine(BasePath, "orders.repo.json");

        public BarTenderRepository(IConfiguration configuration) : base(configuration, "BarTender:RepositoryBaseFolder", Path.Combine(Environment.CurrentDirectory, "BarRepository"))
        {
        }

        public async Task SavePatrons(IEnumerable<Patron> patrons)
        {
            await Save(patrons, PatronsFile).ConfigureAwait(false);
        }

        public async Task<Patron[]> LoadPatrons()
        {
            return await Load<Patron[]>(PatronsFile).ConfigureAwait(false);
        }

        public async Task SaveBannedPatrons(IEnumerable<string> bannedPatronScreenNames)
        {
            await Save(bannedPatronScreenNames, BannedPatronsFile).ConfigureAwait(false);
        }

        public async Task<string[]> LoadBannedPatrons()
        {
            return await Load<string[]>(BannedPatronsFile).ConfigureAwait(false);
        }

        public async Task SaveDrinks(IEnumerable<string> drinks)
        {
            await Save(drinks, DrinksFile).ConfigureAwait(false);
        }

        public async Task<string[]> LoadDrinks()
        {
            return await Load<string[]>(DrinksFile).ConfigureAwait(false);
        }

        public async Task SavePoliteWords(IEnumerable<string> politeWords)
        {
            await Save(politeWords, PoliteWordsFile).ConfigureAwait(false);
        }

        public async Task<string[]> LoadPoliteWords()
        {
            return await Load<string[]>(PoliteWordsFile).ConfigureAwait(false);
        }

        public async Task SaveOrders(IEnumerable<Order> politeWords)
        {
            await Save(politeWords, OrdersFile).ConfigureAwait(false);
        }

        public async Task<Order[]> LoadOrders()
        {
            return await Load<Order[]>(OrdersFile).ConfigureAwait(false);
        }
    }
}
