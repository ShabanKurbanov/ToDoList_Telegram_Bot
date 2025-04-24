using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Exceptions;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Model;
using ToDoList_Telegram_Bot.Types;

namespace ToDoList_Telegram_Bot
{
	class UpdateHandler : IUpdateHandler
	{
		private bool _flag = true;
		private string? _firstName = string.Empty;
		private string _listCommand = "/addtask, /showtasks, /showalltasks, /removetask, /edittask,  /completetask, /showcompleted, /info, /help, /echo, /exit";
		List<ToDoItem> _listTask = new List<ToDoItem>();
		List<ToDoItem> _completedTasks = new List<ToDoItem>();
		
		
		public string ListCommand { get => _listCommand; }
		public bool Flag { get => _flag; }

		ITelegramBotClient? telegramBotClient;

		IUserService userService = new UserService();
		Chat? _chat;
		ToDoUser? toDoUser;

		IToDoService? toDoService;

		public UpdateHandler()
		{
			toDoService = new ToDoService();
		}

		//Метод обратобки команд
		public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
		{
			telegramBotClient = botClient;

			if (update?.Message?.Chat != null)
			{
				_chat = update?.Message?.Chat;
				_firstName = update?.Message?.From?.Username;
			}
			while (_flag)
			{
				telegramBotClient.SendMessage(_chat!, $"Добро пожаловать! Для запуска приложения введите команду: /start");
				string? command = Console.ReadLine();
				if (command != null && command != string.Empty && command == "/start")
				{
					toDoUser = userService.RegisterUser(update!.Message!.From!.TelegramUserId, update.Message.From.TelegramUserName!);
					_flag = false;
				}
					
			}

			_flag = true;
			
			telegramBotClient!.SendMessage(_chat!, $"Привет, {_firstName}! Чем могу помочь?");
			
			while (_flag)
			{
				try
				{
					if(toDoService?.CountTask == 0)
						CommandColTask(); 

					botClient.SendMessage(_chat!, $"Доступные команды: {ListCommand}");
					botClient.SendMessage(_chat!, "Введите команду: ");
					string? command = Console.ReadLine();
					if (command != null)
						switch (command)
						{
							case var _ when command.StartsWith("/addtask") :
								CommandAddTack(command);
								break;
							case "/showtasks":
								CommandShowTacks();
								break;
							case "/showalltasks":
								CommandAllShowTacks();
								break;
							case var _ when command.StartsWith("/removetask"):
								CommandRemoveTask(command);
								break;
							case "/edittask":
								CommandEditTask();
								break;
							case "/completetask":
								CommandTaskCompleted();
								break;
							case "/showcompleted":
								CommandShowCompleted();
								break;
							case "/info":
								CommandInfo();
								break;
							case "/help":
								CommandHelp();
								break;
							case "/exit":
								CommandExit();
								break;
							default:
								botClient.SendMessage(_chat!, "Команда введена не правильно.");
								break;
						}
				}
				catch (ArgumentException e)
				{
					Console.WriteLine(e.Message);

				}

				catch (TaskCountLimitException e)
				{
					Console.WriteLine(e.Message);

				}
				catch (TaskLengthLimitException e)
				{
					Console.WriteLine(e.Message);

				}
				catch (DuplicateTaskException e)
				{
					Console.WriteLine(e.Message);

				}
			}
		}

		private void CommandAllShowTacks()
		{
			if (toDoService!.ShowTacks().Count != 0)
			{
				telegramBotClient!.SendMessage(_chat!, $"Список задач: {toDoService!.ShowTacks().Count} : {toDoService?.CountTask}");
				int count = 1;
				foreach (var item in toDoService!.ShowTacks())
				{
					telegramBotClient!.SendMessage(_chat!, $"\t{count}): [{item.State}] - {item.Name} - {item.CreatedAt} - {item.Id}");
					count++;
				}
			}
			else
			{
				telegramBotClient!.SendMessage(_chat!, "Список задач пуст!");
			}
		}

		//Количество задач 
		private void CommandColTask()
		{			
			telegramBotClient!.SendMessage(_chat!, "Введите максимально допустимое количество задач");
			toDoService?.ColTask();
		}

		//Добавить задачу
		private void CommandAddTack(string name)
		{
			while (true)
			{
				string task = name.Substring(9);
				 
				if (task != null && task != string.Empty)
				{			
					toDoService?.Add(toDoUser!, task);
					telegramBotClient?.SendMessage(_chat!, "Задача добавлена.");
					return;
				}
				else
				{
					telegramBotClient?.SendMessage(_chat!, "Задача не может быть пустым!");
				}
			}
		}

		

