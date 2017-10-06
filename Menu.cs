using System;
using System.Collections.Generic;

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
                Console.WriteLine("  {0}\n", HelpMessage);
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
}
