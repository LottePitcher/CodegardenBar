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
    }
}