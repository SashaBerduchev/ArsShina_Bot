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
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArsShina_Bot
{
    class Program
    {

        static TelegramBotClient bot = new TelegramBotClient("5520950526:AAEyg5uPGeSDjYAuAxP8O04uVTZDkqfvaIo");
        static Http.User User;
        static bool registarStart = false;
        static bool registarEnd = false;
        static bool IsSelectTire = false;
        static Tire tires;
        static string brend;
        static string width;
        static string haighttire;
        static string rad;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.CallbackQuery != null)
            {
                CallbackQuery callbackQuery = update.CallbackQuery;
                Trace.WriteLine(callbackQuery);
                Trace.WriteLine(callbackQuery.Data);

                if (callbackQuery.Data == "NotShowTires")
                {
                    InlineKeyboardButton[] btn = new[] { InlineKeyboardButton.WithCallbackData("Так", "YesShowFilterTires") };
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { btn });
                    bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Тоді виберіть за фільтром!", replyMarkup: inlineKeyboard);
                    IsSelectTire = true;
                }
                else if (callbackQuery.Data == "YesShowTires")
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
                        string link = "https://www.arsshina.com/Tires/Details/" + elem[i].id;
                        InlineKeyboardButton[] inlineKeyboardButton = new InlineKeyboardButton[1];
                        inlineKeyboardButton[0] = InlineKeyboardButton.WithUrl("Придбати", link);
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(new[] { inlineKeyboardButton });
                        await botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id, inputOnlineFile, "Назва: " + elem[i].Name + "\n" + "Ширина: " + elem[i].Width + "\n" + "Висота: " + elem[i].Height + "\n" + "Діаметр: " + elem[i].Radius + "\n" + "Призначення: " + elem[i].TypeOfTire + "\n" + "Ціна: " + elem[i].Price + "ГРН", replyMarkup: inlineKeyboardMarkup);
                    }
                }
                if (callbackQuery.Data == "YesShowFilterTires")
                {
                    string str = Post.Send("Tires", "GetBotTires").Result;
                    if (str != "")
                    {
                        string[] brends = JsonConvert.DeserializeObject<string[]>(str);
                        Console.WriteLine(str);
                        //var btn = new[] {
                        //    for (int i = 0; i < brends.Length; i++)
                        //    {
                        //        InlineKeyboardButton.WithCallbackData(brends[i], brends[i]);
                        //    }
                        //};
                        InlineKeyboardButton[] btn = new InlineKeyboardButton[brends.Length];
                        for (int i = 0; i < brends.Length; i++)
                        {
                            btn[i] = InlineKeyboardButton.WithCallbackData(brends[i], brends[i]);
                        }
                        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { btn });
                        //bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Тоді виберіть за фільтром!");
                        IsSelectTire = true;
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Виберіть бренд", replyMarkup: inlineKeyboard);
                        return;
                    }
                }

                if (IsSelectTire == true && callbackQuery.Data != "NotShowTires" && brend == null)
                {
                    brend = callbackQuery.Data;
                    string response = Post.Send("Tires", "GetBotWidth", brend).Result;
                    string[] width = JsonConvert.DeserializeObject<string[]>(response);
                    InlineKeyboardButton[] btn = new InlineKeyboardButton[width.Length];
                    for (int i = 0; i < width.Length; i++)
                    {
                        btn[i] = InlineKeyboardButton.WithCallbackData(width[i], width[i]);
                    }
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { btn });
                    bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Виберіть ширину", replyMarkup: inlineKeyboard);
                }
                else if (IsSelectTire == true && callbackQuery.Data != "NotShowTires" && brend != null && brend != "" && width == null)
                {
                    Tires tires = new Tires();
                    tires.Name = brend;
                    width = callbackQuery.Data;
                    tires.Width = Convert.ToInt64(width);
                    string response = Post.Send("Tires", "GetBotHeight", JsonConvert.SerializeObject(tires)).Result;
                    string[] height = JsonConvert.DeserializeObject<string[]>(response);
                    InlineKeyboardButton[] btn = new InlineKeyboardButton[height.Length];
                    for (int i = 0; i < height.Length; i++)
                    {
                        btn[i] = InlineKeyboardButton.WithCallbackData(height[i], height[i]);
                    }
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { btn });
                    bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Виберіть висоту", replyMarkup: inlineKeyboard);
                }
                else if (IsSelectTire == true && callbackQuery.Data != "NotShowTires" && brend != null && brend != "" && width != null && width != "" && haighttire == null)
                {
                    Tires tires = new Tires();
                    tires.Name = brend;
                    haighttire = callbackQuery.Data;
                    tires.Width = Convert.ToInt64(width);
                    tires.Height = Convert.ToInt64(haighttire);
                    string response = Post.Send("Tires", "GetBotRadius", JsonConvert.SerializeObject(tires)).Result;
                    string[] height = JsonConvert.DeserializeObject<string[]>(response);
                    InlineKeyboardButton[] btn = new InlineKeyboardButton[height.Length];
                    for (int i = 0; i < height.Length; i++)
                    {
                        btn[i] = InlineKeyboardButton.WithCallbackData(height[i], height[i]);
                    }
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { btn });
                    bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Виберіть радіус", replyMarkup: inlineKeyboard);
                }
                else if (IsSelectTire == true && brend != null && width != null && haighttire != null)
                {
                    IsSelectTire = false;
                    rad = callbackQuery.Data;
                    Tires tires = new Tires(); ;
                    tires.Name = brend;
                    tires.Width = Convert.ToInt64(width);
                    tires.Height = Convert.ToInt64(haighttire);
                    tires.Radius = rad;
                    var data = Post.Send("Tires", "GetFilterTire", JsonConvert.SerializeObject(tires)).Result; ;
                    List<Tires> elem = JsonConvert.DeserializeObject<List<Tires>>(data);
                    Console.WriteLine(elem);
                    for (int i = 0; i < elem.Count; i++)
                    {
                        var pic = Post.Send("Tires", "GetTiresImage", "/" + elem[i].Name + "/" + elem[i].TypeOfTire).Result;
                        TiresImages tiresImages = JsonConvert.DeserializeObject<TiresImages>(pic);

                        MemoryStream ms = new MemoryStream(tiresImages.Image);
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(ms, tiresImages.ImageMimeTypeOfData);
                        string link = "https://www.arsshina.com/Tires/Details/" + elem[i].id;
                        InlineKeyboardButton[] inlineKeyboardButton = new InlineKeyboardButton[1];
                        inlineKeyboardButton[0] = InlineKeyboardButton.WithUrl("Придбати", link);
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(new[] { inlineKeyboardButton });
                        await botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id, inputOnlineFile, "Назва: " + elem[i].Name + "\n" + "Ширина: " + elem[i].Width + "\n" + "Висота: " + elem[i].Height + "\n" + "Діаметр: " + elem[i].Radius + "\n" + "Призначення: " + elem[i].TypeOfTire + "\n" + "Ціна: " + elem[i].Price + "ГРН", replyMarkup: inlineKeyboardMarkup);
                    }
                    InlineKeyboardButton[] btn = new InlineKeyboardButton[2];
                    btn[0] = InlineKeyboardButton.WithCallbackData("Так", "YesShowFilterTires");
                    btn[1] = InlineKeyboardButton.WithCallbackData("Ні", "NoTires");
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { btn });
                    bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Хочете побачити іще шини?", replyMarkup: inlineKeyboard);

                    brend = null;
                    width = null;
                    haighttire = null;
                    rad = null;
                }
                if (callbackQuery.Data == "NoTires")
                {
                    bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Дякую");
                }
            }

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
            if (update.Message != null)
            {
                TelegramBotUser telegramBotUser = new TelegramBotUser();
                telegramBotUser.idChat = update.Message.Chat.Id;
                telegramBotUser.Name = update.Message.Chat.FirstName;
                telegramBotUser.Login = update.Message.Chat.Username;
                var sendTelegramUser = JsonConvert.SerializeObject(telegramBotUser);
                Post.Send("Home", "SetBotUser", sendTelegramUser);
            }
            //await botClient.SendTextMessageAsync(message.Chat, "Хотите посмотреть каталог брендов?");


            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text == null)
                {
                    return;
                }
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привіт " + update.Message.From.FirstName + " " + message.From.LastName);
                    await botClient.SendTextMessageAsync(message.Chat, "Вас вітає Авто Ресурс Сервис!");
                    //await botClient.SendTextMessageAsync(message.Chat, "Хотите посмотреть каталог брендов?");
                    await botClient.SendTextMessageAsync(message.Chat, "Для перегляду усіх команд викоистовуйте: /help");
                    InlineKeyboardButton[] inlineKeyboardButtons = new InlineKeyboardButton[1];
                    inlineKeyboardButtons[0] = InlineKeyboardButton.WithUrl("АвтоРесурс Сервіс", "https://www.arsshina.com/");
                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(new[] { inlineKeyboardButtons });
                    await botClient.SendTextMessageAsync(message.Chat, "Щоб перейти на сайт використовуйте посилання: ", replyMarkup: inlineKeyboardMarkup);
                    //Http.User user = new Http.User(update.Message.From.FirstName, update.Message.From.LastName);

                    //string str = Post.Send("Users", "SaveTelegramUser", senddata).Result;
                    return;
                }
                if (message.Text.ToLower() == "ги" || message.Text.ToLower() == "гии")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Луцк привет!!");
                    return;
                }

                if (message.Text.ToLower() == "/registration")
                {
                    registarStart = true;
                    User = new Http.User(message.From.FirstName, message.From.LastName, "", "");
                    await botClient.SendTextMessageAsync(message.Chat, "Вас звати " + message.From.FirstName + " " + message.From.LastName);
                    await botClient.SendTextMessageAsync(message.Chat, "Уведіть емейл");
                    return;
                }

                if (registarStart == true)
                {
                    if (User.Email == "")
                    {
                        User.Email = message.Text.ToLower();
                        await botClient.SendTextMessageAsync(message.Chat, "Уведіть пароль");
                        return;
                    }

                    //if (User.Email != "" && User.Password == "" && registarEnd == false)
                    //{
                    //    registarEnd = true;

                    //    return;
                    //}

                    if (User.Password == "")
                    {
                        User.Password = message.Text;
                        if (User.Name != "" && User.Sername != "" && User.Password != "" && User.Email != "")
                        {
                            //User.Id = Convert.ToInt32(message.From.Id);
                            var senddata = JsonConvert.SerializeObject(User);
                            var sendresult = Post.Send("Users", "SaveTelegramUser", senddata).Result;
                            registarStart = false;
                            registarEnd = false;
                            if (sendresult == "Good")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "Ви зареєстровані!");
                                return;
                            }
                            await botClient.SendTextMessageAsync(message.Chat, "Акаунт вже існує!");
                            return;

                        }
                        return;
                    }


                }


                if (message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Список комманд");
                    await botClient.SendTextMessageAsync(message.Chat, "/start - запуск \n /help - допомога \n /showalltires - показати усі товари на сайті \n /registration - регістрація");
                    return;

                }

                if (message.Text.ToLower() == "/showalltires")
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
                        string link = "https://www.arsshina.com/Tires/Details/" + elem[i].id;
                        InlineKeyboardButton[] inlineKeyboardButton = new InlineKeyboardButton[1];
                        inlineKeyboardButton[0] = InlineKeyboardButton.WithUrl("Придбати", link);
                        InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(new[] { inlineKeyboardButton });
                        await botClient.SendPhotoAsync(message.Chat, inputOnlineFile, "Назва: " + elem[i].Name + "\n" + "Ширина: " + elem[i].Width + "\n" + "Висота: " + elem[i].Height + "\n" + "Діаметр: " + elem[i].Radius + "\n" + "Призначення: " + elem[i].TypeOfTire + "\n" + "Ціна: " + elem[i].Price + "ГРН", replyMarkup: inlineKeyboardMarkup);



                    }
                    return;

                }
                //if (message.Text.ToLower() == "да" || message.Text.ToLower() == "так")
                //{
                //    string str = Post.Send("Tires", "GetBotTires").Result;
                //    if (str != "")
                //    {
                //        string[] brends = JsonConvert.DeserializeObject<string[]>(str);
                //        Console.WriteLine(str);
                //        for (int i = 0; i < brends.Length; i++)
                //        {
                //            await botClient.SendTextMessageAsync(message.Chat, "/" + brends[i]);
                //        }

                //        await botClient.SendTextMessageAsync(message.Chat, "Выберите бренд");
                //        return;
                //    }

                //}
                //else
                //{
                //    await botClient.SendTextMessageAsync(message.Chat, "Виберите ширину");
                //    string strwidth = Post.Send("Tires", "GetBotWidth", message.Text).Result;
                //    if (strwidth != "")
                //    {
                //        string[] width = JsonConvert.DeserializeObject<string[]>(strwidth);
                //        for (int i = 0; i < width.Length; i++)
                //        {
                //            await botClient.SendTextMessageAsync(message.Chat, "/" + width[i]);
                //        }
                //    }
                //    return;
                //}
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



            try
            {
                var result = Post.Send("Home", "GetBotUser").Result;
                List<TelegramBotUser> telegramBotUsers = JsonConvert.DeserializeObject<List<TelegramBotUser>>(result);
                for (int i = 0; i < telegramBotUsers.Count; i++)
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        // first row
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Показати всі", "YesShowTires"),
                            InlineKeyboardButton.WithCallbackData("Відсортувати", "NotShowTires")
                        },
                        // second row
                        
                    });

                    bot.SendTextMessageAsync(telegramBotUsers[i].idChat, "Хочете побачити нові колекції вантажних шин?" + " " + "@" + telegramBotUsers[i].Login, replyMarkup: inlineKeyboard);
                    if (telegramBotUsers[i].Login == "Dotnetsqlkukhar")
                    {
                        bot.SendTextMessageAsync(telegramBotUsers[i].idChat, "Button тьфу");
                        bot.SendTextMessageAsync(telegramBotUsers[i].idChat, "Село і квіти за вікном");
                    }
                }

                //bot.OnApiResponseReceived += BotOnCallbackQueryReceived;
                //bot.OnMakingApiRequest += BotOnApiRequest;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }


            while (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                {
                    SendNews();
                }
            }
            while (Console.ReadKey().Key == ConsoleKey.Spacebar)
            {
                {
                    SendSite();
                }
            }
            Console.ReadLine();

        }

        private static async void SendSite()
        {
            var result = Post.Send("Home", "GetBotUser").Result;
            List<TelegramBotUser> telegramBotUsers = JsonConvert.DeserializeObject<List<TelegramBotUser>>(result);
            for (int j = 0; j < telegramBotUsers.Count; j++)
            {
                InlineKeyboardButton[] inlineKeyboardButtons = new InlineKeyboardButton[1];
                inlineKeyboardButtons[0] = InlineKeyboardButton.WithUrl("АвтоРесурс Сервіс", "https://www.arsshina.com/");
                InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(new[] { inlineKeyboardButtons });
                await bot.SendTextMessageAsync(telegramBotUsers[j].idChat, "Не забувайте про наш сайт: ", replyMarkup: inlineKeyboardMarkup);
            }
        }

        private static ValueTask BotOnApiRequest(ITelegramBotClient botClient, ApiRequestEventArgs args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static ValueTask BotOnCallbackQueryReceived(ITelegramBotClient botClient, ApiResponseEventArgs args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static async void SendNews()
        {
            try
            {
                var result = Post.Send("Home", "GetBotUser").Result;
                List<TelegramBotUser> telegramBotUsers = JsonConvert.DeserializeObject<List<TelegramBotUser>>(result);
                for (int i = 0; i < telegramBotUsers.Count; i++)
                {
                    string data = Post.Send("News", "GetNowNewsForBot").Result;
                    List<News> news = JsonConvert.DeserializeObject<List<News>>(data);
                    for (int j = 0; j < news.Count; j++)
                    {
                        string returnBase = "";
                        MemoryStream ms = new MemoryStream(news[j].Image);
                        if (news[j].BaseInfo.Length >= 350)
                        {
                            returnBase = news[j].BaseInfo.Substring(0, 350);
                        }
                        else
                        {
                            returnBase = news[j].BaseInfo;
                        }
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(ms, news[j].ImageMimeTypeOfData);

                        //    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        //{
                        //    // first row
                        //    new []
                        //    {
                        //        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        //        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                        //    },
                        //    // second row
                        //    new []
                        //    {
                        //        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        //        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                        //    }
                        //});
                        await bot.SendPhotoAsync(telegramBotUsers[i].idChat, inputOnlineFile, news[j].NameNews + "\n\n" + returnBase + "\n\n" + news[j].NewsLinkSrc);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
        }
    }
    //private async void BotOnCallbackQueryReceived(object sender, ApiResponseEventArgs req)
    //{

    //}
}
