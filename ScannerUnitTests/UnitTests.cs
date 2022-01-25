using NUnit.Framework;
using System.IO;
using System.Text;

namespace ScannerUnitTests
{
    public class Tests
    {
        //string name;
        //FileStream file;
        private FileStream ResetFile(string name)
        {
            if (File.Exists(name))
                File.Delete(name);
            return File.Create(name);
        }
        private void WriteToFile(FileStream file, string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            file.Write(bytes, 0, bytes.Length);
            file.Position = 0;
        }
        [SetUp]
        public void Setup()
        {
            //name = "test.txt";
        }

        [Test]
        public void white_space()
        {
            var name = "white_space" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, " ");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.white_space)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void tabulator()
        {
            var name = "tabulator" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "\t");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.tabulator)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void end_of_line()
        {
            var name = "end_of_line" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, System.Environment.NewLine);
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.end_of_line)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void end_of_file()
        {
            var name = "end_of_file" + ".txt";
            var file = ResetFile(name);
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.end_of_file)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void id()
        {
            var name = "id" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "some134_id2");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.id)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void fun_def()
        {
            var name = "fun_def" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "def fun1 (a,b)");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.fun_def)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void class_def()
        {
            var name = "class_def" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "class motor");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.class_def)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void open_brace()
        {
            var name = "open_brace" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "{");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.open_brace)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void close_brace()
        {
            var name = "close_brace" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "}");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.close_brace)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void open_par()
        {
            var name = "open_par" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "(");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.open_par)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void close_par()
        {
            var name = "close_par" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, ")");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.close_par)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void return_token()
        {
            var name = "return_token" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "return to monke");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.return_token)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void if_token()
        {
            var name = "if_token" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "if u love something");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.if_token)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void while_token()
        {
            var name = "while_token" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "while waiting for better tomorrow");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.while_token)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void for_token()
        {
            var name = "for_token" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "for the motherland");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.for_token)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void comment()
        {
            var name = "comment" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "# just normal comment, keep scrolling \n");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.comment)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void int_number()
        {
            var name = "int_number" + ".txt";
            var file = ResetFile(name);
            int num = 278566; //some souce
            WriteToFile(file, num.ToString());
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.int_number && t.intVal == num)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void string_const()
        {
            var name = "string_const" + ".txt";
            var file = ResetFile(name);
            string content = "i hope nothing \\t brokes here \\n \\\", like rly";
            string decoded = "i hope nothing \t brokes here \n \", like rly";
            WriteToFile(file, "\"" + content + "\"");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.string_const && t.stringVal == decoded)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void assign()
        {
            var name = "assign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.assign)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void or()
        {
            var name = "or" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "||");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.or)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void and()
        {
            var name = "and" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "&&");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.and)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void negation()
        {
            var name = "negation" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "!prawda");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.negation)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void equal()
        {
            var name = "equal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "==");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.equal)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void not_equal()
        {
            var name = "not_equal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "!=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.not_equal)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void greater()
        {
            var name = "greater" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, ">");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.greater)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void greater_or_equal()
        {
            var name = "greater_or_equal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, ">=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.greater_or_equal)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void less()
        {
            var name = "less" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "<");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.less)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void less_or_equal()
        {
            var name = "less_or_equal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "<=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.less_or_equal)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void add()
        {
            var name = "add" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "+");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.add)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void add_assign()
        {
            var name = "add_assign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "+=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.add_assign)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void minus()
        {
            var name = "minus" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "-69");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.minus)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void minus_assign()
        {
            var name = "minus_assign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "-=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.minus_assign)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void multiply()
        {
            var name = "multiply" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "*");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.multiply)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void multiply_assign()
        {
            var name = "multiply_assign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "*=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.multiply_assign)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void divide()
        {
            var name = "divide" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "/");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.divide)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void divide_assign()
        {
            var name = "divide_assign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "/=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.divide_assign)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void divide_mod()
        {
            var name = "divide_mod" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "%");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.divide_mod)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void divide_mod_assign()
        {
            var name = "divide_mod_assign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "%=");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.divide_mod_assign)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void comma()
        {
            var name = "comma" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, ",");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.comma)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void dot()
        {
            var name = "dot" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, ".");
            var stRead = new StreamReader(file);
            var scan = new Scanner.Scanner(stRead);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.dot)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void Filter_should_ignore_white_spaces_etc()
        {
            var name = "Filter_should_ignore_white_spaces_etc" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "     \n      #adfdgh \n{");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var t = scan.GetNextToken();
            file.Close();
            File.Delete(name);
            if (t.type == Scanner.TokenType.open_brace)
                Assert.Pass();
            else
                Assert.Fail();
        }
    }
}