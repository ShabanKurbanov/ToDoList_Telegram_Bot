using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Types;

namespace ToDoList_Telegram_Bot.Model
{
    class ToDoItem
    {
        public Guid? Id { get; set; }
        public ToDoUser? User { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ToDoItemState? State { get; set; }
        public DateTime? StateChangedAt { get; set; }

    }
}
