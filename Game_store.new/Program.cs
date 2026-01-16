using System;

namespace ComputerGamesStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Ціни на ігри
            const double actionGamePrice = 599.0;
            const double strategyGamePrice = 449.0;
            const double rpgGamePrice = 699.0;
            const double simulatorGamePrice = 399.0;
             
            // Зміна кольору заголовка
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("           GAME WORLD - Магазин ігор");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();

            // Вивід меню ігор з кольорами
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" МЕНЮ КОМП'ЮТЕРНИХ ІГОР:");
            Console.ResetColor();

            Console.WriteLine($"1. Екшн-гра (Action) - {actionGamePrice} грн");
            Console.WriteLine($"2. Стратегія (Strategy) - {strategyGamePrice} грн");
            Console.WriteLine($"3. RPG гра - {rpgGamePrice} грн");
            Console.WriteLine($"4. Симулятор - {simulatorGamePrice} грн");
            Console.WriteLine();

            // Введення кількості ігор
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Введіть кількість для кожної категорії ігор:");
            Console.ResetColor();
            Console.WriteLine();

            Console.Write("Екшн-ігри: ");
            int actionQuantity = Convert.ToInt32(Console.ReadLine());

            Console.Write("Стратегії: ");
            int strategyQuantity = Convert.ToInt32(Console.ReadLine());

            Console.Write("RPG ігри: ");
            int rpgQuantity = Convert.ToInt32(Console.ReadLine());

            Console.Write("Симулятори: ");
            int simulatorQuantity = Convert.ToInt32(Console.ReadLine());

            // Розрахунок сум
            double actionTotal = actionGamePrice * actionQuantity;
            double strategyTotal = strategyGamePrice * strategyQuantity;
            double rpgTotal = rpgGamePrice * rpgQuantity;
            double simulatorTotal = simulatorGamePrice * simulatorQuantity;

            double totalAmount = actionTotal + strategyTotal + rpgTotal + simulatorTotal;

            // Використання Math для розрахунків
            double averagePrice = totalAmount / (actionQuantity + strategyQuantity + rpgQuantity + simulatorQuantity);
            averagePrice = Math.Round(averagePrice, 2); // Округлення до 2 знаків

            // Генерація випадкової знижки (5-25%)
            Random random = new Random();
            double discountPercent = random.Next(5, 26);
            double discountAmount = totalAmount * (discountPercent / 100.0);
            discountAmount = Math.Round(discountAmount, 2);

            // Розрахунок бонусних балів (квадратний корінь від суми)
            double bonusPoints = Math.Sqrt(totalAmount);
            bonusPoints = Math.Round(bonusPoints, 1);

            // Розрахунок фінальної суми
            double amountToPay = totalAmount - discountAmount;
            amountToPay = Math.Round(amountToPay, 2);

            // Вивід чека
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("                   ЧЕК");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.ResetColor();

            // Деталі замовлення
            if (actionQuantity > 0)
                Console.WriteLine($" Екшн-ігри ({actionQuantity} шт.) - {actionTotal} грн");

            if (strategyQuantity > 0)
                Console.WriteLine($" Стратегії ({strategyQuantity} шт.) - {strategyTotal} грн");

            if (rpgQuantity > 0)
                Console.WriteLine($" RPG ігри ({rpgQuantity} шт.) - {rpgTotal} грн");

            if (simulatorQuantity > 0)
                Console.WriteLine($" Симулятори ({simulatorQuantity} шт.) - {simulatorTotal} грн");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" Загальна сума: {totalAmount} грн");
            Console.WriteLine($" Середня ціна за гру: {averagePrice} грн");
            Console.ResetColor();
            Console.WriteLine();

            // Інформація про знижку та бонуси
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" Вам нараховано знижку: {discountPercent}%");
            Console.WriteLine($" Сума знижки: {discountAmount} грн");
            Console.WriteLine($" Бонусні бали: {bonusPoints}");
            Console.ResetColor();
            Console.WriteLine();

            // Фінальна сума
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine($" ДО СПЛАТИ: {amountToPay} грн");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();

            // Додаткова статистика
            int totalGames = actionQuantity + strategyQuantity + rpgQuantity + simulatorQuantity;
            Console.WriteLine($" Загальна кількість ігор: {totalGames} шт.");

            // Використання Math.Pow для розрахунку потенційної економії
            double potentialSavings = Math.Pow(totalGames, 2) * 0.1;
            potentialSavings = Math.Round(potentialSavings, 2);
            Console.WriteLine($" Потенційна економія при покупці набору: {potentialSavings} грн");

            // Персоналізоване повідомлення
            Console.WriteLine();
            if (totalAmount > 2000)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" Вітаємо з відмінним вибором! Ви справжній геймер!");
            }
            else if (totalAmount > 1000)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(" Чудовий вибір! Гарного геймінгу!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Дякуємо за покупку! Заходьте ще!");
            }
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}