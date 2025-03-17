using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot;

namespace TGBot
{
	internal class UserClass
	{
		private bool _flag = true;
		private string _firstName = string.Empty;
		private string _listCommand = "/addtask, /showtasks, /removetask, /edittask, /taskcompleted, /showcompleted, /info, /help, /echo, /exit";
		List<string> _listTask = new List<string>();
		List<string> _completedTasks = new List<string>();
		private const int MAX_COUNT_TASK = 100;
		private const int MIN_COUNT_TASK = 1;
		private int _countTask;

		public string ListCommand { get => _listCommand; }
		public bool Flag { get => _flag; }

		//Метод обратобки команд
		public void CommandHandler()
		{
			Command_Start();
			Console.WriteLine($"Доступные команды: {ListCommand}");
			while (_flag)
			{
				Console.Write("Введите команду: ");
				string command = Console.ReadLine();

				if (command == "/addtask")
					Command_AddTack();
				else if (command == "/showtasks")
					Command_ShowTacks();
				else if (command == "/removetask")
					Command_RemoveTask();
				else if (command == "/edittask")
					Command_EditTask();
				else if (command == "/taskcompleted")
					Command_TaskCompleted();
				else if (command == "/showcompleted")
					Command_ShowCompleted();
				else if (command == "/info")
					Command_Info();
				else if (command == "/help")
					Command_Help();
				else if (command == "/exit")
					Command_Exit();
				else if (command.Length > 4 && command.Substring(0, 5) == "/echo")
					Command_Echo(command);
				else Console.WriteLine("Команда введена не правильно.");
			}
		}

		//Команда запуска приложения
		private void Command_Start()
		{
			bool flag = true;
			while (flag && _firstName == string.Empty)
			{
				Console.Write("Пожалуйста, введите ваше имя: ");
				string firstName = Console.ReadLine();

				if (firstName == string.Empty)
				{
					Console.WriteLine("Имя не должно быть пустым! ");
				}
				else
				{
					_firstName = firstName;
					flag = false;
				}
			}

			if (_countTask == 0)
				Command_ColTask();

			Console.WriteLine($"Привет, {_firstName}! Чем могу помочь?");
		}

		//Количество задач 
		private void Command_ColTask()
		{
			Console.WriteLine("Введите максимально допустимое количество задач");
			string index = Console.ReadLine();
			int value = index == string.Empty ? 0 : int.Parse(index);

			if (value > 100)			
				throw new ArgumentException("Превышено количество допустимых задач");
			else if (value < 1)
				throw new ArgumentException("Количество задач не может быть меньше 1");

			_countTask = value;
		}

		//Добавить задачу
		private void Command_AddTack()
		{			

			while (true)
			{
				Console.Write("Пожалуйста, введите описание задачи: ");
				string task = Console.ReadLine();

				if (_countTask <= _listTask.Count)
					throw new TaskCountLimitException(_countTask);
				if (task.Length > 100)
					throw new TaskLengthLimitExeption(task.Length, 100);
				ValidateString(task);
				if (task != string.Empty)
				{
					Command_DuplicateTask(task);
					_listTask.Add(task);
					Console.WriteLine("Задача добавлена.");
					return;
				}
				Console.WriteLine("Задача не может быть пустым!");
			}
		}

		//Метод обработки задач
		private void ValidateString(string str)
		{
			if (str != string.Empty && str[0] == ' ')
				throw new ArgumentException("Задача не может начинаться со знака пробел");
		}

		//Проверка дупликата задач
		private void Command_DuplicateTask(string task)
		{
			for (int i = 0; i < _listTask.Count; i++)
			{
				if (task == _listTask[i])
					throw new DuplicateTaskException(task);
			}
		}

		//Показачать список задач
		private void Command_ShowTacks()
		{
			if (_listTask.Count != 0)
			{
				Console.WriteLine($"Список задач: {_listTask.Count} : {_countTask}");
				int count = 1;
				foreach (var item in _listTask)
				{
					Console.WriteLine($"\t{count}) - {item}");
					count++;
				}
			}
			else
			{
				Console.WriteLine("Список задач пуст!");
			}
		}

