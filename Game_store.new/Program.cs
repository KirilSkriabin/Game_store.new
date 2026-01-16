using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace ComputerGamesStore
{
    class Program
    {
        const string GamesFile = "games.csv";
        const string UsersFile = "users.csv";

        //  РОЛІ 
        const string AdminEmail = "admin"; // адмінський акаунт 
        static bool isAdmin = false;
        static string currentUserEmail = "";

        //  ФІКСОВАНІ ЖАНРИ 
        enum Genre
        {
            Action = 1,
            Strategy,
            RPG,
            Simulator,
            Roleplay,
            Adventure,
            Sports,
            Horror,
            Indie,
            Other
        }

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            EnsureFiles();

            if (!AuthMenu())
                return;

            MainMenu();
        }

        //  FILE INIT 
        static void EnsureFiles()
        {
            if (!File.Exists(GamesFile))
                File.WriteAllText(GamesFile, "Id,Name,Genre,Price,Quantity\n");

            if (!File.Exists(UsersFile))
                File.WriteAllText(UsersFile, "Id,Email,PasswordHash\n");
        }

        //  AUTH 
        static bool AuthMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Вхід");
                Console.WriteLine("2. Реєстрація");
                Console.WriteLine("0. Вихід");
                Console.Write("Вибір: ");

                switch (Console.ReadLine())
                {
                    case "1": return Login();
                    case "2": Register(); break;
                    case "0": return false;
                }
            }
        }

        static bool Login()
        {
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Пароль: ");
            string pass = ReadPassword();
            string hash = Hash(pass);

            foreach (var line in File.ReadAllLines(UsersFile).Skip(1))
            {
                var p = line.Split(',');
                if (p.Length != 3) continue;

                if (p[1] == email && p[2] == hash)
                {
                    currentUserEmail = email;
                    isAdmin = string.Equals(email, AdminEmail, StringComparison.OrdinalIgnoreCase);
                    return true;
                }
            }

            Console.WriteLine("❌ Невірні дані");
            Console.ReadKey();
            return false;
        }

        static void Register()
        {
            Console.Write("Email: ");
            string email = Console.ReadLine();

            if (File.ReadAllLines(UsersFile).Any(l => l.Split(',').Length == 3 && l.Split(',')[1] == email))
            {
                Console.WriteLine("❌ Email вже існує");
                Console.ReadKey();
                return;
            }

            Console.Write("Пароль: ");
            string pass = ReadPassword();
            int id = GenerateId(UsersFile);

            File.AppendAllText(UsersFile, $"{id},{email},{Hash(pass)}\n");
            Console.WriteLine("✅ Реєстрація успішна");
            Console.ReadKey();
        }

        //  MAIN MENU 
        static void MainMenu()
        {
            while (true)
            {
                Console.Clear();

                // Показуємо хто зайшов
                Console.WriteLine(isAdmin
                    ? $"Увійшли як: {currentUserEmail} (ADMIN)"
                    : $"Увійшли як: {currentUserEmail} (BUYER)");
                Console.WriteLine();

                if (isAdmin)
                {
                    Console.WriteLine("1. Додати гру");
                    Console.WriteLine("2. Показати ігри");
                    Console.WriteLine("3. Видалити гру");
                    Console.WriteLine("4. Статистика");
                    Console.WriteLine("5. Купити ігри (кошик з рандомною знижкою)");
                    Console.WriteLine("6. Редагувати гру");
                    Console.WriteLine("7. Фільтрація ігор за жанром");
                    Console.WriteLine("0. Вихід");
                }
                else
                {
                    // БАЗОВЕ МЕНЮ ПОКУПЦЯ
                    Console.WriteLine("2. Показати ігри");
                    Console.WriteLine("4. Статистика");
                    Console.WriteLine("5. Купити ігри (кошик з рандомною знижкою)");
                    Console.WriteLine("7. Фільтрація ігор за жанром");
                    Console.WriteLine("0. Вихід");
                }

                Console.Write("\nВибір: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    // Доступно всім
                    case "2": ShowGames(); break;
                    case "4": Stats(); break;
                    case "5": BuyGames(); break;
                    case "7": FilterGamesByGenre(); break;
                    case "0": return;

                    // Тільки адміну
                    case "1":
                        if (!RequireAdmin()) break;
                        AddGame();
                        break;

                    case "3":
                        if (!RequireAdmin()) break;
                        DeleteGame();
                        break;

                    case "6":
                        if (!RequireAdmin()) break;
                        EditGame();
                        break;

                    default:
                        Console.WriteLine("❌ Невірний вибір або немає доступу.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        //  ПЕРЕВІРКА ПРАВ АДМІНА 
        static bool RequireAdmin()
        {
            if (isAdmin) return true;

            Console.WriteLine("⛔ Доступ заборонено. Ця функція доступна лише адміністратору.");
            Console.ReadKey();
            return false;
        }

        //  GAMES 
        static void AddGame()
        {
            Console.Clear();
            Console.WriteLine("=== ДОДАТИ ГРУ ===");

            Console.Write("Назва: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Назва не може бути порожньою.");
                Console.ReadKey();
                return;
            }

            Genre genre = ChooseGenre();

            Console.Write("Ціна: ");
            if (!double.TryParse(Console.ReadLine(), out double price) || price <= 0)
            {
                Console.WriteLine("❌ Невірна ціна.");
                Console.ReadKey();
                return;
            }

            Console.Write("Кількість: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 0)
            {
                Console.WriteLine("❌ Невірна кількість.");
                Console.ReadKey();
                return;
            }

            int id = GenerateId(GamesFile);
            File.AppendAllText(GamesFile, $"{id},{EscapeCsv(name.Trim())},{genre},{price},{qty}\n");

            Console.WriteLine("✅ Гру додано!");
            Console.ReadKey();
        }

        static void ShowGames()
        {
            Console.Clear();

            Console.WriteLine("ID | Назва                | Жанр       |   Ціна | Кількість");
            Console.WriteLine(new string('-', 60));

            foreach (var line in File.ReadAllLines(GamesFile).Skip(1))
            {
                var p = line.Split(',');
                if (p.Length != 5) continue;

                Console.WriteLine($"{p[0],2} | {p[1],-20} | {p[2],-10} | {p[3],6} | {p[4],8}");
            }

            Console.ReadKey();
        }

        static void DeleteGame()
        {
            Console.Clear();
            Console.WriteLine("=== ВИДАЛЕННЯ ГРИ ===");
            Console.Write("ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Невірний ID.");
                Console.ReadKey();
                return;
            }

            var lines = File.ReadAllLines(GamesFile)
                .Where(l => !(l.StartsWith(id + ",")))
                .ToArray();

            File.WriteAllLines(GamesFile, lines);

            Console.WriteLine("✅ Якщо ID існував — гру видалено.");
            Console.ReadKey();
        }

        static void Stats()
        {
            Console.Clear();
            Console.WriteLine("=== СТАТИСТИКА ===");

            var data = File.ReadAllLines(GamesFile).Skip(1)
                .Select(l => l.Split(','))
                .Where(p => p.Length == 5 && double.TryParse(p[3], out _) && int.TryParse(p[4], out _))
                .ToList();

            if (data.Count == 0)
            {
                Console.WriteLine("Немає даних для статистики.");
                Console.ReadKey();
                return;
            }

            double min = data.Min(p => double.Parse(p[3]));
            double max = data.Max(p => double.Parse(p[3]));
            double avg = data.Average(p => double.Parse(p[3]));
            double sum = data.Sum(p => double.Parse(p[3]) * int.Parse(p[4]));
            int count = data.Count;

            Console.WriteLine($"Кількість ігор: {count}");
            Console.WriteLine($"Мінімальна вартість: {min}");
            Console.WriteLine($"Максимальна вартість: {max}");
            Console.WriteLine($"Середня вартість: {avg}");
            Console.WriteLine($"Сума на складі: {sum}");
            Console.ReadKey();
        }

        //  ФІЛЬТРАЦІЯ ЗА ЖАНРОМ 
        static void FilterGamesByGenre()
        {
            Console.Clear();
            Console.WriteLine("=== ФІЛЬТРАЦІЯ ІГОР ЗА ЖАНРОМ ===");

            Genre genre = ChooseGenre();
            string genreStr = genre.ToString();

            var rows = File.ReadAllLines(GamesFile).Skip(1)
                .Select(l => l.Split(','))
                .Where(p => p.Length == 5 && string.Equals(p[2], genreStr, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.Clear();
            Console.WriteLine($"=== Жанр: {genreStr} ===");
            Console.WriteLine("ID | Назва                | Жанр       |   Ціна | Кількість");
            Console.WriteLine(new string('-', 60));

            if (rows.Count == 0)
            {
                Console.WriteLine("Нічого не знайдено.");
                Console.ReadKey();
                return;
            }

            foreach (var p in rows)
                Console.WriteLine($"{p[0],2} | {p[1],-20} | {p[2],-10} | {p[3],6} | {p[4],8}");

            Console.ReadKey();
        }

        //  Кошик з рандомною знижкою 
        static void BuyGames()
        {
            Console.Clear();
            Console.WriteLine("=== КУПІВЛЯ ІГОР ===");

            var gamesList = File.ReadAllLines(GamesFile).Skip(1)
                .Select(l => l.Split(','))
                .Where(p => p.Length == 5)
                .Select(p => new
                {
                    Id = int.TryParse(p[0], out var id) ? id : -1,
                    Name = p[1],
                    Genre = p[2],
                    Price = double.TryParse(p[3], out var pr) ? pr : 0,
                    Quantity = int.TryParse(p[4], out var qt) ? qt : 0
                })
                .Where(g => g.Id > 0)
                .ToList();

            if (gamesList.Count == 0)
            {
                Console.WriteLine("Магазин порожній. Додайте ігри через меню.");
                Console.ReadKey();
                return;
            }

            Dictionary<int, int> cart = new Dictionary<int, int>();

            while (true)
            {
                Console.Write("Введіть ID гри для покупки (0 - завершити): ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Невірний ID. Спробуйте ще раз.");
                    continue;
                }
                if (id == 0) break;

                var game = gamesList.FirstOrDefault(g => g.Id == id);
                if (game == null)
                {
                    Console.WriteLine("Гра з таким ID не знайдена.");
                    continue;
                }

                Console.Write($"Вкажіть кількість для '{game.Name}': ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                {
                    Console.WriteLine("Невірна кількість. Пропускаємо.");
                    continue;
                }

                int alreadyInCart = cart.ContainsKey(id) ? cart[id] : 0;
                if (qty + alreadyInCart > game.Quantity)
                {
                    int available = game.Quantity - alreadyInCart;
                    Console.WriteLine($"На складі лишилось {available} шт для цієї гри (з урахуванням кошика).");
                    continue;
                }

                if (cart.ContainsKey(id)) cart[id] += qty;
                else cart[id] = qty;

                Console.WriteLine("Додано до кошика.");
            }

            if (cart.Count == 0)
            {
                Console.WriteLine("Кошик порожній.");
                Console.ReadKey();
                return;
            }

            double total = 0;
            Console.WriteLine("\n=== Вміст кошика ===");
            foreach (var kvp in cart)
            {
                var g = gamesList.First(x => x.Id == kvp.Key);
                int q = kvp.Value;
                double line = g.Price * q;
                Console.WriteLine($"{g.Name} x{q} = {line:F2} грн");
                total += line;
            }

            if (cart.Count >= 3)
            {
                double discountPercent = new Random().Next(5, 26);
                double discountAmount = Math.Round(total * discountPercent / 100.0, 2);
                total = Math.Round(total - discountAmount, 2);
                Console.WriteLine($"\n🎉 Ви отримали знижку {discountPercent}% (-{discountAmount:F2} грн)!");
            }

            Console.WriteLine($"\nЗагальна сума до сплати: {total:F2} грн");
            Console.WriteLine("Підтвердити покупку? (1 - так, інше - ні): ");
            string confirm = Console.ReadLine();

            if (confirm == "1")
            {
                UpdateStockAfterPurchase(cart);
                Console.WriteLine("✅ Покупка успішна! Кількість на складі оновлено.");
            }
            else
            {
                Console.WriteLine("Покупку скасовано. На складі нічого не змінено.");
            }

            Console.ReadKey();
        }

        static void UpdateStockAfterPurchase(Dictionary<int, int> cart)
        {
            var lines = File.ReadAllLines(GamesFile).ToList();
            if (lines.Count == 0) return;

            for (int i = 1; i < lines.Count; i++)
            {
                var p = lines[i].Split(',');
                if (p.Length != 5) continue;

                if (!int.TryParse(p[0], out int id)) continue;
                if (!int.TryParse(p[4], out int qtyInStock)) continue;

                if (cart.ContainsKey(id))
                {
                    int bought = cart[id];
                    int newQty = qtyInStock - bought;
                    if (newQty < 0) newQty = 0;

                    lines[i] = $"{p[0]},{p[1]},{p[2]},{p[3]},{newQty}";
                }
            }

            File.WriteAllLines(GamesFile, lines);
        }

        //  Редагування гри 
        static void EditGame()
        {
            Console.Clear();
            Console.WriteLine("=== Редагування гри ===");

            PrintGamesTableHeader();
            foreach (var line in File.ReadAllLines(GamesFile).Skip(1))
            {
                var p = line.Split(',');
                if (p.Length != 5) continue;
                Console.WriteLine($"{p[0],2} | {p[1],-20} | {p[2],-10} | {p[3],6} | {p[4],8}");
            }

            Console.Write("\nВведіть ID гри, яку хочете редагувати: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний ID");
                Console.ReadKey();
                return;
            }

            var lines = File.ReadAllLines(GamesFile).ToList();
            int index = lines.FindIndex(l => l.StartsWith(id + ","));
            if (index == -1)
            {
                Console.WriteLine("Гра з таким ID не знайдена");
                Console.ReadKey();
                return;
            }

            var parts = lines[index].Split(',');
            if (parts.Length != 5)
            {
                Console.WriteLine("Некоректний рядок у файлі");
                Console.ReadKey();
                return;
            }

            string oldName = parts[1];
            string oldGenre = parts[2];
            string oldPrice = parts[3];
            string oldQty = parts[4];

            Console.WriteLine("Залиште поле порожнім, якщо не хочете його змінювати.");

            Console.Write($"Назва [{oldName}]: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = oldName;
            name = EscapeCsv(name.Trim());

            Console.WriteLine($"Жанр зараз: {oldGenre}");
            Console.Write("Змінити жанр? (1 - так, інше - ні): ");
            string changeGenre = Console.ReadLine();
            string genreStr = oldGenre;
            if (changeGenre == "1")
            {
                Genre newGenre = ChooseGenre();
                genreStr = newGenre.ToString();
            }

            Console.Write($"Ціна [{oldPrice}]: ");
            string priceInput = Console.ReadLine();
            double price;
            if (string.IsNullOrWhiteSpace(priceInput)) price = double.Parse(oldPrice);
            else if (!double.TryParse(priceInput, out price) || price <= 0)
            {
                Console.WriteLine("Невірна ціна. Використовується стара.");
                price = double.Parse(oldPrice);
            }

            Console.Write($"Кількість [{oldQty}]: ");
            string qtyInput = Console.ReadLine();
            int qty;
            if (string.IsNullOrWhiteSpace(qtyInput)) qty = int.Parse(oldQty);
            else if (!int.TryParse(qtyInput, out qty) || qty < 0)
            {
                Console.WriteLine("Невірна кількість. Використовується стара.");
                qty = int.Parse(oldQty);
            }

            lines[index] = $"{id},{name},{genreStr},{price},{qty}";
            File.WriteAllLines(GamesFile, lines);

            Console.WriteLine("✅ Гру успішно відредаговано!");
            Console.ReadKey();
        }

        //  HELPERS 
        static int GenerateId(string path)
        {
            int max = 0;
            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var p = line.Split(',');
                if (p.Length > 0 && int.TryParse(p[0], out int id))
                    if (id > max) max = id;
            }
            return max + 1;
        }

        static string Hash(string s)
        {
            using (var sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLower();
            }
        }

        static string ReadPassword()
        {
            string p = "";
            ConsoleKeyInfo k;
            while ((k = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (k.Key == ConsoleKey.Backspace && p.Length > 0)
                {
                    p = p.Substring(0, p.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(k.KeyChar))
                {
                    p += k.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return p;
        }

        //  ДРУК ШАПКИ ТАБЛИЦІ 
        static void PrintGamesTableHeader()
        {
            Console.WriteLine("ID | Назва                | Жанр       |   Ціна | Кількість");
            Console.WriteLine(new string('-', 60));
        }

        //  ВИБІР ЖАНРУ (ОБОВʼЯЗКОВО ЗІ СПИСКУ) 
        static Genre ChooseGenre()
        {
            while (true)
            {
                Console.WriteLine("\nОберіть жанр:");
                var values = (Genre[])Enum.GetValues(typeof(Genre));
                foreach (var g in values)
                    Console.WriteLine($"{(int)g}. {g}");

                Console.Write("Введіть номер жанру: ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    Enum.IsDefined(typeof(Genre), choice))
                {
                    return (Genre)choice;
                }

                Console.WriteLine("❌ Невірний вибір жанру. Спробуйте ще раз.");
            }
        }

        static string EscapeCsv(string s)
        {
            if (s == null) return "";
            return s.Replace(",", " ");
        }
    }
}
