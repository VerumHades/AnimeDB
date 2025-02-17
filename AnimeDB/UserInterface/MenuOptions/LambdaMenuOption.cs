using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface.MenuOptions
{
    /// <summary>
    /// Menu option that can be activated to run a lambda
    /// </summary>
    public class LambdaMenuOption : MenuOption
    {
        private Action<NestedMenu> action;

        public LambdaMenuOption(string text, string description, Action<NestedMenu> action)
        {
            Text = text;
            Description = description;
            this.action = action;
        }

        public string Text { get; }

        public string Argument => "";

        public string Description { get; set; }

        public void execute(int x, int y, NestedMenu menu)
        {
            action(menu);
        }
    }
}
