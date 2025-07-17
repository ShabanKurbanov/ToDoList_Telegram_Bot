using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_Telegram_Bot.Model;

namespace ToDoList_Telegram_Bot.Interface
{
	internal interface IToDoService
	{
		IReadOnlyList<ToDoItem> GetAllByUserId(long userId);
		//Возвращает ToDoItem для UserId co статусом Active
		IReadOnlyList<ToDoItem> GetActiveByUserId(long userId);
		ToDoItem Add(ToDoUser user, string name);
		void MarkAsCopleted(Guid id, ITelegramBotClient _botClient, Update _update);
		void Delete(Guid id, ITelegramBotClient _botClient, Update _update);

		void CommandAddTack(ITelegramBotClient _botClient, Update _update, string command);
		void CommandShowTacks(ITelegramBotClient _botClient, Update _update);
		void CommandRemoveTask(ITelegramBotClient _botClient, Update _update, string command);
		void CommandEditTask(ITelegramBotClient _botClient, Update _update);
		void CommandTaskCompleted(ITelegramBotClient _botClient, Update _update);
		void CommandShowCompleted(ITelegramBotClient _botClient, Update _update);
		void CommandInfo(ITelegramBotClient _botClient, Update _update);
		void CommandHelp(ITelegramBotClient _botClient, Update _update);
		bool CommandExit();
		void CommandStart(ITelegramBotClient _botClient, Update _update);
		void CommandColTask(ITelegramBotClient _botClient, Update _update);
		
		int CountTask { get; set; }
	}
}
