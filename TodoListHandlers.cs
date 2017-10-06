using System;
using System.Linq;
using System.Windows.Forms;

namespace todo_list
{
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
