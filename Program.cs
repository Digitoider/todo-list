using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo_list
{
    class Menu
    {
        List<Item> items = new List<Item>();
        private int activeItem;

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
            Item itemToRemove = null;
            foreach(var item in items)
            {
                if(item.Id == id)
                {
                    itemToRemove = item;
                    break;
                }
            }
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
            var todoListhandlers = new TodoListHandlers();
            //menu.HandleEscape = () => { return 0; };
            menu.AddItem("Показать список", todoListhandlers.showList, () => { return; });
            menu.AddItem("Добавить задачу в список", todoListhandlers.addItem, () => { return; });
            menu.AddItem("Редактировать дело из списка", todoListhandlers.editItem, () => { return; });
            menu.AddItem("Удалить дело из списка", todoListhandlers.deleteItem, () => { return; });

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
                list.HandleEscape = () => { return 0; };
                foreach(var todo in db.Todos)
                {
                    list.AddItem(todo.Task, () => { }, () => { return; });
                }
                list.Activate();
            }
                //Console.WriteLine("Displaing list isn't done yet");
        }
        public void editItem()
        {
            Console.WriteLine("Editing list isn't done yet");
        }
        public void deleteItem()
        {
            using (var db = new TaskContext())
            {
                var list = new Menu();
                list.HandleEscape = () => { return 0; };

                //for (int i = 0; i < db.Todos.Count(); i++)
                //{
                //    var todo = db.Todos.ToArray()[i];
                //    list.AddItem(todo.Task, () => {
                //        db.Todos.Remove(todo);
                //        db.SaveChanges();
                //        /*TODO удаление из списка list для перерисовки*/
                //        list.RemoveItemAt(i);
                //    }, () => { return; });
                //}

                foreach (var todo in db.Todos)
                {
                    ItemEventHandler handleEnter = () => {
                        int id = todo.Id;
                        db.Todos.Remove(todo);
                        db.SaveChanges();
                        /*TODO удаление из списка list для перерисовки*/
                        list.RemoveItemById(id);
                    };
                    ItemEventHandler handleEscape = () => { return; };
                    var item = new Item(todo.Task, handleEnter, handleEscape);
                    item.Id = todo.Id;
                    list.AddItem(item);
                }
                list.Activate();
            }
            Console.WriteLine("Deleting list isn't done yet");
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
