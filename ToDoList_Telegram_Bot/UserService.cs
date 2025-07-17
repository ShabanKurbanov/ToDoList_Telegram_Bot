using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Model;

namespace ToDoList_Telegram_Bot
{
	class UserService : IUserService
	{
		private readonly Dictionary<long, ToDoUser> _users = new Dictionary<long, ToDoUser>(); 

		public ToDoUser? GetUser(long telegramUserId)
		{
			_users.TryGetValue(telegramUserId, out var user);
			return user!;
		}

		public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
		{
			if (!_users.ContainsKey(telegramUserId))
			{
				var user = new ToDoUser
				{
					TelegramUserId = telegramUserId,
					TelegramUserName = telegramUserName
				};
				_users[telegramUserId] = user;
			}
			return _users[telegramUserId];
		}
	}
}
