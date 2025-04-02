using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Telegram_Bot.Types
{
    class User
    {
        public long Id { get; set; }
		public string? Username { get; internal set; }
	}
}
