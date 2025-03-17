using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList_Telegram_Bot
{
    class TaskLengthLimitExeption : Exception
    {
        public TaskLengthLimitExeption(int taskLendth, int taskLengthLimit) : 
            base($"Длина задачи '{taskLendth}' превышает максимально допустимое значение {taskLengthLimit}"){}
    }
}
