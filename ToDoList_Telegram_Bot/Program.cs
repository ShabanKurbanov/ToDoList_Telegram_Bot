using System.Reflection;
using ToDoList_Telegram_Bot;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using ToDoList_Telegram_Bot.Interface;
namespace TGBot
{
	internal class Program
	{		
		static void Main(string[] args)
		{
			
			IUserService userService = new UserService();
			IToDoService toDoService = new ToDoService(userService);
			IUpdateHandler handler = new UpdateHandler(userService, toDoService);
			ITelegramBotClient botClient = new ConsoleBotClient();
			

			botClient.StartReceiving(handler);
		}
	}

}
