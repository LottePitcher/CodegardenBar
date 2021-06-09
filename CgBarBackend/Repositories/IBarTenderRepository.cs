using System.Collections.Generic;
using System.Threading.Tasks;
using CgBarBackend.Models;

namespace CgBarBackend.Repositories
{
    public interface IBarTenderRepository
    {
        Task SavePatrons(IEnumerable<Patron> patrons);
        Task<Patron[]> LoadPatrons();
        Task SaveBannedPatrons(IEnumerable<string> bannedPatronScreenNames);
        Task<string[]> LoadBannedPatrons();
        Task SaveDrinks(IEnumerable<string> drinks);
        Task<string[]> LoadDrinks();
        Task SavePoliteWords(IEnumerable<string> politeWords);
        Task<string[]> LoadPoliteWords();
        Task SaveOrders(IEnumerable<Order> politeWords);
        Task<Order[]> LoadOrders();
        Task<BarTenderMessage[]> LoadMessages();
        Task SaveMessages(IEnumerable<BarTenderMessage> message);
    }
}