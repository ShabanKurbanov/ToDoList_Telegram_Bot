using Microsoft.VisualBasic;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Exception;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Model;

namespace ToDoList_Telegram_Bot
{
	internal class ToDoService : IToDoService
	{

		private const int MAX_COUNT_TASK = 100;
		private const int MIN_COUNT_TASK = 1;
		IReadOnlyList<ToDoItem> _toDoItem = new List<ToDoItem>();
		List<ToDoItem> _listTask = new List<ToDoItem>();
		List<ToDoItem> _completedTasks = new List<ToDoItem>();
		ToDoUser? toDoUser;
		private bool _flag = true;

		public int CountTask { get; set; }

		private readonly IUserService? _userService;

		public ToDoService() { }

		public ToDoService(IUserService userService)
		{
			_userService = userService;
		}

		public ToDoItem Add(ToDoUser user, string name)
		{
			throw new NotImplementedException();
		}

		public void Delete(Guid id, ITelegramBotClient _botClient, Update _update)
		{
			CommandShowTacks(_botClient, _update);

			for (int i = 0; i < _listTask.Count; i++)
			{
				if (id == _listTask[i].Id)
				{
					_listTask.RemoveAt(i);
					_botClient!.SendMessage(_update!.Message.Chat, $"Задача: \"{_listTask[i].Name}\" удалена - [{id}]");
				}
			}
		}

		public IReadOnlyList<ToDoItem> GetActiveByUserId(long userId)
		{
			List<ToDoItem> toDoItems = new List<ToDoItem>();

			for (int i = 0; i < _listTask.Count; i++)
			{
				if (_listTask[i].State == ToDoItemState.Active)
					toDoItems.Add( _listTask[i]);
			}

			return toDoItems;
		}

		public IReadOnlyList<ToDoItem> GetAllByUserId(long userId)
		{
			return _listTask;
		}

		public void MarkAsCopleted(Guid id, ITelegramBotClient _botClient, Update _update)
		{
			if (id != Guid.Empty)
			{
				CommandShowTacks(_botClient, _update);

				for (int i = 0; i < _listTask.Count; i++)
				{
					if (id == _listTask[i].Id)
					{
						var task = _listTask[i];
						task.State = ToDoItemState.Completed;
						_listTask.RemoveAt(i);
						_completedTasks.Add(task);
						_botClient!.SendMessage(_update!.Message.Chat, $"Задача: \"{task.Name}\" выполнена - {id}.");

					}
				}			

			}
			else
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Строка не может быть пустым!");
				return;
			}
		}

		//Команда запуска приложения
		public void CommandStart(ITelegramBotClient _botClient, Update _update)
		{
			while (_flag)
			{

				if (_userService!.GetUser(_update!.Message.From.Id) != null) return;

				_botClient!.SendMessage(_update!.Message.Chat, "Доступные команды: /start, /help, /info, /exit");

				_botClient!.SendMessage(_update.Message.Chat, "Введите команду: ");
				string? command = Console.ReadLine();
				if (command != null)
					switch (command)
					{
						case "/start":
							_userService.RegisterUser(_update.Message.From.Id, _update.Message.From.Username!);
							break;
						case "/info":
							CommandInfo(_botClient, _update);
							break;
						case "/help":
							CommandHelp(_botClient, _update);
							break;
						case "/exit":
							_flag = CommandExit();
							break;
						default:
							_botClient.SendMessage(_update.Message.Chat, "Команда введена не правильно.");
							break;
					}
			}
		}


		

		//Добавить задачу
		public void CommandAddTack(ITelegramBotClient _botClient, Update _update, string command)
		{

			while (true)
			{
				//_botClient!.SendMessage(_update!.Message.Chat, "Пожалуйста, введите описание задачи: ");
				//string? task = Console.ReadLine();

				string task = command.Substring(9);

				if (task != null && task != string.Empty)
				{
					if (CountTask <= _listTask.Count)
						throw new TaskCountLimitException(CountTask);
					else if (task.Length > 100)
						throw new TaskLengthLimitException(task.Length, 100);
					ValidateString(task);
					CommandDuplicateTask(task);
					var toDoItem = new ToDoItem();
					toDoItem.Name = task;
					toDoItem.User = toDoUser;
					_listTask.Add(toDoItem);
					_botClient!.SendMessage(_update!.Message.Chat, "Задача добавлена.");
					return;
				}
				else
				{
					_botClient!.SendMessage(_update!.Message.Chat, "Задача не может быть пустым!");
					return;
				}

			}
		}

