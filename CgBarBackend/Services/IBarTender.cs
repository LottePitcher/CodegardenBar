using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CgBarBackend.Models;

namespace CgBarBackend.Services
{
    public interface IBarTender
    {
        event EventHandler<Patron> PatronAdded;
        event EventHandler<string> PatronExpired;
        event EventHandler<Patron> DrinkOrdered;
        event EventHandler<string> DrinkExpired;
        IEnumerable<Patron> Patrons { get; }
        IReadOnlyList<string> Drinks { get; }
        IReadOnlyList<string> PoliteWords { get; }
        void AddPatron(string screenName, string name, string profileImage, string byScreenName = null);
        void OrderDrink(string screenName, string drink, string byScreenName = null);
        Task Load();
        bool BanPatron(string screenName);
        bool UnBanPatron(string screenName);
        bool PatronExists(string screenName);
        bool AddDrink(string name);
        bool RemoveDrink(string name);
        bool AddPoliteWord(string name);
        bool RemovePoliteWord(string name);
    }
}