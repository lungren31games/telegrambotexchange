using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotTest // Простір імен той самий, що і в парсера
{
    //Program.cs - це головний файл, який запускає бота та обробляє повідомлення від користувачів. Він використовує Telegram.Bot для взаємодії з Telegram API та викликає методи з CurrencyParser для отримання курсу валют.
    class Program
    {
        // 👇 Встав сюди свій токен
        private static readonly string BotToken = BotConfiguration.Token;
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient(BotToken);
            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMe();
            Console.WriteLine($"✅ Бот {me.Username} працює!");
            Console.ReadLine();
            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message && message.Text is { } messageText)
            {
                var chatId = message.Chat.Id;

                if (messageText == "/start")
                {
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("💰 Курс валют")
                    })
                    { ResizeKeyboard = true };

                    await botClient.SendMessage(
                        chatId,
                        "Привіт! Натисни кнопку, щоб отримати курс Долара та Євро.",
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken);
                }
                else if (messageText == "💰 Курс валют")
                {
                    await botClient.SendMessage(chatId, "🔍 Дивлюсь актуальний курс...", cancellationToken: cancellationToken);

                    // Викликаємо метод з іншого файлу
                    string rates = await CurrencyParser.GetExchangeRates();

                    await botClient.SendMessage(
                        chatId,
                        rates,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: cancellationToken);
                }
            }
        }

        static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}