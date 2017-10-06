using System;
using System.Text;
using System.Threading.Tasks;

namespace todo_list
{

    public delegate void ItemEventHandler();

    class Program
    {
        static void Main(string[] args)
        {
            SetupConsole();

            var menu = new Menu();
            menu.HelpMessage = "Нажмите ESC для выхода из программы";
            var todoListhandlers = new TodoListHandlers();
            ItemEventHandler EscapeHandler = () => { return; };
            menu.AddItem("Показать список", todoListhandlers.showList, EscapeHandler);
            menu.AddItem("Добавить задачу в список", todoListhandlers.addItem, EscapeHandler);
            menu.AddItem("Редактировать дело из списка", todoListhandlers.editItem, EscapeHandler);
            menu.AddItem("Удалить дело из списка", todoListhandlers.deleteItem, EscapeHandler);

            menu.Activate();
        }

        private static void SetupConsole()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
    }
}
