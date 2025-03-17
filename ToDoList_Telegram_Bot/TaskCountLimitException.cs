﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Telegram_Bot
{
    class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач равное {taskCountLimit}")
        {
        }
    }
}
