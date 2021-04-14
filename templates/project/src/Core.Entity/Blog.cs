using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class Blog : BaseDB
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
    }
}
