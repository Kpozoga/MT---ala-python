using Parser;
using Scanner;
using System;
using System.IO;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1. Czytaj z pliku");
            Console.WriteLine("2. Czytaj z wejścia konsoli");
            var key = Console.ReadKey();
            Console.WriteLine();
            ITokenator scan;
            StreamReader stream = StreamReader.Null;
            if (key.KeyChar == '1')
            {
                Console.WriteLine("Podaj ścieżkę do pliku");
                string path = Console.ReadLine();
                //string path = "file.txt";
                try
                {
                    stream = new StreamReader(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Skaner będzie czytał z konsoli");
                stream = new StreamReader(Console.OpenStandardInput());
            }
            scan = new Scanner.Scanner(stream);
            scan = new Filter(scan);
            IParser pars = new Parser.Parser(scan);
            var head = pars.Parse();
            //printTree(head);
            Console.WriteLine();
            IVisitor interpreter = new Interpreter();
            try {
            interpreter.Visit(head); }
            catch(Exception e) { Console.WriteLine(e.Message); }
            Console.ReadKey();
        }

        static void printTree(INode node, string pf = "")
        {
            Console.WriteLine(pf + node);
            if (node.Children == null) return;
            foreach (var c in node.Children)
                printTree(c, pf + "     ");

        }
    }
}
