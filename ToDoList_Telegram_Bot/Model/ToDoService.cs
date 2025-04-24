using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Exceptions;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Types;

namespace ToDoList_Telegram_Bot.Model
{
	class ToDoService : IToDoService
	{
		public int CountTask { get; set; }
		List<ToDoItem> _listTask = new List<ToDoItem>();
		List<ToDoItem> _completedTasks = new List<ToDoItem>();
		private const int MAX_COUNT_TASK = 100;
		private const int MIN_COUNT_TASK = 1;

		public ToDoItem Add(ToDoUser user, string name)
		{
			if (CountTask <= _listTask.Count)
				throw new TaskCountLimitException(CountTask);
			else if (name.Length > 100)
				throw new TaskLengthLimitException(name.Length, 100);			

			ValidateString(name);
			CommandDuplicateTask(name);

			ToDoItem toDoItem = new ToDoItem();

			toDoItem.Name = name;
			toDoItem.State = ToDoItemState.Active;
			toDoItem.CreatedAt = DateTime.Now;
			toDoItem.Id = Guid.NewGuid();
			_listTask.Add(toDoItem);

			return toDoItem;
		}	

		public void Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
		{
			throw new NotImplementedException();
		}

		public void MarkCompleted(Guid id)
		{
			throw new NotImplementedException();
		}

		private void ValidateString(string str)
		{
			if (str != string.Empty && str[0] == ' ')
				throw new ArgumentException("Задача не может начинаться со знака пробел");
		}

		private void CommandDuplicateTask(string task)
		{
			for (int i = 0; i < _listTask.Count; i++)
			{
				if (task == _listTask[i].Name)
					throw new DuplicateTaskException(task);
			}
		}

		public IReadOnlyList<ToDoItem> ShowTacks()
		{
			return _listTask;
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

		public void RemoveTask(string command)
		{
			string? str = command.Substring(12);
			int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);

			index--;
			string? task = _listTask[index].Name;
		}

		public void EditTask(string editTasks)
		{
			string? str = Console.ReadLine();
			int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count);
			index -= 1;

			ToDoItem toDoItem = _listTask[index];
			toDoItem.Name = editTasks;
			toDoItem.CreatedAt = DateTime.Now;
		}

		public void TaskCompleted()
		{
			string? str = Console.ReadLine();
			int index = ParseAndValidateInt(str, MIN_COUNT_TASK, _listTask.Count+1);

			_listTask[index-1].State = ToDoItemState.Completed;
		}

		public void ColTask()
		{
			string? index = Console.ReadLine();

			CountTask = ParseAndValidateInt(index, MIN_COUNT_TASK, MAX_COUNT_TASK);
		}
	}
}
