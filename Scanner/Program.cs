using System;

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
            if (key.KeyChar=='1')
            {
                Console.WriteLine("Podaj ścieżkę do pliku");
                scan = new Filter(Console.ReadLine());
            }
            else
            {
                Console.WriteLine("Skaner będzie czytał z konsoli");
                scan = new Filter(Console.OpenStandardInput());
            }
            
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
