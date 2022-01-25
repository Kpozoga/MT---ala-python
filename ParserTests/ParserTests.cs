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
            var node = pars.TryParseClassDef();
            file.Close();
            File.Delete(name);
            if (node is ClassNode)
                Assert.Pass();
            else
                Assert.Fail();
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
            var node = pars.TryParseFunDef();
            file.Close();
            File.Delete(name);
            if (node is FunNode)
                Assert.Pass();
            else
                Assert.Fail();
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
            var node = pars.TryParseValueRefOrFunCall();
            file.Close();
            File.Delete(name);
            if (node is Parser.FunCall)
                Assert.Pass();
            else
                Assert.Fail();
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
            var node = pars.TryParseClassDef();
            file.Close();
            File.Delete(name);
            if (node is ClassNode n && n.Children[0] is AssignExpression)
                Assert.Pass();
            else
                Assert.Fail();
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
            var node = pars.TryParseClassDef();
            file.Close();
            File.Delete(name);
            if (node is ClassNode n && n.Children[0] is FunNode)
                Assert.Pass();
            else
                Assert.Fail();
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
            var node = pars.ParsePrimaryExpression();
            file.Close();
            File.Delete(name);
            if (node is NumericValue)
                Assert.Pass();
            else
                Assert.Fail();
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
            pars.current = pars.scanner.GetNextToken();
            var node = pars.ParsePrimaryExpression();
            file.Close();
            File.Delete(name);
            if (node == null)
                Assert.Pass();
            else Assert.Fail();
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
            var node = pars.TryParseValueRefOrFunCall();
            file.Close();
            File.Delete(name);
            if (node is ValueRef)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void FunCallParams()
        {
            var name = "FunCallParams" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "(14, \"string\", var_ref)");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var funParams = pars.TryParseFunCallParams();
            file.Close();
            File.Delete(name);
            if (funParams[0] is NumericValue && funParams[1] is StringValue && funParams[2] is ValueRef)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void MultExpression()
        {
            var name = "MultExpression" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "1*a*5");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var exp = pars.ParseMultExpression();
            file.Close();
            File.Delete(name);
            if (exp is MultiplicativeExpression e && e.right is NumericValue && e.left is MultiplicativeExpression)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void AddExpression()
        {
            var name = "AddExpression" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "1 + 25 - 10");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var exp = pars.ParseExpression();
            file.Close();
            File.Delete(name);
            if (exp is AdditiveExpression e && e.right is NumericValue && e.left is AdditiveExpression && e.op == "-")
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void Return()
        {
            var name = "Return" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "return 13");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseReturn();
            file.Close();
            File.Delete(name);
            if (node is ReturnNode n && n.Children[0] is NumericValue)
                Assert.Pass();
            else
                Assert.Fail();
        }


        [Test]
        public void LineAssign()
        {
            var name = "LineAssign" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "a = 1");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseLine();
            file.Close();
            File.Delete(name);
            if (node is AssignExpression n && n.left is ValueRef && n.right is NumericValue)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void LineFunCall()
        {
            var name = "LineFunCall" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "line_call(1,2,3)");
            var stRead = new StreamReader(file);
            Scanner.ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseLine();
            file.Close();
            File.Delete(name);
            if (node is FunCall n && n.funParams.Count == 3)
                Assert.Pass();
            else
                Assert.Fail();
        }
    }
}