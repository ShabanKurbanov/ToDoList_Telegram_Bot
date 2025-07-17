using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Telegram_Bot.Exception
{
    class DuplicateTaskException : IOException 
    {
        public DuplicateTaskException(string task) :base($"Задача '{task}' уже существует") { }
    }
}
