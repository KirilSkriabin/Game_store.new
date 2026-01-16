using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComputerGamesStore
{
    // Лабораторна робота №3 — "Цикли. Багаторазові обчислення та структури даних у C#"
    class Program
    {
        // --- Константи цін (за замовчуванням) ---
        const double DefaultActionPrice = 599.0;
        const double DefaultStrategyPrice = 449.0;
        const double DefaultRpgPrice = 699.0;
        const double DefaultSimulatorPrice = 399.0;

        // Сховище ігор в пам'яті
        static List<Game> games = new List<Game>();

        // Прості дані для входу
        const string correctLogin = "admin";
        const string correctPassword = "12345";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "GAME WORLD - Магазин ігор (Лабораторна №3)";

            // Система входу (do-while) — максимум 3 спроби
            bool loggedIn = LoginSystem();
            if (!loggedIn)
            {
                Console.WriteLine("Надто багато невдалих спроб. Програма завершує роботу.");
                return;
            }

            // Ініціалізуємо кілька тестових товарів (щоб було з чим працювати)
            SeedSampleGames();

            // Головне меню (while)
            ShowMainMenu();
        }

        // ----------------- Система входу -----------------
        static bool LoginSystem()
        {
            int attempts = 0;
            const int maxAttempts = 3;
            string enteredLogin, enteredPassword;

            do
            {
                Console.Clear();
                Console.WriteLine("=== Система входу в GAME WORLD ===");
                Console.Write("Логін: ");
                enteredLogin = Console.ReadLine();
                Console.Write("Пароль: ");
                enteredPassword = ReadPassword();

                if (enteredLogin == correctLogin && enteredPassword == correctPassword)
                {
                    Console.WriteLine("\nВхід успішний! Ласкаво просимо.");
                    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
                    Console.ReadKey();
                    return true;
                }
                else
                {
                    attempts++;
                    Console.WriteLine($"\nНевірні дані. Залишилось спроб: {maxAttempts - attempts}");
                    if (attempts < maxAttempts)
                    {
                        Console.WriteLine("Натисніть будь-яку клавішу щоб спробувати ще раз...");
                        Console.ReadKey();
                    }
                }

            } while (attempts < maxAttempts);

            return false; // перевищено спроби
        }

        // Допоміжна: читає пароль без відображення на екрані
        static string ReadPassword()
        {
            string pwd = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd = pwd.Substring(0, pwd.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pwd += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            return pwd;
        }

        // ----------------- Меню -----------------
        static void ShowMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.WriteLine("           GAME WORLD - Магазин ігор");
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.ResetColor();
                Console.WriteLine();

                Console.WriteLine("1. Додати ігри (ввести через цикл)");
                Console.WriteLine("2. Переглянути асортимент");
                Console.WriteLine("3. Статистика по товарам");
                Console.WriteLine("4. Пошук гри (за назвою)");
                Console.WriteLine("5. Розрахувати покупку (кошик)");
                Console.WriteLine("6. Зберегти інвентар у файл");
                Console.WriteLine("7. Завантажити інвентар з файлу");
                Console.WriteLine("0. Вихід");
                Console.WriteLine();
                Console.Write("Ваш вибір: ");

                string input = Console.ReadLine();
                int choice;
                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("⚠ Помилка: введіть число від 0 до 7.");
                    Pause();
                    continue; // повертаємось до меню
                }

                switch (choice)
                {
                    case 1:
                        AddGamesInteractive();
                        break;
                    case 2:
                        ShowProducts();
                        break;
                    case 3:
                        ShowStatistics();
                        break;
                    case 4:
                        SearchGameByName();
                        break;
                    case 5:
                        CalculatePurchase();
                        break;
                    case 6:
                        SaveInventoryToFile();
                        break;
                    case 7:
                        LoadInventoryFromFile();
                        break;
                    case 0:
                        Console.WriteLine("Вихід із програми...");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("❌ Невірний вибір! Спробуйте ще раз.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися до меню...");
                    Console.ReadKey();
                }
            }
        }

        static void Pause()
        {
            Console.WriteLine("Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        // ----------------- Структура Game -----------------
        enum Genre { Action, Strategy, RPG, Simulator, Other }

        struct Game
        {
            public int Id;            // int
            public string Name;       // string
            public Genre GameGenre;   // enum
            public double Price;      // double
            public int Quantity;      // int

            // Конструктор
            public Game(int id, string name, Genre genre, double price, int quantity)
            {
                Id = id;
                Name = name;
                GameGenre = genre;
                Price = price;
                Quantity = quantity;
            }

            public void DisplayLine()
            {
                Console.WriteLine($"[{Id,2}] {Name,-30} | {GameGenre,-9} | {Price,8:F2} грн | Кільк: {Quantity}");
            }

            public string ToFileString()
            {
                return $"{Id}\t{Name}\t{GameGenre}\t{Price}\t{Quantity}";
            }

            public static bool TryParseFromFile(string line, out Game g)
            {
                g = new Game();
                var parts = line.Split('\t');
                if (parts.Length != 5) return false;
                int id; double price; int qty; Genre genre;
                if (!int.TryParse(parts[0], out id)) return false;
                if (!Enum.TryParse(parts[2], out genre)) return false;
                if (!double.TryParse(parts[3], out price)) return false;
                if (!int.TryParse(parts[4], out qty)) return false;
                g = new Game(id, parts[1], genre, price, qty);
                return true;
            }
        }

        // ----------------- Функції меню -----------------
        static void AddGamesInteractive()
        {
            Console.Clear();
            Console.WriteLine("=== Додавання ігор ===");
            Console.Write("Скільки ігор ви хочете додати (мін 1, макс 20): ");

            int count;
            if (!int.TryParse(Console.ReadLine(), out count) || count < 1 || count > 20)
            {
                Console.WriteLine("⚠ Невірне число. Дозволено від 1 до 20.");
                return;
            }

            // for - введення фіксованої кількості
            int startId = games.Count > 0 ? games.Max(g => g.Id) + 1 : 1;
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\nВведення гри #{i + 1}:");

                Console.Write("Назва: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ім'я не може бути пустим. Пропускаємо цю гру (continue).");
                    continue; // пропускаємо невалідну ітерацію
                }

                Console.Write("Жанр (Action/Strategy/RPG/Simulator/Other): ");
                string genreStr = Console.ReadLine();
                Genre genre;
                if (!Enum.TryParse(genreStr, true, out genre))
                {
                    Console.WriteLine("Невідомий жанр — буде встановлено Other.");
                    genre = Genre.Other;
                }

                Console.Write("Ціна (грн): ");
                double price;
                if (!double.TryParse(Console.ReadLine(), out price) || price <= 0)
                {
                    Console.WriteLine("Невірна ціна — пропускаємо цю гру.");
                    continue; // пропускаємо невалідну ітерацію
                }

                Console.Write("Кількість на складі: ");
                int qty;
                if (!int.TryParse(Console.ReadLine(), out qty) || qty < 0)
                {
                    Console.WriteLine("Невірна кількість — встановлюємо 0.");
                    qty = 0; // дозволяємо, але показуємо приклад використання continue / break
                }

                var game = new Game(startId + i, name.Trim(), genre, price, qty);
                games.Add(game);
                Console.WriteLine("Гра додана.");
            }
        }

        static void ShowProducts()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("МЕНЮ КОМП'ЮТЕРНИХ ІГОР:");
            Console.ResetColor();

            if (games.Count == 0)
            {
                Console.WriteLine("Поки що ігор немає. Додайте їх через пункт 1.");
                return;
            }

            Console.WriteLine("[ID] Назва                          | Жанр      |    Ціна | Кільк");
            Console.WriteLine(new string('─', 70));

            foreach (var g in games)
                g.DisplayLine();
        }

        static void ShowStatistics()
        {
            Console.Clear();
            Console.WriteLine("=== Статистика інвентарю ===");

            if (games.Count == 0)
            {
                Console.WriteLine("Немає даних для статистики.");
                return;
            }

            // Загальна сума (цін * кількість)
            double totalSum = 0;
            double sumPrices = 0;
            int countWithPrice = 0;
            int expensiveCount = 0; // > 500 грн

            double minPrice = double.MaxValue;
            double maxPrice = double.MinValue;
            string cheapest = "-";
            string mostExpensive = "-";

            // Приклад використання циклу for
            for (int i = 0; i < games.Count; i++)
            {
                var g = games[i];

                // Пропуск невалідної ціни
                if (g.Price <= 0) continue;

                totalSum += g.Price * g.Quantity;
                sumPrices += g.Price;
                countWithPrice++;

                if (g.Price > 500) expensiveCount++;

                if (g.Price < minPrice)
                {
                    minPrice = g.Price;
                    cheapest = g.Name;
                }
                if (g.Price > maxPrice)
                {
                    maxPrice = g.Price;
                    mostExpensive = g.Name;
                }
            }

            double averagePrice = countWithPrice > 0 ? sumPrices / countWithPrice : 0;

            Console.WriteLine($"Загальна сума (враховуючи кількість): {totalSum:F2} грн");
            Console.WriteLine($"Середня ціна товару: {averagePrice:F2} грн");
            Console.WriteLine($"Кількість товарів дорожчих за 500 грн: {expensiveCount}");
            if (countWithPrice > 0)
            {
                Console.WriteLine($"Найдешевша гра: {cheapest} ({minPrice:F2} грн)");
                Console.WriteLine($"Найдорожча гра: {mostExpensive} ({maxPrice:F2} грн)");
            }
            else
            {
                Console.WriteLine("Немає коректних цін для мін/макс.");
            }

            // Форматований звіт (функція для звіту)
            Console.WriteLine("\n=== Звіт: таблиця інвентарю ===");
            Console.WriteLine("[ID] Назва                          | Жанр      |    Ціна | Кільк");
            Console.WriteLine(new string('─', 70));
            foreach (var g in games)
                g.DisplayLine();

            Console.WriteLine("\n(Підсумки вгорі — перевірте, чи вірно враховано кількості та ціни.)");
        }

        static void SearchGameByName()
        {
            Console.Clear();
            Console.WriteLine("=== Пошук гри за назвою ===");
            Console.Write("Введіть частину або повну назву: ");
            string query = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Порожній запит.");
                return;
            }

            bool found = false;
            // Приклад використання foreach + break
            foreach (var g in games)
            {
                if (g.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine("Знайдено:");
                    g.DisplayLine();
                    found = true;
                    // Якщо потрібно зупинити пошук після першої знайденої — використовуємо break
                    // break; // <- можна розкоментувати, якщо хочемо тільки перший результат
                }
            }

            if (!found)
                Console.WriteLine("За заданим запитом нічого не знайдено.");
        }

        static void CalculatePurchase()
        {
            Console.Clear();
            Console.WriteLine("=== РОЗРАХУНОК ПОКУПКИ ===");

            if (games.Count == 0)
            {
                Console.WriteLine("Немає товарів у магазині — спочатку додайте ігри.");
                return;
            }

            // Користувач обирає кілька ігор по ID і вказує кількість
            Dictionary<int, int> cart = new Dictionary<int, int>();

            while (true)
            {
                Console.Write("Введіть ID гри для додавання в кошик (0 - завершити): ");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id))
                {
                    Console.WriteLine("Невірний ID — спробуйте ще раз.");
                    continue; // пропускаємо поточну ітерацію
                }
                if (id == 0) break;

                var game = games.FirstOrDefault(g => g.Id == id);
                if (game.Name == null)
                {
                    Console.WriteLine("Гра з таким ID не знайдена.");
                    continue;
                }

                Console.Write($"Вкажіть кількість для '{game.Name}': ");
                int qty;
                if (!int.TryParse(Console.ReadLine(), out qty) || qty <= 0)
                {
                    Console.WriteLine("Невірна кількість. Пропускаємо.");
                    continue;
                }

                if (cart.ContainsKey(id)) cart[id] += qty;
                else cart[id] = qty;

                Console.WriteLine("Додано до кошика.");
            }

            if (cart.Count == 0)
            {
                Console.WriteLine("Кошик порожній.");
                return;
            }

            double total = 0;
            Console.WriteLine("\n=== Вміст кошика ===");
            foreach (var kvp in cart)
            {
                var g = games.First(x => x.Id == kvp.Key);
                int q = kvp.Value;
                double line = g.Price * q;
                Console.WriteLine($"{g.Name} x{q} = {line:F2} грн");
                total += line;
            }

            // Знижка випадкова між 5 та 25% (демонстрація Math та Random)
            double discountPercent = new Random().Next(5, 26);
            double discountAmount = Math.Round(total * discountPercent / 100.0, 2);
            double toPay = Math.Round(total - discountAmount, 2);

            Console.WriteLine("\n════════════════════════════════════");
            Console.WriteLine($"Загальна сума: {total:F2} грн");
            Console.WriteLine($"Знижка: {discountPercent}% (-{discountAmount:F2} грн)");
            Console.WriteLine($"До сплати: {toPay:F2} грн");
            Console.WriteLine("════════════════════════════════════");
        }

        static void SaveInventoryToFile()
        {
            Console.Clear();
            Console.WriteLine("=== Збереження інвентарю у файл ===");
            if (games.Count == 0)
            {
                Console.WriteLine("Немає даних для збереження.");
                return;
            }

            string path = "games_inventory.txt";
            try
            {
                var lines = games.Select(g => g.ToFileString()).ToArray();
                File.WriteAllLines(path, lines);
                Console.WriteLine($"Інвентар збережено у файл: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні: {ex.Message}");
            }
        }

        static void LoadInventoryFromFile()
        {
            Console.Clear();
            Console.WriteLine("=== Завантаження інвентарю з файлу ===");
            string path = "games_inventory.txt";
            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не знайдено: games_inventory.txt");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(path);
                var loaded = new List<Game>();
                foreach (var line in lines)
                {
                    Game g;
                    if (Game.TryParseFromFile(line, out g))
                        loaded.Add(g);
                    else
                        Console.WriteLine($"Пропущено невірний рядок: {line}");
                }

                if (loaded.Count > 0)
                {
                    games = loaded;
                    Console.WriteLine($"Завантажено {games.Count} ігор з файлу.");
                }
                else
                {
                    Console.WriteLine("Файл не містив коректних записів.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні: {ex.Message}");
            }
        }

        static void SeedSampleGames()
        {
            if (games.Count > 0) return; // не дублюємо

            games.Add(new Game(1, "Apex Strike", Genre.Action, DefaultActionPrice, 10));
            games.Add(new Game(2, "Empire Tactics", Genre.Strategy, DefaultStrategyPrice, 5));
            games.Add(new Game(3, "Legends of Arcanum", Genre.RPG, DefaultRpgPrice, 8));
            games.Add(new Game(4, "City Simulator X", Genre.Simulator, DefaultSimulatorPrice, 3));
            games.Add(new Game(5, "Indie Puzzle", Genre.Other, 199.0, 12));
        }
    }
}
