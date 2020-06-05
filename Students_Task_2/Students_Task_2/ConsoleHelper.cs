using System;
using System.Collections.Generic;
using System.Text;

namespace Students_Task_2
{
    class ConsoleHelper
    {
        public static int ShowMenu(bool canCancel, params string[] options)
        {
            //setting starting points for our program
            const int startX = 1;
            const int startY = 1;

            //setting the values for our lines
            const int optionsPerLine = 1;
            const int spacingPerLine = 24;

            int currentSelection = 0;

            ConsoleKey key;

            //hide the cursor for not confusing the user
            Console.CursorVisible = false;

            do
            {
                //clearing the Console
                Console.Clear();

                //making sure, the option, where the cursor is at, is marked red
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);

                    if (i == currentSelection)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(options[i]);

                    Console.ResetColor();
                }

                //making sure, the user controls the cursor comfortabely on the menu
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        {
                            if (currentSelection % optionsPerLine > 0)
                                currentSelection--;
                            else
                                currentSelection += optionsPerLine - 1;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (currentSelection % optionsPerLine < optionsPerLine - 1)
                                currentSelection++;
                            else
                                currentSelection -= optionsPerLine - 1;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= optionsPerLine)
                                currentSelection -= optionsPerLine;
                            else
                                currentSelection += options.Length - optionsPerLine;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection + optionsPerLine < options.Length)
                                currentSelection += optionsPerLine;
                            else
                                currentSelection += optionsPerLine - options.Length;
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            if (canCancel)
                                return -1;
                            break;
                        }
                }

            } while (key != ConsoleKey.Enter);

            //Making the cursor visible again for further using from the user
            Console.CursorVisible = true;

            //returning the index of the chosen option
            return currentSelection;
        }
    }
}