		//Показачать список задач
		public void CommandShowTacks(ITelegramBotClient _botClient, Update _update)
		{
			if (_listTask.Count != 0)
			{
				_botClient!.SendMessage(_update!.Message.Chat, $"Список задач: {_listTask.Count} : {CountTask}");
				int count = 1;
				foreach (var item in _listTask)
				{
					_botClient!.SendMessage(_update!.Message.Chat, $"\t{count}) - {item.Name} - {item.CreatedAt} - {item.Id} - [{item.State}]");
					count++;
				}
			}
			else
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Список задач пуст!");
			}
		}

		//Удалить задачу
		public void CommandRemoveTask(ITelegramBotClient _botClient, Update _update, string command)
		{

			if (command != null && command != string.Empty)
			{
				CommandShowTacks(_botClient, _update);
				//_botClient!.SendMessage(_update!.Message.Chat, "Введите номер задачи которую хотите удалить: ");

				string? str = command.Substring(12);
				int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);

				index--;
				var task = _listTask[index];
				_listTask.RemoveAt(index);
				_botClient!.SendMessage(_update!.Message.Chat, $"Задача: \"{task.Name}\" удалена");

			}
			else
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Строка не может быть пустым!");
				return;
			}

		}

		//Изменить задачу
		public void CommandEditTask(ITelegramBotClient _botClient, Update _update)
		{
			CommandShowTacks(_botClient, _update);

			while (true)
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Введите номер задачи, которую хотите изменить: ");
				string? str = Console.ReadLine();
				int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);
				index -= 1;
				while (true)
				{
					_botClient!.SendMessage(_update!.Message.Chat, "Введите задачу: ");
					string? editTasks = Console.ReadLine();
					if (editTasks != string.Empty && editTasks != null)
					{
						var toDoItem = _listTask[index];
						toDoItem.Name = editTasks;
						toDoItem.StateChangedAt = DateTime.Now;
						_listTask.RemoveAt(index);
						_listTask.Insert(index, toDoItem);
						_botClient!.SendMessage(_update!.Message.Chat, "Задача изменена.");
						CommandShowTacks(_botClient, _update);
						return;
					}
					_botClient!.SendMessage(_update!.Message.Chat, "Задача не может быть пустым!");
					return;
				}

			}
		}

		//Задача выполнена
		public void CommandTaskCompleted(ITelegramBotClient _botClient, Update _update)
		{
			CommandShowTacks(_botClient, _update);
			_botClient!.SendMessage(_update!.Message.Chat, "Введите номер выполненной задачи: ");

			string? str = Console.ReadLine();
			int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);

			index--;
			var task = _listTask[index];
			task.State = ToDoItemState.Completed;
			_listTask.RemoveAt(index);
			_completedTasks.Add(task);
			_botClient!.SendMessage(_update!.Message.Chat, $"Задача: \"{task}\" выполнена.");

		}

		//Выполненные задачи
		public void CommandShowCompleted(ITelegramBotClient _botClient, Update _update)
		{
			if (_completedTasks.Count != 0)
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Список выполненных задач: ");
				int count = 1;
				foreach (var item in _completedTasks)
				{
					_botClient!.SendMessage(_update!.Message.Chat, $"\t{count}) - {item}");
					count++;
				}
			}
			else
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Список задач пуст!");
			}
		}

		//Информация о Программе
		public void CommandInfo(ITelegramBotClient _botClient, Update _update)
		{
			Version? vetsion = Assembly.GetExecutingAssembly().GetName().Version;
			DateTime creationTgB = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
			_botClient!.SendMessage(_update!.Message.Chat, $"Версия программы: {vetsion}");
			_botClient!.SendMessage(_update!.Message.Chat, $"Дата создания: {creationTgB}");
		}

		//Информация о командах
		public void CommandHelp(ITelegramBotClient _botClient, Update _update)
		{
			_botClient!.SendMessage(_update!.Message.Chat, "Перед вами Telegram bot для работы необходимо вести команду /start и ввести имя");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/start - запуск Telegram bot");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/addtask - добавить задачу, описание задачи писать сразу после ввода команды через пробел");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/showtasks - показать задачи");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/removetask - удалить задачу, номер задачи которую необходимо удалить писать сразу после ввода комнады через пробел");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/edittask - изменить задачу");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/taskcompleted - задача выполнена");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/completetask - выполнение задачи по Guid");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/delete - удаление задачи по Guid");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/showcompleted - показать выполненные задачи");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/info - узнать инофрмацию о Telegram bot");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/exit - для выхода из Telegram bot сначало необходимо вести команду /exit затем нажать Ctrl + C");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/echo - вывод введенных данных");

		}

		//Количество задач 
		public void CommandColTask(ITelegramBotClient _botClient, Update _update)
		{
			_botClient!.SendMessage(_update!.Message.Chat, "Введите максимально допустимое количество задач");
			string? index = Console.ReadLine();
			CountTask = ParseAndValidateInt(index, MIN_COUNT_TASK, MAX_COUNT_TASK);
		}

		//Проверка конвертации строки
		private int ParseAndValidateInt(string? str, int min, int max)
		{

			int value = (str != null && str != string.Empty) ? int.Parse(str) : throw new ArgumentException("Строка не может быть пустым");

			if (value >= max)
				throw new ArgumentException("Превышено количество допустимых задач");
			else if (value < min)
				throw new ArgumentException("Количество задач не может быть меньше 1");

			return value;
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
				var toDoItem = _listTask[i];
				if (task == toDoItem.Name)
					throw new DuplicateTaskException(task);
			}
		}

		//Выход из Приложения
		public bool CommandExit()
		{
			return  false;
		}
	}
}