		//Метод обработки задач
		private void ValidateString(string str)
		{
			if (str != string.Empty && str[0] == ' ')
				throw new ArgumentException("Задача не может начинаться со знака пробел");
		}

		//Проверка дупликата задач
		private void CommandDuplicateTask(string task)
		{
			for (int i = 0; i < _listTask.Count; i++)
			{
				if (task == _listTask[i].Name)
					throw new DuplicateTaskException(task);
			}
		}

		//Показачать список задач
		private void CommandShowTacks()
		{
			if (toDoService!.ShowTacks().Count != 0)
			{
				telegramBotClient!.SendMessage(_chat!, $"Список задач: {toDoService!.ShowTacks().Count} : {toDoService?.CountTask}");
				int count = 1;
				foreach (var item in toDoService!.ShowTacks())
				{
					telegramBotClient!.SendMessage(_chat!, $"\t{count}):  {item.Name} - {item.CreatedAt} - {item.Id}");
					count++;
				}
			}
			else
			{
				telegramBotClient!.SendMessage(_chat!, "Список задач пуст!");
			}
		}

		//Изменить задачу
		private void CommandEditTask()
		{
			
			while (true)
			{
				telegramBotClient!.SendMessage(_chat!, "Введите номер задачи, которую хотите изменить и описание новой задачи ");
				
				while (true)
				{					
					string? editTasks = Console.ReadLine();
					if (editTasks != string.Empty && editTasks != null)
					{
						toDoService?.EditTask(editTasks);
						telegramBotClient!.SendMessage(_chat!, "Задача изменена.");
						return;
					}
					telegramBotClient!.SendMessage(_chat!, "Задача не может быть пустым!");
				}

			}
		}

		//Удалить задачу
		private void CommandRemoveTask(string command)
		{
			toDoService?.RemoveTask(command);
			telegramBotClient!.SendMessage(_chat!, $"Задача удалена");

		}

		//Задача выполнена
		private void CommandTaskCompleted()
		{			
			telegramBotClient!.SendMessage(_chat!, "Введите номер выполненной задачи: ");

			toDoService?.TaskCompleted();

			telegramBotClient!.SendMessage(_chat!, $"Задача выполнена.");

		}

		//Выполненные задачи
		private void CommandShowCompleted()
		{
			if (_completedTasks.Count != 0)
			{
				telegramBotClient!.SendMessage(_chat!, "Список выполненных задач: ");
				int count = 1;
				foreach (var item in _completedTasks)
				{
					Console.WriteLine($"\t{count}) - {item}");
					count++;
				}
			}
			else
			{
				telegramBotClient!.SendMessage(_chat!, "Список задач пуст!");
			}
		}

		//Информация о командах
		private void CommandHelp()
		{
			telegramBotClient!.SendMessage(_chat!, "Перед вами Telegram bot для работы необходимо вести команду /start и ввести имя");
			telegramBotClient!.SendMessage(_chat!, "\t -/start - запуск Telegram bot");
			telegramBotClient!.SendMessage(_chat!, "\t -/addtask - добавить задачу");
			telegramBotClient!.SendMessage(_chat!, "\t -/showtasks - показать задачи");
			telegramBotClient!.SendMessage(_chat!, "\t -/showalltasks - показать весь список задач с пометкой о выполнении");
			telegramBotClient!.SendMessage(_chat!, "\t -/removetask - удалить задачу");
			telegramBotClient!.SendMessage(_chat!, "\t -/edittask - изменить задачу");
			telegramBotClient!.SendMessage(_chat!, "\t -/completetask - задача выполнена");
			telegramBotClient!.SendMessage(_chat!, "\t -/showcompleted - показать выполненные задачи");
			telegramBotClient!.SendMessage(_chat!, "\t -/info - узнать инофрмацию о Telegram bot");
			telegramBotClient!.SendMessage(_chat!, "\t -/exit - выйти из Telegram bot");
			telegramBotClient!.SendMessage(_chat!, "\t -/echo - вывод введенных данных");

		}

		//Информация о Программе
		private void CommandInfo()
		{
			Version? vetsion = Assembly.GetExecutingAssembly().GetName().Version;
			DateTime creationTgB = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
			telegramBotClient!.SendMessage(_chat!, $"Версия программы: {vetsion}");
			telegramBotClient!.SendMessage(_chat!, $"Дата создания: {creationTgB}");
		}

		//Выход из Приложения
		private void CommandExit()
		{
			_flag = false;
		}
	}
}
