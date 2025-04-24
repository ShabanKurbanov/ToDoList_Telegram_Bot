using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Model;
using ToDoList_Telegram_Bot.Types;

namespace ToDoList_Telegram_Bot.Interface
{
    interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);
        IReadOnlyList<ToDoItem> ShowTacks();
		void MarkCompleted(Guid id);
        void Delete(Guid id);
        void RemoveTask(string command);
        void EditTask(string editTasks);
        void TaskCompleted();
        void ColTask();
		public int CountTask { get; set; }

	}
}
