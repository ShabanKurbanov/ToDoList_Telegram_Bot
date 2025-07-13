using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Reflection;

namespace ToDoList_Telegram_Bot
{
	class UpdateHandler : IUpdateHandler
	{
		IUserService? _userService;
		ITelegramBotClient? _botClient;
		Update? _update;

		private bool _flag = true;
		private string? _firstName = string.Empty;
		private string _listCommand = "/addtask, /showtasks, /removetask, /edittask, /taskcompleted, /showcompleted, /info, /help, /echo, /exit";
		List<string> _listTask = new List<string>();
		List<string> _completedTasks = new List<string>();
		private const int MAX_COUNT_TASK = 100;
		private const int MIN_COUNT_TASK = 1;
		private int _countTask;

		public string ListCommand { get => _listCommand; }
		public bool Flag { get => _flag; }

		//Метод обратобки команд
		public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
		{
			botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");

			_botClient = botClient;
			_update = update;

			CommandStart();

			botClient.SendMessage(update.Message.Chat, $"Доступные команды: {ListCommand}");
			while (_flag)
			{
				try
				{
					botClient.SendMessage(update.Message.Chat, "Введите команду: ");
					string? command = Console.ReadLine();
					if (command != null)
						switch (command)
						{
							case "/addtask":
								CommandAddTack();
								break;
							case "/showtasks":
								CommandShowTacks();
								break;
							case "/removetask":
								CommandRemoveTask();
								break;
							case "/edittask":
								CommandEditTask();
								break;
							case "/taskcompleted":
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
								botClient.SendMessage (update.Message.Chat, "Команда введена не правильно.");
								break;
						}
				}

				catch (ArgumentException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}

				catch (TaskCountLimitException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}
				catch (TaskLengthLimitException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}
				catch (DuplicateTaskException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}

			}
		}

		//Команда запуска приложения
		private void CommandStart()
		{

			_userService = new UserService();

			//bool flag = true;
			//while (flag && _firstName == string.Empty)
			//{
			//	Console.Write("Пожалуйста, введите ваше имя: ");
			//	string? firstName = Console.ReadLine();

			//	if (firstName == string.Empty)
			//	{
			//		Console.WriteLine("Имя не должно быть пустым! ");
			//	}
			//	else
			//	{
			//		_firstName = firstName;
			//		flag = false;
			//	}
			//}

			//if (_countTask == 0)
			//	CommandColTask();

			//Console.WriteLine($"Привет, {_firstName}! Чем могу помочь?");
		}

		//Количество задач 
		private void CommandColTask()
		{
			_botClient!.SendMessage(_update!.Message.Chat, "Введите максимально допустимое количество задач");
			string? index = Console.ReadLine();
			_countTask = ParseAndValidateInt(index, MIN_COUNT_TASK, MAX_COUNT_TASK);
		}

		//Добавить задачу
		private void CommandAddTack()
		{

			while (true)
			{
				_botClient!.SendMessage(_update!.Message.Chat, "Пожалуйста, введите описание задачи: ");
				string? task = Console.ReadLine();

				if (task != null && task != string.Empty)
				{
					if (_countTask <= _listTask.Count)
						throw new TaskCountLimitException(_countTask);
					else if (task.Length > 100)
						throw new TaskLengthLimitException(task.Length, 100);
					ValidateString(task);
					CommandDuplicateTask(task);
					_listTask.Add(task);
					_botClient!.SendMessage(_update!.Message.Chat, "Задача добавлена.");
					return;
				}
				else
				{
					_botClient!.SendMessage(_update!.Message.Chat, "Задача не может быть пустым!");
				}
			}
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
				if (task == _listTask[i])
					throw new DuplicateTaskException(task);
			}
		}

		//Показачать список задач
		private void CommandShowTacks()
		{
			if (_listTask.Count != 0)
			{
				_botClient!.SendMessage(_update!.Message.Chat, $"Список задач: {_listTask.Count} : {_countTask}");
				int count = 1;
				foreach (var item in _listTask)
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

		//Изменить задачу
		private void CommandEditTask()
		{
			CommandShowTacks();

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
						_listTask.RemoveAt(index);
						_listTask.Insert(index, editTasks);
						_botClient!.SendMessage(_update!.Message.Chat, "Задача изменена.");
						CommandShowTacks();
						return;
					}
					_botClient!.SendMessage(_update!.Message.Chat, "Задача не может быть пустым!");
				}

			}
		}

		//Удалить задачу
		private void CommandRemoveTask()
		{
			CommandShowTacks();
			_botClient!.SendMessage(_update!.Message.Chat, "Введите номер задачи которую хотите удалить: ");

			string? str = Console.ReadLine();
			int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);

			index--;
			string? task = _listTask[index];
			_listTask.RemoveAt(index);
			_botClient!.SendMessage(_update!.Message.Chat, $"Задача: \"{task}\" удалена");

		}

		//Задача выполнена
		private void CommandTaskCompleted()
		{
			CommandShowTacks();
			_botClient!.SendMessage(_update!.Message.Chat, "Введите номер выполненной задачи: ");

			string? str = Console.ReadLine();
			int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);

			index--;
			string? task = _listTask[index];
			_listTask.RemoveAt(index);
			_completedTasks.Add(task);
			_botClient!.SendMessage(_update!.Message.Chat, $"Задача: \"{task}\" выполнена.");

		}

		//Выполненные задачи
		private void CommandShowCompleted()
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

		//Информация о командах
		private void CommandHelp()
		{
			_botClient!.SendMessage(_update!.Message.Chat, "Перед вами Telegram bot для работы необходимо вести команду /start и ввести имя");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/start - запуск Telegram bot");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/addtask - добавить задачу");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/showtasks - показать задачи");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/removetask - удалить задачу");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/edittask - изменить задачу");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/taskcompleted - задача выполнена");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/showcompleted - показать выполненные задачи");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/info - узнать инофрмацию о Telegram bot");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/exit - выйти из Telegram bot");
			_botClient!.SendMessage(_update!.Message.Chat, "\t -/echo - вывод введенных данных");

		}

		//Информация о Программе
		private void CommandInfo()
		{
			Version? vetsion = Assembly.GetExecutingAssembly().GetName().Version;
			DateTime creationTgB = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
			_botClient!.SendMessage(_update!.Message.Chat, $"Версия программы: {vetsion}");
			_botClient!.SendMessage(_update!.Message.Chat, $"Дата создания: {creationTgB}");
		}

		//Выход из Приложения
		private void CommandExit()
		{
			_flag = false;
		}

	}
}
