using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Telegram_Bot.Types
{
    class Message
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public Chat? Chat { get; set; }
        public User? From { get; set; }
    }
}
