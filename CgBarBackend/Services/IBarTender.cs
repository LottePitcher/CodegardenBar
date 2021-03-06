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
        event EventHandler<Patron> DrinkDelivered;
        event EventHandler<string> DrinkExpired;
        IEnumerable<Patron> Patrons { get; }
        IReadOnlyList<string> Drinks { get; }
        IReadOnlyList<string> PoliteWords { get; }
        IReadOnlyList<BarTenderMessage> Messages { get; }
        void AddPatron(string screenName, string name, string profileImage, string byScreenName = null);
        void OrderDrink(string screenName, string drink, bool polite = false, string byScreenName = null);
        Task Load();
        bool BanPatron(string screenName);
        bool UnBanPatron(string screenName);
        bool PatronExists(string screenName);
        bool AddDrink(string name);
        bool RemoveDrink(string name);
        bool AddPoliteWord(string name);
        bool RemovePoliteWord(string name);
        event EventHandler<Patron> PatronPolitenessChanged;
        bool AddMessage(BarTenderMessage message);
        bool RemoveMessage(int index);
        event EventHandler<string> BarTenderSpeaks;
        void RefillDrink(string screenName, bool polite = false, string byScreenName = null);
    }
}