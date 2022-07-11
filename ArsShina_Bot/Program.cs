using ArsShina_Bot.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ArsShina_Bot
{
    class Program
    {
        
        static ITelegramBotClient bot = new TelegramBotClient("5520950526:AAGSxweLlc9RGMxO94IG9_siV2tHxoTXEiI");
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
                    await botClient.SendTextMessageAsync(message.Chat, "Для просмотра всех комманд используйте /help");
                    Http.User user = new Http.User(update.Message.From.FirstName, update.Message.From.LastName);
                    var senddata = JsonConvert.SerializeObject(user);
                    string str = Post.Send("Users", "SaveTelegramUser", senddata).Result;
                    return;
                }
                if (message.Text.ToLower() == "ги")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Луцк привет!!");
                }


                if(message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Список комманд");
                    await botClient.SendTextMessageAsync(message.Chat, "/start - запуск \n /help = помощь \n /showalltires - показать все товары на сайте");
                    return;

                }

                if(message.Text.ToLower() == "/showalltires")
                {
                    var str = Post.Send("Tires", "GetAllTires").Result;
                    List<Tires> elem = JsonConvert.DeserializeObject<List<Tires>>(str);
                    Console.WriteLine(elem);
                    for (int i = 0; i < elem.Count; i++)
                    {
                        var pic = Post.Send("Tires", "GetTiresImage", "/" + elem[i].Name + "/" + elem[i].TypeOfTire).Result;
                        TiresImages tiresImages = JsonConvert.DeserializeObject<TiresImages>(pic);

                        MemoryStream ms = new MemoryStream(tiresImages.Image);
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(ms, tiresImages.ImageMimeTypeOfData);
                        await botClient.SendPhotoAsync(message.Chat, inputOnlineFile, "Назва: " + elem[i].Name + "\n" + "Ширина: " + elem[i].Width + "\n" + "Висота: " + elem[i].Height+"\n" + "Ціна: " + elem[i].Price+"ГРН");
                        //await botClient.SendTextMessageAsync(message.Chat, "Назва " + elem[i].Name+" /n" +"Вага " + elem[i].Width+ " /n" + "Висота "+elem[i].Height+ " /n");
                        //await botClient.SendTextMessageAsync(message.Chat, "/" + elem[i].Width);
                        //await botClient.SendTextMessageAsync(message.Chat, "/" + elem[i].Height);
                      
                        
                    }
                    return;

                }
                if (message.Text.ToLower() == "да" || message.Text.ToLower() == "так")
                {
                    string str = Post.Send("Tires", "GetBotTires").Result;
                    if (str != "")
                    {
                        string[] brends = JsonConvert.DeserializeObject<string[]>(str);
                        Console.WriteLine(str);
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
            var cts = new CancellationTokenSource();
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
