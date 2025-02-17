using AnimeDB.UserInterface.MenuOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface
{
    /// <summary>
    /// A class for a menu element with options that can be navigated with arrows or 'w' and 's' and 'enter' and 'space' to confirm
    /// </summary>
    public class NestedMenu: Element
    {
        private List<(int,int)> positions = new();
        private List<MenuOption> options = new();
        private int selected_option = 0;

        public NestedMenu(List<MenuOption> options, int selected_option)
        {
            this.options = options;
            this.positions = new(options.Count);
            this.selected_option = selected_option;
        }

        override public void Event(Root root, ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) selected_option = (selected_option + options.Count - 1) % options.Count;
            else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) selected_option = (selected_option + options.Count + 1) % options.Count;
            else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                if (selected_option >= 0 && selected_option < options.Count)
                {
                    options[selected_option].execute(positions[selected_option].Item1, positions[selected_option].Item2, this);
                }
            }
        }
        
        override public void Draw(Root root, bool selected)
        {
            base.Draw(root, selected);

            int offset = 0;
            for (int i = 0; i < options.Count; i++)
            {
                string line = $" {(i == selected_option ? ">" : " ")} {options[i].Text}";

                DrawTextWrapped(Width / 2, offset, options[i].Argument, Width / 2 - 4);
                DrawText(2, offset++, line);

                if(positions.Count <= i) positions.Add((Width / 2, offset));
                else positions[i] = ( Width / 2, offset );
            }
        }
    }
}
