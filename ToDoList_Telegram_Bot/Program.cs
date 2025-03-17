using System.Reflection;
namespace TGBot
{
	internal class Program
	{
		private static bool flag = true;

		static void Main(string[] args)
		{
			UserClass instance = new UserClass();

			Console.WriteLine($"Добро пожаловать! Для запуска приложения введите команду: /start");

			while (flag)
			{
				string command = Console.ReadLine();
				if (command != null && command != string.Empty && command == "/start")
				{
					instance.CommandHandler();
					flag = false;
				}
				else
				{
					Console.WriteLine("Пожалуйста, введите команду!");
				}

			}
		}
	}

}
