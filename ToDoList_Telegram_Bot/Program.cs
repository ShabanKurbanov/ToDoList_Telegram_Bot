using System.Reflection;
using ToDoList_Telegram_Bot;
namespace TGBot
{
	internal class Program
	{
		private static bool flag = true;
		private static string commandStart = "Пожалуйста, введите команду /start!";
		static void Main(string[] args)
		{
			UserClass instance = new UserClass();

			Console.WriteLine($"Добро пожаловать! Для запуска приложения введите команду: /start");

			while (flag)
			{
				try
				{
					string command = Console.ReadLine();
					if (command != null && command != string.Empty && command == "/start")
					{
						instance.CommandHandler();
						flag = false;
					}
					else
					{
						Console.WriteLine(commandStart);
					}
				}
				catch (ArgumentException e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}

				catch(TaskCountLimitException e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}
				catch (TaskLengthLimitExeption e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}
				catch(DuplicateTaskException e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}
				
			}
		}
	}

}
