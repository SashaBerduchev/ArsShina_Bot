﻿using ArsShina_Bot.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace ArsShina_Bot
{
    class Program
    {
        
        static ITelegramBotClient bot = new TelegramBotClient("5520950526:AAFnQh_ejXv-SztIkbnTihawdwARWhwJ_Lg");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            var logString = Newtonsoft.Json.JsonConvert.SerializeObject(update);
            FileStream fileStreamLog = new FileStream(@"BotFile.log", FileMode.Append);
            if (update != null)
            {
                for (int i = 0; i < logString.Length; i++)
                {
                    byte[] array = Encoding.Default.GetBytes(logString.ToString());
                    fileStreamLog.Write(array, 0, array.Length);
                }
            }
            fileStreamLog.Close();

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет " + update.Message.From.FirstName);
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на сайт АвтоРесурс Сервис!");
                    await botClient.SendTextMessageAsync(message.Chat, "Хотите посмотреть каталог брендов?");
                    Http.User user = new Http.User(update.Message.From.FirstName, update.Message.From.LastName);
                    var senddata = JsonConvert.SerializeObject(user);
                    string str = Post.Send("Users", "SaveTelegramUser", senddata).Result;
                    return;
                }
                if (message.Text.ToLower() == "ги")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Луцк привет!!");
                }
                if (message.Text.ToLower() == "да" || message.Text.ToLower() == "так")
                {
                    string str = Post.Send("Tires", "GetBotTires").Result;
                    if (str != "")
                    {
                        string[] brends = JsonConvert.DeserializeObject<string[]>(str);
                        for (int i = 0; i < brends.Length; i++)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "/" + brends[i]);
                        }

                        await botClient.SendTextMessageAsync(message.Chat, "Выберите бренд");
                    }

                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Виберите ширину");
                    string strwidth = Post.Send("Tires", "GetBotWidth", message.Text).Result;
                    if (strwidth != "")
                    {
                        string[] width = JsonConvert.DeserializeObject<string[]>(strwidth);
                        for (int i = 0; i < width.Length; i++)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "/" + width[i]);
                        }
                    }
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            Post post = new Post();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
