using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Reflection;
using ToDoList_Telegram_Bot.Exception;
using ToDoList_Telegram_Bot.Interface;
using ToDoList_Telegram_Bot.Model;

namespace ToDoList_Telegram_Bot
{
	class UpdateHandler : IUpdateHandler
	{
		IUserService? _userService;
		ITelegramBotClient? _botClient;
		IToDoService? _toDoService;
		
		Update? _update;
	
		private bool _flag = true;
		private string _listCommand = "/addtask, /showtasks, /removetask, /edittask, /taskcompleted, /completetask, /delete, /showcompleted, /info, /help, /echo, /exit";
		
		public string ListCommand { get => _listCommand; }


		public UpdateHandler(IUserService userService, IToDoService toDoService)
		{
			_userService = userService;
			_toDoService = toDoService;
		}

		//Метод обратобки команд
		public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
		{
			botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");

			_botClient = botClient;
			_update = update;

			_toDoService?.CommandStart(_botClient, _update);

			while (_flag)
			{				
				try
				{
					if (_toDoService?.CountTask == 0)
						_toDoService?.CommandColTask(_botClient, _update);

					botClient.SendMessage(update.Message.Chat, $"Доступные команды: {ListCommand}");

					botClient.SendMessage(update.Message.Chat, "Введите команду: ");
					string? command = Console.ReadLine();
					if (command != null)
						switch (command)
						{
							case var _ when command.StartsWith("/addtask "):
								_toDoService?.CommandAddTack(_botClient, _update, command);
								break;
							case "/showtasks":
								_toDoService?.CommandShowTacks(_botClient, _update);
								break;
							case var _ when command.StartsWith("/removetask "):
								_toDoService?.CommandRemoveTask(_botClient, _update, command);
								break;
							case "/edittask":
								_toDoService?.CommandEditTask(_botClient, _update);
								break;
							case "/taskcompleted":
								_toDoService?.CommandTaskCompleted(_botClient, _update);
								break;
							case var _ when command.StartsWith("/completetask "): //"/completetask":
								string id = command.Substring(14);
								_toDoService?.MarkAsCopleted(Guid.Parse(id), _botClient, _update);
								break;
							case var _ when command.StartsWith("/delete "): //"/completetask":
								string str = command.Substring(8);
								_toDoService?.MarkAsCopleted(Guid.Parse(str), _botClient, _update);
								break;
							case "/showcompleted":
								_toDoService?.CommandShowCompleted(_botClient, _update);
								break;
							case "/info":
								_toDoService?.CommandInfo(_botClient, _update);
								break;
							case "/help":
								_toDoService?.CommandHelp(_botClient, _update);
								break;
							case "/exit":
								_flag = _toDoService!.CommandExit();
								break;
							default:
								botClient.SendMessage (update.Message.Chat, "Команда введена не правильно.");
								break;
						}
				}

				catch (ArgumentException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}

				catch (TaskCountLimitException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}
				catch (TaskLengthLimitException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}
				catch (DuplicateTaskException e)
				{
					botClient.SendMessage(update.Message.Chat, e.Message);
				}

			}
		}

	}
}
