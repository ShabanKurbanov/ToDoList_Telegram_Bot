﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Telegram_Bot.Model
{
	internal class ToDoItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public ToDoUser? User { get; set; }

		public string? Name { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ToDoItemState State { get; set; } = ToDoItemState.Active;

		public DateTime StateChangedAt { get; set; }
	}
}
