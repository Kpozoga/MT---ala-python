using NUnit.Framework;
using Parser;
using Scanner;
using System;
using System.IO;
using System.Text;

namespace InterpreterTests
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
            
        }

        [Test]
        public void ClassAttributes()
        {
            var name = "ClassAttributes" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "class bike { a = 1 b = 2 c = 3 }");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseClassDef();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            interpreter.Visit(node);
            file.Close();
            File.Delete(name);
            if (interpreter.global.variables.Count == 1 &&
                interpreter.global["bike"].Attributes.Count == 3)
                Assert.Pass();
            else
                Assert.Fail();
        }
        [Test]
        public void ClassMethods()
        {
            var name = "ClassMethods" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "class bike { def fun1(){} def fun2 () {} }");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseClassDef();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            interpreter.Visit(node);
            file.Close();
            File.Delete(name);
            if (interpreter.functions.Count == 2)
                Assert.Pass();
            else
                Assert.Fail();
        }
        [Test]
        public void FunArgs()
        {
            var name = "FunArgs" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "def fun1(a1, a2 , a3){}");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseFunDef();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            interpreter.Visit(node);
            file.Close();
            File.Delete(name);
            if (interpreter.functions["fun13"].args.Count == 3)
                Assert.Pass();
            else
                Assert.Fail();
        }
        [Test]
        public void FunBody()
        {
            var name = "FunBody" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "def fun1(a1){ a1 = 3 a=a1 a = 4 return a1}");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseFunDef();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            interpreter.Visit(node);
            file.Close();
            File.Delete(name);
            if (interpreter.functions["fun11"].Children.Count == 4 &&
                interpreter.functions["fun11"].HasReturn() == true)
                Assert.Pass();
            else
                Assert.Fail();
        }
        [Test]
        public void FunCallUndefined()
        {
            var name = "FunCallUndefined" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "fun()");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseValueRefOrFunCall();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                
            }
            catch (Exception e) { Assert.Pass(); }
            Assert.Fail();
        }
        [Test]
        public void FunCallCorrect()
        {
            var name = "FunCallCorrect" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "def fun() { a=1 } fun()");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var def = pars.TryParseFunDef();
            var call = pars.TryParseValueRefOrFunCall();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(def);
                interpreter.Visit(call);
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }
        }
        [Test]
        public void IfFalse()
        {
            var name = "IfFalse" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "if 2 > 3 { dostuff()}");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseIf();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void ForLoop()
        {
            var name = "ForLoop" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "for i = 1:10 { a = i print(i)}");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseFor();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void WhileLoop()
        {
            var name = "WhileLoop" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "a = 10 while a>0 {a-=1}");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var varDef = pars.TryParseVarDef();
            var node = pars.TryParseWhile();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(varDef);
                interpreter.Visit(node);
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }
        }
        [Test]
        public void LogicExp()
        {
            var name = "LogicExp" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "4 > 3 && 5!=6");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseLogicExpression();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "bool" || val.IntVal <= 0)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void RelExp()
        {
            var name = "RelExp" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "4 > 3");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseLogicExpression();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "bool" || val.IntVal <= 0)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void AddExp()
        {
            var name = "AddExp" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "10 + -4");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseExpression();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "int" || val.IntVal != 6)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void MultExp()
        {
            var name = "MultExp" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "8*-9");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseExpression();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "int" || val.IntVal != -72)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void AssignExp()
        {
            var name = "AssignExp" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "b = 1");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseVarDef();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                if (interpreter.GetCurrentContext()["b"].IntVal != 1)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void Return()
        {
            var name = "Return" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "return 12");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.TryParseReturn();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "int" || val.IntVal != 12)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void ValRef()
        {
            var name = "ValRef" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "var1");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseExpression();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "ref" || val.StringVal != "var1")
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void NumVal()
        {
            var name = "NumVal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "89");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var node = pars.ParseExpression();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(node);
                var val = interpreter.valStack.Pop();
                if (val.Type != "int" || val.IntVal != 89)
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        [Test]
        public void StringVal()
        {
            var name = "StringVal" + ".txt";
            var file = ResetFile(name);
            WriteToFile(file, "(\"ala\")");
            var stRead = new StreamReader(file);
            ITokenator scan = new Scanner.Scanner(stRead);
            scan = new Scanner.Filter(scan);
            var pars = new Parser.Parser(scan);
            var nodes = pars.TryParseFunCallParams();
            var interpreter = new Interpreter.Interpreter();
            interpreter.CreateNewContext();
            file.Close();
            File.Delete(name);
            try
            {
                interpreter.Visit(nodes[0]);
                var val = interpreter.valStack.Pop();
                if (val.Type != "string" || val.StringVal != "ala")
                    Assert.Fail();
                Assert.Pass();
            }
            catch (SuccessException e) { throw; }
            catch (Exception e) { Assert.Fail(); }

        }
        //public void ProgramTest;
    }
}