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
			Console.WriteLine($"Добро пожаловать! Для запуска приложения введите команду: /start");

			while (flag)
			{
				try
				{
					string? command = Console.ReadLine();
					

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
