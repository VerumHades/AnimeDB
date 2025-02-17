using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface
{
    /// <summary>
    /// A class that handles managing all elements (Usually just stacking prompts tho)
    /// 
    /// And the screen
    /// </summary>
    public class Root
    {

        private List<Element> elements = new() { };
        private Stack<Element> prompts = new();
        private int selected_element = 0;

        public void addElement(Element element)
        {
            elements.Add(element);
        }

        /// <summary>
        /// Open the screen and render
        /// </summary>
        public static Screen? scr = null;
        public void Run()
        {
            while (true)
            {
                Console.CursorVisible = false;

                int dw = Console.WindowWidth - 1;
                int dh = Console.WindowHeight - 1;
                if (scr == null || (scr.Width != dw || scr.Height != dh))
                    scr = new Screen(dw, dh);

                scr.Clear();

                Element? prompt = null;
                if (prompts.Count > 0) prompt = prompts.Peek();

                if (prompt != null)
                {
                    prompt.X = dw / 2 - prompt.Width / 2;
                    prompt.Y = dh / 2 - prompt.Height / 2;
                    prompt.Draw(this, true);
                }
                else
                    for (int i = 0; i < elements.Count; i++) elements[i].Draw(this, i == selected_element);

                scr.Render();

                var key = Console.ReadKey(intercept: true);

                if (prompt != null)
                {
                    prompt.Event(this, key);
                }
                else
                {
                    if (key.Key == ConsoleKey.Tab)
                    {
                        selected_element = (selected_element + 1 + elements.Count) % elements.Count;
                        continue;
                    }

                    if (key.Key == ConsoleKey.Tab && key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        selected_element = (selected_element + 1 + elements.Count) % elements.Count;
                        continue;
                    }

                    if (selected_element >= 0 && selected_element < elements.Count) elements[selected_element].Event(this, key);
                }
            }
            //Console.CursorVisible = true;
        }

        public void OpenPrompt(Element prompt)
        {
            this.prompts.Push(prompt);
         
        }
        public void ClosePrompt()
        {
            if(this.prompts.Count > 0) this.prompts.Pop();
        }
    }
}
