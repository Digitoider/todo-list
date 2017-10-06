using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace todo_list
{
    class Menu
    {
        List<Item> items = new List<Item>();
        private int activeItem;
        public string HelpMessage { get; set; }

        public Func<int> HandleEscape { get; internal set; }

        public void AddItem(string item, ItemEventHandler handleEnter, ItemEventHandler handleEscape)
        {
            items.Add(new Item(item, handleEnter, handleEscape));
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void RemoveItemById(int id)
        {
            var itemToRemove = FindItemById(id);
            if(itemToRemove != null)
            {
                items.Remove(itemToRemove);
                activeItem--;
                if (activeItem < 0)
                {
                    activeItem = 0;
                }
            }
        }

        public Item FindItemById(int id)
        {
            Item itemToReturn = null;
            foreach (var item in items)
            {
                if (item.Id == id)
                {
                    itemToReturn = item;
                    break;
                }
            }
            return itemToReturn;
        }

        public void RenameItemById(int id, string name)
        {
            Item itemToRename = FindItemById(id);
            if (itemToRename != null)
            {
                itemToRename.Name = name;
            }
        }

        public void Activate()
        {
            if (items.Count == 0) return;
            activeItem = 0;
            Redraw();
            bool isActive = true;
            while (isActive)
            {
                var pressedKey = Console.ReadKey().Key;
                switch (pressedKey)
                {
                    case ConsoleKey.DownArrow: NextItem(); break;
                    case ConsoleKey.UpArrow: PrevItem(); break;
                    case ConsoleKey.Enter:
                        items[activeItem].HandleEnter();
                        Redraw();
                        break;
                    case ConsoleKey.Escape: isActive = false; break;
                    default: break;
                }
            }
            
        }

        public void NextItem()
        {
            if(items.Count-1 == activeItem)
            {
                activeItem = 0;
            }
            else
            {
                activeItem++;
            }
            Redraw();
        }

        public void PrevItem()
        {
            if(activeItem == 0)
            {
                activeItem = items.Count - 1;
            }
            else
            {
                activeItem--;
            }
            Redraw();
        }

        private void Redraw()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            if (!String.IsNullOrEmpty(HelpMessage))
            {
                Console.WriteLine(HelpMessage);
            }
            
            foreach(var item in items)
            {
                if (item == items[activeItem])
                {
                    Console.WriteLine("> " + item.Name);
                }
                else
                {
                    Console.WriteLine("  " + item.Name);
                }
                
            }
        }
    }

    public delegate void ItemEventHandler();

    class Item 
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public ItemEventHandler HandleEnter;
        public ItemEventHandler HandleEscape;
        public Item(string name, ItemEventHandler handleEnter, ItemEventHandler handleEscape)
        {
            Name = name;
            this.HandleEnter = handleEnter;
            this.HandleEscape = handleEscape;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
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
    }

    class TodoListHandlers
    {
        public void showList()
        {
            using (var db = new TaskContext())
            {
                var list = new Menu();
                list.HelpMessage = "Нажмите ESC, чтобы вернуться в меню";
                list.HandleEscape = () => { return 0; };
                foreach(var todo in db.Todos)
                {
                    list.AddItem(todo.Task, () => { }, () => { return; });
                }
                list.Activate();
            }
        }
        public void editItem()
        {
            using (var db = new TaskContext())
            {
                var list = new Menu();
                list.HelpMessage = "Нажмите ESC для возврата в МЕНЮ. Для редактирования записи нажмите ENTER";
                list.HandleEscape = () => {  return 0; };
                foreach (var todo in db.Todos)
                {
                    ItemEventHandler handleEnter = () => {
                        int id = todo.Id;

                        int y = db.Todos.Count();
                        Console.SetCursorPosition(0, ++y);

                        Console.Write("Редактирование записи: ");
                        string dataToEdit = todo.Task;
                        
                        SendKeys.SendWait(dataToEdit);

                        var updatedTask = Console.ReadLine();

                        todo.Task = updatedTask;
                        db.SaveChanges();
                        list.RenameItemById(id, updatedTask);
                    };
                    ItemEventHandler handleEscape = () => { return; };
                    var item = new Item(todo.Task, handleEnter, handleEscape);
                    item.Id = todo.Id;
                    list.AddItem(item);
                }
                list.Activate();
            }


            Console.WriteLine("Editing list isn't done yet");
        }
        
        public void deleteItem()
        {
            using (var db = new TaskContext())
            {
                var list = new Menu();
                list.HelpMessage = "Нажмите ESC для возврата в МЕНЮ. Для удаления записи нажмите ENTER";
                list.HandleEscape = () => { return 0; };
                foreach (var todo in db.Todos)
                {
                    ItemEventHandler handleEnter = () => {
                        int id = todo.Id;
                        db.Todos.Remove(todo);
                        db.SaveChanges();
                        list.RemoveItemById(id);
                    };
                    ItemEventHandler handleEscape = () => { return; };
                    var item = new Item(todo.Task, handleEnter, handleEscape);
                    item.Id = todo.Id;
                    list.AddItem(item);
                }
                list.Activate();
            }
        }
        public void addItem()
        {
            Console.Write("Новая задача: ");
            string task = Console.ReadLine();

            using (var db = new TaskContext())
            {
                db.Todos.Add(new Todo { Task = task });
                db.SaveChanges();
            }
        }
    }
}
