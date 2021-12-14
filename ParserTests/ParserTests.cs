using NUnit.Framework;
using System.IO;
using System.Text;
using Parser;

namespace ParserTests
{
    public class Tests
    {
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
        public void ClassDef()
        {
            var name = "ClassDef" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "class bike {}");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseClassOrFunDef();
            if (node is ClassNode)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }

        [Test]
        public void FunDef()
        {
            var name = "FunDef" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "def fun(v1, v2) {}");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseClassOrFunDef();
            if (node is FunNode)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }
        [Test]
        public void FunCall()
        {
            var name = "FunCall" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "bike_me_to_the_moon() ");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            pars.current = pars.scan.GetNextToken();
            var node = pars.ParseValueRefOrFunCall();
            if (node is Parser.FunCall)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }

        [Test]
        public void ClassDefWithAttributes()
        {
            var name = "ClassDefWithAttributes" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "class bike { a = 1 b = 3}");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseClassOrFunDef();
            if (node is ClassNode n && n.Children[0] is AssignExpression)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }

        [Test]
        public void ClassDefWithMethods()
        {
            var name = "ClassDefWithMethods" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "class bike { def met1(arg1) {} }");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseClassOrFunDef();
            if (node is ClassNode n && n.Children[0] is FunNode)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }

        [Test]
        public void NumericVal()
        {
            var name = "NumericVal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "69");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            pars.current = pars.scan.GetNextToken();
            var node = pars.ParsePrimaryExpression();
            if (node is NumericValue)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }

        [Test]
        public void StringConstNotRecognisedBeyondFunCall()
        {
            var name = "StringConstNotRecognisedBeyondFunCall" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "\"ala ma kota\"");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            pars.current = pars.scan.GetNextToken();
            try{
                var node = pars.ParsePrimaryExpression();
            }catch (TokenMismatchException e) { Assert.Pass(); }
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }

        [Test]
        public void VarRef()
        {
            var name = "VarRef" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "a.b.c ");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseValueRefOrFunCall();
            if (node is ValueRef)
                Assert.Pass();
            else
                Assert.Fail();
            file.Close();
            File.Delete(name);
        }
    }
}