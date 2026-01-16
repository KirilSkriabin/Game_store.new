using System;

namespace ComputerGamesStore
{
    class Program
    {
        // --- Константи цін ---
        const double actionGamePrice = 599.0;
        const double strategyGamePrice = 449.0;
        const double rpgGamePrice = 699.0;
        const double simulatorGamePrice = 399.0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ShowMainMenu(); // запуск головного меню
        }

        // --- ГОЛОВНЕ МЕНЮ ---
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("           GAME WORLD - Магазин ігор");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("1. Переглянути асортимент");
            Console.WriteLine("2. Розрахувати покупку");
            Console.WriteLine("3. Інформація про магазин");
            Console.WriteLine("4. Налаштування");
            Console.WriteLine("0. Вихід");
            Console.WriteLine();

            Console.Write("Ваш вибір: ");
            string input = Console.ReadLine();

            try
            {
                int choice = int.Parse(input);
                switch (choice)
                {
                    case 1:
                        ShowProducts();
                        break;
                    case 2:
                        CalculatePurchase();
                        break;
                    case 3:
                        ShowInfo();
                        break;
                    case 4:
                        Settings();
                        break;
                    case 0:
                        Console.WriteLine("Вихід із програми...");
                        return;
                    default:
                        Console.WriteLine("❌ Невірний вибір! Спробуйте ще раз.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("⚠ Помилка! Введіть число від 0 до 4.");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися до меню...");
            Console.ReadKey();
            ShowMainMenu(); // рекурсивне повернення до меню
        }

        // --- ФУНКЦІЇ МЕНЮ ---

        static void ShowProducts()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("МЕНЮ КОМП'ЮТЕРНИХ ІГОР:");
            Console.ResetColor();

            Console.WriteLine($"1. Екшн-гра (Action) - {actionGamePrice} грн");
            Console.WriteLine($"2. Стратегія (Strategy) - {strategyGamePrice} грн");
            Console.WriteLine($"3. RPG гра - {rpgGamePrice} грн");
            Console.WriteLine($"4. Симулятор - {simulatorGamePrice} грн");

        }

        static void CalculatePurchase()
        {
            Console.Clear();
            Console.WriteLine("=== РОЗРАХУНОК ПОКУПКИ ===");

            try
            {
                Console.Write("Екшн-ігри: ");
                int actionQuantity = Convert.ToInt32(Console.ReadLine());

                Console.Write("Стратегії: ");
                int strategyQuantity = Convert.ToInt32(Console.ReadLine());

                Console.Write("RPG ігри: ");
                int rpgQuantity = Convert.ToInt32(Console.ReadLine());

                Console.Write("Симулятори: ");
                int simulatorQuantity = Convert.ToInt32(Console.ReadLine());

                double total = actionGamePrice * actionQuantity
                             + strategyGamePrice * strategyQuantity
                             + rpgGamePrice * rpgQuantity
                             + simulatorGamePrice * simulatorQuantity;

                double discount = new Random().Next(5, 26);
                double discountAmount = Math.Round(total * discount / 100.0, 2);
                double amountToPay = Math.Round(total - discountAmount, 2);

                Console.WriteLine("\n════════════════════════════════════");
                Console.WriteLine($"Загальна сума: {total} грн");
                Console.WriteLine($"Знижка: {discount}% (-{discountAmount} грн)");
                Console.WriteLine($"До сплати: {amountToPay} грн");
                Console.WriteLine("════════════════════════════════════");
            }
            catch (FormatException)
            {
                Console.WriteLine("⚠ Помилка: введено нечислове значення!");
            }
        }

        static void ShowInfo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ІНФОРМАЦІЯ ПРО МАГАЗИН");
            Console.ResetColor();

            Console.WriteLine("Назва: GAME WORLD");
            Console.WriteLine("Адреса: м. Київ, вул. Ігрова, 5");
            Console.WriteLine("Телефон: +38 (044) 123-45-67");
            Console.WriteLine("Години роботи: 10:00 - 20:00");

        }

        static void Settings()
        {
            Console.Clear();
            Console.WriteLine("=== НАЛАШТУВАННЯ ===");
            Console.WriteLine("Функція в розробці...");
        }
    }
}
