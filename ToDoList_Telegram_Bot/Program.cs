using System.Reflection;
using ToDoList_Telegram_Bot;
using ToDoList_Telegram_Bot.Exceptions;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Types;
namespace TGBot
{
	internal class Program
	{
		
		static void Main(string[] args)
		{
			IUpdateHandler handler = new UpdateHandler();
			ITelegramBotClient telegramBotClient = new ConsoleBotClient();
			
				try
				{					
						telegramBotClient.StartReceiving(handler);					
				}
				

				catch(Exception e)
				{
					Console.Write("Не корректный ввод данных: ");
					Console.WriteLine(e.Message);
					
				}
				
			}
		
	}

}
