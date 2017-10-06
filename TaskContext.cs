using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo_list
{
    class TaskContext : DbContext 
    {
        public TaskContext() : base("DbConnetction")
        {

        }

        public DbSet<Todo> Todos { get; set; }
    }
}
