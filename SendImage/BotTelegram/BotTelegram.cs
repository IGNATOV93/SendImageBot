using IniParser;
using IniParser.Model;
using System.Linq.Expressions;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SendImage.BotTelegram
{
    public abstract class BotTelegram
    {
        private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
        private static FileIniDataParser parser = new FileIniDataParser();

        public static IniData data = parser.ReadFile(path);
        private static IniData dataini = parser.ReadFile(path);
        static private TelegramBotClient client = new TelegramBotClient(data["Profile0"]["YourBotTelegreamToken"]);
        public static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            try
            {
                var message = update.Message;
                long idMessage= 0; 
                string idChat="";

                if (message?.Type == MessageType.Photo)
                {

                    idChat = message.Chat.Id.ToString();
                    idMessage= message.MessageId;
                    var idPhoto = message.MediaGroupId;
                    var name = message.Chat.FirstName;
                    var link = $"tg://user?id={idChat}";
                    var inlineKeyboardLinkUser = new InlineKeyboardMarkup(
                               new[]
                                 {
                                    InlineKeyboardButton.WithUrl("\U0001F7E2 Связаться",link)
                                 });

                    await botClient.SendTextMessageAsync(idChat, "Ваша заявка принята !");
                    await botClient.SendTextMessageAsync(idChat, $"Заявка от : {name }", replyMarkup: inlineKeyboardLinkUser) ;
                    await botClient.ForwardMessageAsync(idChat,idChat,(int)idMessage);
                    return;
                }


                if (update?.CallbackQuery?.Data != null) //ОБРАБОТКА UPDATE 
                {
                   idChat=update.CallbackQuery.Message.Chat.Id.ToString();
                    string answerbutton = update.CallbackQuery.Data.ToString();//Что пришло с Inline кнопки
                    idMessage= update.CallbackQuery.Message.MessageId;

                }
                if (message == null) { return; }
                if (message.Text != null) //ОБРАБОТКА СООБЩЕНИЙ 
                {
                    var text = message.Text;
                    idChat = message.Chat.Id.ToString();
                    idMessage = (long)message.Chat?.Id;
                    await botClient.DeleteMessageAsync(idChat, message.MessageId);
                    if (text == "/start")
                    {
                        await botClient.SendTextMessageAsync(idChat, "Привет !" +
                            "Если ты хочешь отправить то что ты умеешь,то просто оптравь их сюда в бота.\r\n" +
                            "Можно до 10 фотографий (Обязательно все фото отправлять одним сообщением!)\r\n" +
                            "Если нас вы заинтересуете ,мы с вами обязательно свяжемся ! ");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(idChat, "Нет такой команды !Бот принимает только фотографии !");
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
            
            return;
        }
        public static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2.Message);
            return Task.CompletedTask;
        }

        internal static void StartsBot()
        {
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }
    }
}