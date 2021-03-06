using System;
using System.Text;
using System.IO;

namespace Scanner
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
            if (key.KeyChar=='1')
            {
                Console.WriteLine("Podaj ścieżkę do pliku");
                // using stream
                string path = Console.ReadLine();
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
            scan = new Scanner(stream);
            scan = new Filter(scan);
            Token t = new Token();
            while (t.type != TokenType.end_of_file) 
            {
                t = scan.GetNextToken();
                Console.WriteLine(t.type);
            }
            Console.ReadKey();
        }
    }
}
