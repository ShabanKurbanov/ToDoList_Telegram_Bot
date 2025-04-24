using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Types;

namespace ToDoList_Telegram_Bot.Model
{
	class UserService : IUserService
	{
		ToDoUser? toDoUser;

		public ToDoUser? GetUser(long telegramUserId)
		{
			return toDoUser;
		}

		public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
		{			
				toDoUser = new ToDoUser();
				toDoUser.TelegramUserId = telegramUserId;
				toDoUser.TelegramUserName = telegramUserName;
				toDoUser.UserId = new Guid();
				toDoUser.RegisteredAt = new DateTime();
				return toDoUser;			
		}
	}
}
