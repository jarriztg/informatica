﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fdprPaint
{
    class Program
    {
        static void Main(string[] args)
        {
            // barra griga sotto con menu di salvataggio con coordinate, valore personalizzato e colore personalizzato usa serialize per scirvere proprio i caratteri 
            int x = 0;
            int y = 0;
            int max_x = Console.WindowWidth - 1;
            int max_y = Console.WindowHeight - 2;
            string path;
            string prompt;
            bool charread = false;
            char[,] mappa = new char[max_x + 1, max_y + 1];
            Console.TreatControlCAsInput = true;

            char matita = '\0';
            ConsoleKeyInfo input = new ConsoleKeyInfo('W', ConsoleKey.W, false, false, false);

            printmenu(new string[] { "^X: Exit", "^C Change char", "^L Load", "^O Save", "x: " + Console.CursorLeft + ", y: " + Console.CursorTop }, true);

            while (!(input.Key == ConsoleKey.X && input.Modifiers == ConsoleModifiers.Control))
            {
                input = Console.ReadKey(true);

                if (input.Modifiers == ConsoleModifiers.Control)
                {
                    switch (input.Key)
                    {
                        case ConsoleKey.C:
                            
                            int left = Console.CursorLeft;
                            int top = Console.CursorTop;
                            while (charread == false) {

                                try {
                                    matita = Convert.ToChar(readmenu("Input your char: ", 1));
                                    charread = true;
                                } catch(Exception) {
                                    charread = false;
                                    printmenu(new string[] { "Invalid char. Please try again." }, false);
                                    align();
                                    System.Threading.Thread.Sleep(3000);
                                }
                                Console.SetCursorPosition(left, top);
                            }
                            charread = false;
                            break;
                        case ConsoleKey.L:
                            
                            prompt = "Input the file path: ";
                            path = readmenu(prompt, 10000000);
                            while (!readfromfile(ref mappa, path)) {
                                prompt = "Wrong file path! Input the file path: ";
                                path = readmenu(prompt, 10000000);
                            };
                            break;
                        case ConsoleKey.O:
                            prompt = "Input the destination file path: ";
                            path = readmenu(prompt, 10000000);
                            while (!printtofile(ref mappa, path)) {
                                prompt = "An error occurred! Input the destination file path: ";
                                path = readmenu(prompt, 10000000);
                            };
                            break;
                    }

                } else {
                    switch (input.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            x--;

                            printTo(ref x, ref y, max_x, max_y, matita, input.Key, ref mappa);
                            break;
                        case ConsoleKey.RightArrow:
                            x++;

                            printTo(ref x, ref y, max_x, max_y, matita, input.Key, ref mappa);
                            break;
                        case ConsoleKey.DownArrow:
                            y++;

                            printTo(ref x, ref y, max_x, max_y, matita, input.Key, ref mappa);
                            break;
                        case ConsoleKey.UpArrow:
                            y--;

                            printTo(ref x, ref y, max_x, max_y, matita, input.Key, ref mappa);
                            break;
                    }

                }
                printmenu(new string[] { "^X: Exit", "^C Change char", "^L Load", "^O Save", "x: " + Console.CursorLeft + ", y: " + Console.CursorTop }, true);

            }
        }

        static void align()
        {

            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(left, top);
        }

        static bool printtofile(ref char[,] mappa, string dest)
        {

            // x is width (oriz), y is height (vert), index 0 is x, index 1 is y
            string result = ""; 
            try
            {
                if(Directory.Exists(@dest)) {
                    printmenu(new string[] { "Path is a directory. Specify another path." }, false);
                    align();
                    System.Threading.Thread.Sleep(3000);
                    return false;
                } else if(File.Exists(@dest)) {
                    do
                    {
                        result = readmenu("File exists. Overwrite (y/n)?", 1);
                    } while (result != "y" && result != "n");
                    if (result == "n")  { return false; } else { System.IO.File.WriteAllText (@dest, ""); };
                }
                
                
                System.IO.StreamWriter file = new System.IO.StreamWriter(@dest);

                for (int yy = 0; yy < mappa.GetLength(1); yy++)
                {
                    for (int xx = 0; xx < mappa.GetLength(0); xx++)
                    {
                        if (mappa[xx, yy] == '\0')
                        {
                            mappa[xx, yy] = ' ';
                        }
                        file.Write(mappa[xx, yy]);
                    }
                    file.WriteLine();
                }
                file.Close();
            } catch(Exception) {
                return false;
            }
            

            return true;
        }


        static bool readfromfile(ref char[,] mappa, string dest)
        {

            // x is width (oriz), y is height (vert), index 0 is x, index 1 is y
            Console.Clear();

            try
            {
                if (Directory.Exists(@dest))
                {
                    printmenu(new string[] { "Path is a directory. Specify another path." }, false);
                    align();
                    System.Threading.Thread.Sleep(3000);
                    return false;
                }
                else if (!File.Exists(@dest))
                {
                    printmenu(new string[] { "File does not exist. Specify another path." }, false);
                    align();
                    System.Threading.Thread.Sleep(3000);
                    return false;
                }
                int width = 0;
                int height = 0;
                using (var reader = File.OpenText(@dest))
                {
                    string curline = reader.ReadLine();
                    while (curline != null)
                    {
                        height++;
                        if (curline.Length > width) width = curline.Length;
                        curline = reader.ReadLine();
                    }
                }

                if (height > Console.LargestWindowHeight)
                {
                    height = Console.LargestWindowHeight;
                }
                if (width > Console.LargestWindowWidth)
                {
                    width = Console.LargestWindowWidth;
                }

                Console.SetWindowSize(width, height);

                using (var reader = File.OpenText(@dest))
                {

                    int curheight = 0;
                    int curwidth = 0;
                    string curline = reader.ReadLine();
                    while (curline != null)
                    {
                        curheight++;
                        if (curheight < height)
                        {
                            for (int x = 0; x < curline.Length; x++)
                            {
                                curwidth++;
                                if (curwidth < width)
                                {
                                    Console.Write(curline[x]);
                                }
                            }
                            Console.WriteLine();

                            curwidth = 0;
                        }
                        curline = reader.ReadLine();
                    }
                }
                Console.SetCursorPosition(0, 0);

            }
            catch (Exception)
            {
                return false;
            }


            return true;
        }


        static string readmenu(string wut, int dove)
        {
            string input = "";
            char inputchar;

            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            printmenu(new string[] { wut }, false);

            align();
            for (int x = 1; x <= dove; x++)
            {
                inputchar = Console.ReadKey().KeyChar;
                if (inputchar == '\r')
                {
                    x = dove + 1;
                }
                else
                {
                    if(inputchar != '\r') input += inputchar.ToString();
                }
            }

            Console.SetCursorPosition(left, top);
            Console.ResetColor();
            return input;
        }
        static void printmenu(string[] elements, bool reset)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            for (int x = 0; x < Console.WindowWidth; x++)
            {
                Console.Write(" ");
            }

            int space = Console.WindowWidth;
            for (int x = 0; x < elements.Length; x++)
            {
                Console.SetCursorPosition(Console.WindowWidth - space, Console.WindowHeight - 1);
                if (elements[x].Length > space / (elements.Length - x))
                {
                    space -= elements[x].Length + 1;
                }
                else space -= space / (elements.Length - x);
                Console.Write(elements[x]);
            }
            if (reset == true)
            {
                Console.ResetColor();
                Console.SetCursorPosition(0, 0);
                Console.SetCursorPosition(left, top);
            }
        }

        static void checkmaxmin(ref int c, int max)
        {
            if(c < 0) {
                c = 0;
            } else if (c > max)
            {
                c--;
            }
        }
        static void printTo(ref int x, ref int y, int max_x, int max_y, char print, ConsoleKey last, ref char[,] mappa)
        {

            checkmaxmin(ref x, max_x);
            checkmaxmin(ref y, max_y);
            mappa[Console.CursorLeft, Console.CursorTop] = print;
            mappa[x, y] = print;

            Console.Write(print);
            Console.SetCursorPosition(x, y);
            Console.Write(print);
            Console.SetCursorPosition(x, y);
        }
    }
}
