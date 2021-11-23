using System;
using Microsoft.Data.Sqlite;

namespace DataBase
{
   

    class Program
    {
        static void Main(string[] args)
        {
            DataBase db = new DataBase();
            Console.WriteLine("Welcome to console interface");
            while (true)
            {
                Console.WriteLine("(S)how, (A)dd, (R)emove, S(e)arch?");
                Console.Write("--> ");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine("");
                if (key.Key == ConsoleKey.Escape)
                    break;
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        Console.WriteLine( db.GetAll() );
                        break;
                    case ConsoleKey.A:
                        db.AddBookInteractive();
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine(db.GetAll());
                        Console.Write("Select item to remove: ");
                        if (db.RemoveBook(Console.ReadLine()) == true)
                            Console.WriteLine("Removed");
                        break;
                    case ConsoleKey.E:
                        Console.Write("Search query: ");
                        string s = Console.ReadLine();
                        Console.WriteLine(db.GetAll(s));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
