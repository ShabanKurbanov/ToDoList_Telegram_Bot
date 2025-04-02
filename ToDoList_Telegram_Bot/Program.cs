using System.Reflection;
using ToDoList_Telegram_Bot.Exceptions;
namespace TGBot
{
	internal class Program
	{
		private static bool flag = true;
		private static string commandStart = "Пожалуйста, введите команду /start!";
		static void Main(string[] args)
		{
			CommandHandler instance = new CommandHandler();

			Console.WriteLine($"Добро пожаловать! Для запуска приложения введите команду: /start");

			while (flag)
			{
				try
				{
					string? command = Console.ReadLine();
					if (command != null && command != string.Empty && command == "/start")
					{
						instance.CommandStartApp();
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
				catch (TaskLengthLimitException e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}
				catch(DuplicateTaskException e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}

				catch(Exception e)
				{
					Console.Write("Не корректный ввод данных: ");
					Console.WriteLine(e.Message);
					Console.WriteLine(commandStart);
				}
				
			}
		}
	}

}
