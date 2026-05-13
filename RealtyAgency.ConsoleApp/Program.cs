using System;
using RealtyAgency.Core;

namespace RealtyAgency.ConsoleUI
{
    class Program
    {
        static AgencyService service = new AgencyService();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n--- ГОЛОВНЕ МЕНЮ ---");
                Console.WriteLine("1. Управління клієнтами");
                Console.WriteLine("2. Управління нерухомістю");
                Console.WriteLine("3. Управління пропозиціями");
                Console.WriteLine("4. Пошук");
                Console.WriteLine("0. Вихід");
                Console.Write("Вибір: ");
                string choice = Console.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1": ClientSubMenu(); break;
                    case "2": PropertySubMenu(); break;
                    case "3": OfferSubMenu(); break;
                    case "4": SearchSubMenu(); break;
                }
            }
        }

        static void ClientSubMenu()
        {
            Console.WriteLine("\n1.Додати 2.Видалити 3.Змінити Прізвище/Рахунок 4.Перегляд конкретного 5.Список");
            string sub = Console.ReadLine();
            if (sub == "1") {
                Client c = new Client();
                Console.Write("ID: "); c.Id = int.Parse(Console.ReadLine());
                Console.Write("Прізвище: "); c.LastName = Console.ReadLine();
                Console.Write("Ім'я: "); c.FirstName = Console.ReadLine();
                Console.Write("Рахунок: "); c.BankAccountNumber = Console.ReadLine();
                service.AddClient(c);
            }
            else if (sub == "2") {
                Console.Write("Введіть ID для видалення: ");
                int id = int.Parse(Console.ReadLine());
                if (service.DeleteClient(id)) Console.WriteLine("Видалено.");
            }
            else if (sub == "3") {
                Console.Write("ID клієнта: "); int id = int.Parse(Console.ReadLine());
                Console.Write("Нове Прізвище: "); string ln = Console.ReadLine();
                Console.Write("Новий Рахунок: "); string b = Console.ReadLine();
                service.UpdateClient(id, ln, b);
            }
            else if (sub == "4") {
                Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                var cl = service.GetClient(id);
                Console.WriteLine(cl?.ToString() ?? "Не знайдено");
            }
            else if (sub == "5") {
                Console.WriteLine("Сортувати: 1.Ім'я 2.Прізвище 3.Рахунок");
                int m = int.Parse(Console.ReadLine());
                service.GetClientsSorted(m).ForEach(Console.WriteLine);
            }
        }

        static void PropertySubMenu()
        {
            Console.WriteLine("\n1.Додати 2.Видалити 3.Список");
            string sub = Console.ReadLine();
            if (sub == "1") {
                Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                Console.Write("Ціна: "); decimal p = decimal.Parse(Console.ReadLine());
                Console.Write("Вулиця: "); string st = Console.ReadLine();
                Console.Write("Кількість кімнат (0 для ділянки): "); int r = int.Parse(Console.ReadLine());
                service.AddProperty(new Apartment(id, p, "Київ", st, r));
            }
            else if (sub == "2") {
                Console.Write("ID для видалення: ");
                int id = int.Parse(Console.ReadLine());
                if (service.DeleteProperty(id)) Console.WriteLine("Об'єкт видалено.");
            }
            else if (sub == "3") {
                Console.WriteLine("Сортувати: 1.Тип 2.Вартість");
                int m = int.Parse(Console.ReadLine());
                service.GetPropertiesSorted(m).ForEach(x => Console.WriteLine(x.GetDetails()));
            }
        }

        static void OfferSubMenu()
        {
            Console.WriteLine("\n1.Додати об'єкт клієнту 2.Відхилити пропозицію 3.Бажаний об'єкт (Тип+Ціна)");
            string sub = Console.ReadLine();
            if (sub == "1") {
                Console.Write("ID Клієнта: "); int cid = int.Parse(Console.ReadLine());
                Console.Write("ID Нерухомості: "); int pid = int.Parse(Console.ReadLine());
                try { service.CreateOffer(cid, pid); Console.WriteLine("Додано."); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            else if (sub == "2") {
                Console.Write("ID Клієнта: "); int cid = int.Parse(Console.ReadLine());
                Console.Write("ID Нерухомості: "); int pid = int.Parse(Console.ReadLine());
                service.RejectOffer(cid, pid);
                Console.WriteLine("Видалено з пропозицій.");
            }
            else if (sub == "3") {
                Console.Write("Який тип (напр. 2-кімнатна): "); string t = Console.ReadLine();
                Console.Write("Макс ціна: "); decimal pr = decimal.Parse(Console.ReadLine());
                var res = service.CheckAvailability(t, pr);
                res.ForEach(x => Console.WriteLine("Доступно: " + x.GetDetails()));
            }
        }

        static void SearchSubMenu()
{
    Console.WriteLine("\n--- МЕНЮ ПОШУКУ ---");
    Console.WriteLine("4.1 Пошук клієнтів");
    Console.WriteLine("4.2 Пошук нерухомості");
    Console.WriteLine("4.3 Пошук по всім даним");
    Console.WriteLine("4.4 Розширений пошук");
    Console.Write("Вибір: ");
    string sub = Console.ReadLine();
    
    Console.Write("Введіть запит для пошуку: ");
    string k = Console.ReadLine();
    
    switch (sub)
    {
        case "4.1":
            service.SearchClients(k).ForEach(Console.WriteLine);
            break;

        case "4.2":
            service.SearchProperties(k).ForEach(p => Console.WriteLine(p.GetDetails()));
            break;

        case "4.3":
            var allResults = service.SearchEverything(k);
            Console.WriteLine($"\nЗнайдено збігів: {allResults.Count}");
            foreach (var item in allResults)
            {
                if (item is Client c) Console.WriteLine($"[Клієнт] {c}");
                else if (item is RealEstate p) Console.WriteLine($"[Об'єкт] {p.GetDetails()}");
            }
            break;

        case "4.4":
            Console.Write("Введіть тип об'єкта: ");
            string t = Console.ReadLine();
            service.AdvancedSearch(k, t).ForEach(Console.WriteLine);
            break;

        default:
            Console.WriteLine("Невірний пункт підменю.");
            break;
    }
    
    Console.WriteLine("\nНатисніть клавішу для повернення...");
    Console.ReadKey();
}
    }
}