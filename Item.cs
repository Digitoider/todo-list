namespace todo_list
{
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
}
