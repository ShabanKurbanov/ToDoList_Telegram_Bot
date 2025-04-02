using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Types;

namespace ToDoList_Telegram_Bot.Interface
{
    interface IUpdateHandler
    {
        void HandleUpdateAsync(ITelegramBotClient botClient, Update update);
		
	}
}
