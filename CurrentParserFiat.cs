using HtmlAgilityPack;
using System.Text; // Потрібно для StringBuilder

namespace TelegramBotTest 
{
    public static class CurrencyParser
    {
        public static async Task<string> GetExchangeRates()
        {
            try
            {
                string url = "https://minfin.com.ua/ua/currency/banks/";

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = await web.LoadFromWebAsync(url);

                // StringBuilder дозволяє збирати текст по шматочках
                StringBuilder resultMessage = new StringBuilder();

                // --- 1. Шукаємо ДОЛАР ---
                var usdRow = doc.DocumentNode.SelectSingleNode("//tr[contains(., 'USD')]");
                if (usdRow != null)
                {
                    var cells = usdRow.SelectNodes("td");
                    if (cells != null && cells.Count >= 3)
                    {
                        string buy = cells[1].InnerText.Trim().Replace("\n", "");
                        string sell = cells[2].InnerText.Trim().Replace("\n", "");

                        resultMessage.AppendLine("🇺🇸 **Долар (USD):**");
                        resultMessage.AppendLine($"🔽 Купівля: {buy} грн");
                        resultMessage.AppendLine($"🔼 Продаж: {sell} грн");
                        resultMessage.AppendLine(); // Пустий рядок для відступу
                    }
                }

                // --- 2. Шукаємо ЄВРО ---
                var eurRow = doc.DocumentNode.SelectSingleNode("//tr[contains(., 'EUR')]");
                if (eurRow != null)
                {
                    var cells = eurRow.SelectNodes("td");
                    if (cells != null && cells.Count >= 3)
                    {
                        string buy = cells[1].InnerText.Trim().Replace("\n", "");
                        string sell = cells[2].InnerText.Trim().Replace("\n", "");

                        resultMessage.AppendLine("🇪🇺 **Євро (EUR):**");
                        resultMessage.AppendLine($"🔽 Купівля: {buy} грн");
                        resultMessage.AppendLine($"🔼 Продаж: {sell} грн");
                    }
                }

                // Перевірка, чи знайшли хоч щось
                if (resultMessage.Length == 0)
                {
                    return "❌ Не вдалося знайти курси на сайті.";
                }

                resultMessage.AppendLine($"\n_Джерело: {url}_");

                // Перетворюємо зібраний текст у рядок і віддаємо
                return resultMessage.ToString();
            }
            catch (Exception ex)
            {
                return $"⚠️ Помилка парсера: {ex.Message}";
            }
        }
    }
}