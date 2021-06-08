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
        void AddPatron(string screenName, string name, string profileImage);
        void OrderDrink(string screenName, string drink);
        Task Load();
        bool BanPatron(string screenName);
        bool UnBanPatron(string screenName);
    }
}