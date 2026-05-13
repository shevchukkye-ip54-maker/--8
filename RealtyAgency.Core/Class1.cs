using System;
using System.Collections.Generic;
using System.Linq;

namespace RealtyAgency.Core
{
    public interface IEntity { int Id { get; set; } }

    public abstract class Person : IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Client : Person
    {
        public string BankAccountNumber { get; set; }
        public List<Offer> Offers { get; set; } = new List<Offer>();
        public override string ToString() => $"[ID:{Id}] {LastName} {FirstName} | Рахунок: {BankAccountNumber}";
    }

    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public Address(string city, string street) { City = city; Street = street; }
    }

    public abstract class RealEstate : IEntity
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public Address Location { get; set; }
        public string TypeName { get; protected set; }

        protected RealEstate(int id, decimal price, string city, string street)
        {
            Id = id; Price = price;
            Location = new Address(city, street);
        }
        public abstract string GetDetails();
    }

    public class Apartment : RealEstate
    {
        public int Rooms { get; set; }
        public Apartment(int id, decimal price, string city, string street, int rooms)
            : base(id, price, city, street)
        {
            Rooms = rooms;
            TypeName = (rooms > 0) ? $"{rooms}-кімнатна квартира" : "Приватна ділянка";
        }
        public override string GetDetails() => $"[{TypeName}] ID:{Id}, вул. {Location.Street}, ціна: {Price}$";
    }

    public class Offer
    {
        public RealEstate Property { get; set; }
        public DateTime DateCreated { get; set; }
        public Offer(RealEstate property) { Property = property; DateCreated = DateTime.Now; }
    }

    public class AgencyService
    {
        private List<Client> _clients = new List<Client>();
        private List<RealEstate> _properties = new List<RealEstate>();

        // --- КЛІЄНТИ ---
        public void AddClient(Client c) => _clients.Add(c);
        public bool DeleteClient(int id) => _clients.RemoveAll(c => c.Id == id) > 0;
        public Client GetClient(int id) => _clients.FirstOrDefault(c => c.Id == id);

        public void UpdateClient(int id, string newLastName, string newBank)
        {
            var c = _clients.FirstOrDefault(x => x.Id == id);
            if (c != null)
            {
                c.LastName = newLastName;
                c.BankAccountNumber = newBank;
            }
        }

        public List<Client> GetClientsSorted(int mode) => mode switch
        {
            1 => _clients.OrderBy(c => c.FirstName).ToList(),
            2 => _clients.OrderBy(c => c.LastName).ToList(),
            3 => _clients.OrderBy(c => c.BankAccountNumber.FirstOrDefault()).ToList(),
            _ => _clients
        };
        /// </summary>
        /// Нерухомість
        /// </summary>
        public void AddProperty(RealEstate p) => _properties.Add(p);
        public bool DeleteProperty(int id) => _properties.RemoveAll(p => p.Id == id) > 0;
        public RealEstate GetProperty(int id) => _properties.FirstOrDefault(p => p.Id == id);

        public List<RealEstate> GetPropertiesSorted(int mode) => mode switch
        {
            1 => _properties.OrderBy(p => p.TypeName).ToList(),
            2 => _properties.OrderBy(p => p.Price).ToList(),
            _ => _properties
        };
        /// </summary>
        // Пропозиції
        /// </summary>
        public void CreateOffer(int cId, int pId)
        {
            var c = GetClient(cId);
            var p = GetProperty(pId);
            if (c == null || p == null) throw new Exception("Клієнта або об'єкт не знайдено!");
            if (c.Offers.Count >= 5) throw new Exception("Вимога n < 5: не можна більше 5 пропозицій!");
            c.Offers.Add(new Offer(p));
        }

        public void RejectOffer(int cId, int pId)
        {
            var c = GetClient(cId);
            if (c != null) c.Offers.RemoveAll(o => o.Property.Id == pId);
        }

        public List<RealEstate> CheckAvailability(string type, decimal maxPrice)
        {
            return _properties.Where(p => p.TypeName.Contains(type) && p.Price <= maxPrice).ToList();
        }

        public List<Client> SearchClients(string key) =>
            _clients.Where(c => c.LastName.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                               c.FirstName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<RealEstate> SearchProperties(string key) =>
            _properties.Where(p => p.GetDetails().Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Client> AdvancedSearch(string ln, string type) =>
            _clients.Where(c => c.LastName.Contains(ln) && c.Offers.Any(o => o.Property.TypeName.Contains(type))).ToList();

        /// <summary>
        /// Пошук по всім даним: і серед клієнтів, і серед об'єктів нерухомості (Вимога 4.3).
        /// </summary>
        public List<object> SearchEverything(string key)
        {
            var results = new List<object>();
            /// </summary>
            // Пошук серед клієнтів (ім'я або прізвище)
            /// </summary>
            var clients = _clients.Where(c =>
                c.FirstName.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();
            /// </summary>
            // Шукаємо серед нерухомості (опис або вулиця)
            /// </summary>
            var properties = _properties.Where(p =>
                p.GetDetails().Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();

            results.AddRange(clients);
            results.AddRange(properties);

            return results;
        }
    }
}