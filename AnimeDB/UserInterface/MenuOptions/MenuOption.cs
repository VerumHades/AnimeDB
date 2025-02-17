using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface.MenuOptions
{
    public interface MenuOption
    {
        public string Text { get; }
        public string Argument { get; }

        public string Description { get; set; }

        public void execute(int x, int y, NestedMenu menu);
    }
}