		//Изменить задачу
		private void Command_EditTask()
		{
			Command_ShowTacks();

			while (true)
			{
				Console.Write("Введите номер задачи, которую хотите изменить: ");
				string str = Console.ReadLine();
				int index = str == string.Empty ? 0 : int.Parse(str);

				if (index <= _listTask.Count && index > 0)
				{
					index -= 1;
					while (true)
					{
						Console.Write("Введите задачу: ");
						string editTasks = Console.ReadLine();
						if (editTasks != string.Empty)
						{
							_listTask.RemoveAt(index);
							_listTask.Insert(index, editTasks);
							Console.WriteLine("Задача изменена.");
							Command_ShowTacks();
							return;
						}
						Console.WriteLine("Задача не может быть пустым!");
					}
				}

				Console.WriteLine("Введен неправильный номер задачи.");
			}
		}

		//Удалить задачу
		private void Command_RemoveTask()
		{
			Command_ShowTacks();
			while (true)
			{
				Console.Write("Введите номер задачи которую хотите удалить: ");

				string str = Console.ReadLine();
				int index = str == string.Empty ? 0 : int.Parse(str);

				if (index <= _listTask.Count && index > 0)
				{
					index--;
					string task = _listTask[index];
					_listTask.RemoveAt(index);
					Console.WriteLine($"Задача: \"{task}\" удалена");
					return;
				}
				Console.WriteLine("Введен не корректный номер задачи.");
			}
		}

		//Задача выполнена
		private void Command_TaskCompleted()
		{
			Command_ShowTacks();
			while (true)
			{
				Console.Write("Введите номер выполненной задачи: ");

				string str = Console.ReadLine();
				int index = str == string.Empty ? 0 : int.Parse(str);

				if (index <= _listTask.Count && index > 0)
				{
					index--;
					string task = _listTask[index];
					_listTask.RemoveAt(index);
					_completedTasks.Add(task);
					Console.WriteLine($"Задача: \"{task}\" выполнена.");
					return;
				}
				Console.WriteLine("Введен не корректный номер задачи.");
			}

		}

		//Выполненные задачи
		private void Command_ShowCompleted()
		{
			if (_completedTasks.Count != 0)
			{
				Console.WriteLine("Список выполненных задач: ");
				int count = 1;
				foreach (var item in _completedTasks)
				{
					Console.WriteLine($"\t{count}) - {item}");
					count++;
				}
			}
			else
			{
				Console.WriteLine("Список задач пуст!");
			}
		}

		//Информация о командах
		private void Command_Help()
		{
			Console.WriteLine("Перед вами Telegram bot для работы необходимо вести команду /start и ввести имя");
			Console.WriteLine("\t -/start - запуск Telegram bot");
			Console.WriteLine("\t -/addtask - добавить задачу");
			Console.WriteLine("\t -/showtasks - показать задачи");
			Console.WriteLine("\t -/removetask - удалить задачу");
			Console.WriteLine("\t -/edittask - изменить задачу");
			Console.WriteLine("\t -/taskcompleted - задача выполнена");
			Console.WriteLine("\t -/showcompleted - показать выполненные задачи");
			Console.WriteLine("\t -/info - узнать инофрмацию о Telegram bot");
			Console.WriteLine("\t -/exit - выйти из Telegram bot");
			Console.WriteLine("\t -/echo - вывод введенных данных");

		}

		//Информация о Программе
		private void Command_Info()
		{
			Version vetsion = Assembly.GetExecutingAssembly().GetName().Version;
			DateTime creationTgB = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
			Console.WriteLine($"Версия программы: {vetsion}");
			Console.WriteLine($"Дата создания: {creationTgB}");
		}

		//Выход из Приложения
		private void Command_Exit()
		{
			_flag = false;
		}

		//Вывод введенных пользователем данных
		private void Command_Echo(string command)
		{
			Console.WriteLine(command.Substring(5));
		}
	}
}
