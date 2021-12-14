using System;
using System.Collections.Generic;
using System.Text;
using Scanner;

namespace Parser
{
    public class TokenMismatchException : Exception
    {
        public TokenMismatchException() {}
        public TokenMismatchException(string message) : base(message) {}
        public TokenMismatchException(string message, Exception inner) : base(message, inner) {}
    }
    public abstract class INode
    {
        public List<INode> Children { set; get; }
    }
    public interface IExpression
    {
        public int Mod { set; get; }
    }
    public class AdditiveExpression : INode, IExpression
    {
        string op;
        IExpression left;
        IExpression right;
        public AdditiveExpression(string op, IExpression left, IExpression right)
        {
            this.Children = new List<INode>();
            this.op = op;
            this.left = left;
            this.right = right;
            if (left is INode l)
                this.Children.Add(l);
            if (right is INode r)
                this.Children.Add(r);
        }
        public int Mod { set; get; }
        public override string ToString()
        {
            return $"Additive expression {op}";// + left.ToString() + "\n" + right.ToString();
        }
    }  
    public class MultiplicativeExpression : INode, IExpression
    {
        string op;
        IExpression left;
        IExpression right;
        public MultiplicativeExpression(string op, IExpression left, IExpression right)
        {
            this.Children = new List<INode>();
            this.op = op;
            this.left = left;
            this.right = right;
            if (left is INode l)
                this.Children.Add(l);
            if (right is INode r)
                this.Children.Add(r);
        }

        public int Mod { set; get; }
        public override string ToString()
        {
            return $"Multiplicative expression {op}";// + left.ToString() + "\n" + right.ToString();
        }
    }
    public class DivideExpression : INode, IExpression
    {
        public int Mod { set; get; }
    }
    public class AssignExpression : INode, IExpression
    {
        string op;
        IExpression left;
        IExpression right;
        public AssignExpression(string op, IExpression left, IExpression right)
        {
            this.Children = new List<INode>();
            this.op = op;
            this.left = left;
            this.right = right;
            if (left is INode l)
                this.Children.Add(l);
            if (right is INode r)
                this.Children.Add(r);
        }

        public int Mod { set; get; }
        public override string ToString()
        {
            return $"Assign expression {op}";// + left.ToString() + "\n" + right.ToString();
        }
    }
    public class NumericValue : INode, IExpression
    {
        int val;
        public int Mod { set; get; }

        public NumericValue(int val)
        {
            this.val = val;
        }
        public override string ToString()
        {
            var m = Mod == -1 ? "-" : "";
            return $"Numeric value: {m}{val}";
        }
    }
    public class StringValue : INode, IExpression
    {
        string val;
        public int Mod { set; get; }

        public StringValue(string val)
        {
            this.val = val;
        }
        public override string ToString()
        {
            return $"String const: \"{val}\"";
        }
    }
    public class ValueRef : INode, IExpression
    {
        string name;
        public int Mod { set; get; }

        public ValueRef(string val)
        {
            this.name = val;
        }
        public override string ToString()
        {
            var m = Mod == -1 ? "-" : "";
            return $"Variable reference: {m}{name}";
        }
    }
    public class FunCall : INode, IExpression
    {
        string name;
        List<IExpression> funParams;

        public FunCall(string name, List<IExpression> funParams)
        {
            this.name = name;
            this.funParams = funParams;
        }
        public int Mod { set; get; }
        public override string ToString()
        {
            var m = Mod == -1 ? "-" : "";
            var s = $"Function call: {m}{name}() with params: ";
            foreach(var p in funParams)
            { s += (p.ToString() + ", "); }
            return s; 
        }
    }
    public class ReturnNode : INode
    {
        public ReturnNode(IExpression exp)
        {
            this.Children = new List<INode>();
            if(exp is INode n)
                this.Children.Add(n);
        }

        public override string ToString()
        {
            return $"Return";
        }
    }
    public class ClassNode : INode
    {
        string name;
        public ClassNode(string name, List<INode> list)
        {
            this.name = name;
            this.Children = list;
        }

        public override string ToString()
        {
            return $"Class {name}";
        }
    }
    public class ProgramNode : INode
    {
        public ProgramNode(List<INode> list)
        {
            this.Children = list;
        }

        public override string ToString()
        {
            return "Program";
        }
    }
    public class FunNode : INode
    {
        string name;
        List<string> args;
        public FunNode(string name, List<string> args, List<INode> list)
        {
            this.Children = list;
            this.name = name;
            this.args = args;

        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder(base.ToString() + $" name: {name}, args: ");
            foreach(string arg in args)
            {
                s.Append(arg);
                s.Append(", ");
            }
            return s.ToString().TrimEnd().TrimEnd(',');
        }
    }
    public class Parser
    {
        public ITokenator scan;
        public Token current;
        public Parser(ITokenator scanner)
        {
            scan = scanner;
        }
        public INode Parse()
        {
            List<INode> defs = new List<INode>();
            INode def;
            while ((def = ParseClassOrFunDef())!= null)
            {
                defs.Add(def);
                //Console.WriteLine(def);
                //current = scan.GetNextToken();
            }
            return new ProgramNode(defs);
        }
        public INode ParseClassOrFunDef()
        {
            current = scan.GetNextToken();
            if (current.type == TokenType.class_def)
                return ParseClassDef();
            if (current.type == TokenType.fun_def)
                return ParseFunDef();
            return null;
        }
        public INode ParseClassDef()
        {
            List<INode> nodes = new List<INode>();
            MatchNextToken(TokenType.id);
            string name = current.stringVal;
            MatchNextToken(TokenType.open_brace);
            current = scan.GetNextToken();
            while(current.type!=TokenType.close_brace)
            {
                INode node = null;
                if (current.type==TokenType.id)
                {
                    string varName = current.stringVal;
                    MatchNextToken(TokenType.assign);
                    current = scan.GetNextToken();
                    IExpression exp = ParseExpression();
                    node = new AssignExpression("=", new ValueRef(varName), exp);
                }
                else if (current.type == TokenType.fun_def)
                {
                    node = ParseFunDef();
                }
                if (node != null)
                    nodes.Add(node);
            }
            return new ClassNode(name, nodes);
        }
        public INode ParseFunDef()
        {
            MatchNextToken(TokenType.id);
            string name = current.stringVal;
            var args = ParseFunArgs();
            var body = ParseBlock();
            return new FunNode(name, args, body);
        }
        public List<string> ParseFunArgs()
        {
            MatchNextToken(TokenType.open_par);
            var args = new List<string>();
            current = scan.GetNextToken();
            if (current.type == TokenType.id)
            {
                args.Add(current.stringVal);
                current = scan.GetNextToken();
                while (current.type != TokenType.close_par)
                {
                    MatchCurrentToken(TokenType.comma);
                    MatchNextToken(TokenType.id);
                    args.Add(current.stringVal);
                    current = scan.GetNextToken();
                }
            }
            return args;
        }
        public List<INode> ParseBlock()
        {
            MatchNextToken(TokenType.open_brace);
            current = scan.GetNextToken();
            List<INode> nodes = new List<INode>();
            INode node;
            while (current.type != TokenType.close_brace)
            {
                node = null;
                switch (current.type)
                {
                    case TokenType.if_token: break;
                    case TokenType.while_token: break;
                    case TokenType.for_token: break;
                    case TokenType.id: node = ParseLine(); break;
                    case TokenType.return_token: node = ParseReturn(); break; // chyba złe miejsce na return
                    //default:  ParseExpression(); break;
                    default: throw new TokenMismatchException($"Unexpected token {current.type} at line {current.lineNr}, character {current.startPos}");
                }
                if (node!=null)
                    nodes.Add(node);
            }
            //current = scan.GetNextToken();
            return nodes;
        }
        public INode ParseLine()
        {
            var exp = ParseValueRefOrFunCall();
            if (exp is ValueRef)
            {
                if(isAssignOp(current.type))
                {
                    var op = current.stringVal;
                    current = scan.GetNextToken();
                    var right = ParseExpression();
                    exp = new AssignExpression(op, exp, right);
                }
                else
                {
                    //throw new Exception($"Unused value ref at line {current.lineNr}, character {current.startPos}");
                }
            }
            if (exp is INode n)
                return n;
            else return null;
        }
        public INode ParseReturn()
        {
            current = scan.GetNextToken();
            var exp = ParseExpression();
            return new ReturnNode(exp);
        }
        public IExpression ParseExpression()
        {
            IExpression left = ParseMultExpression();
            if (left == null)
                return null;
            while (isAdditiveOp(current.type))
            {
                string op = current.stringVal;
                current = scan.GetNextToken();
                var right = ParseMultExpression();
                if (right == null)
                    throw new TokenMismatchException($"Missing expression after {op} operator at line {current.lineNr}, character {current.startPos}");
                left = new AdditiveExpression(op, left, right);
            }
            return left;
        }
        public IExpression ParseMultExpression()
        {
            IExpression left = ParsePrimaryExpression();
            if (left == null)
                return null;
            while (isMultiplicativeOp(current.type)) 
            {
                string op = current.stringVal;
                current = scan.GetNextToken();
                var right = ParsePrimaryExpression();
                if (right == null)
                    throw new TokenMismatchException($"Missing expression after {op} operator at line {current.lineNr}, character {current.startPos}");
                left = new MultiplicativeExpression(op, left, right);
            }
            return left;
        }
        public bool isMultiplicativeOp(TokenType type)
        {
            switch(type)
            {
                case TokenType.multiply:
                case TokenType.divide:
                case TokenType.divide_mod: return true;
                default: return false;
            }    
        }
        public bool isAdditiveOp(TokenType type)
        {
            switch (type)
            {
                case TokenType.add:
                case TokenType.minus: return true;
                default: return false;
            }
        }
        public bool isAssignOp(TokenType type)
        {
            switch (type)
            {
                case TokenType.assign:
                case TokenType.add_assign:
                case TokenType.minus_assign:
                case TokenType.multiply_assign:
                case TokenType.divide_assign:
                case TokenType.divide_mod_assign: return true;
                default: return false;
            }
        }
        public IExpression ParsePrimaryExpression()
        {
            int modifier = 1;
            if (current.type == TokenType.minus)
            {
                modifier = -1;
                current = scan.GetNextToken();
            }
            IExpression exp;
            switch (current.type)
            {
                case TokenType.open_par:
                    {
                        current = scan.GetNextToken();
                        exp = ParseExpression();
                        MatchCurrentToken(TokenType.close_par);
                        current = scan.GetNextToken();
                        break;
                    }
                case TokenType.int_number:
                    {
                        exp = new NumericValue(current.intVal);
                        current = scan.GetNextToken();
                        break;
                    }
                case TokenType.id:
                    {
                        exp = ParseValueRefOrFunCall();
                        break;
                    }
                default: throw new TokenMismatchException($"Unexpected token {current.type} at line {current.lineNr}, character {current.startPos}");
            }
            exp.Mod = modifier;           
            return exp;
        }
        public IExpression ParseValueRefOrFunCall()
        {
            StringBuilder sb = new StringBuilder(current.stringVal);
            current = scan.GetNextToken();
            while (current.type == TokenType.dot)
            {
                sb.Append(current.stringVal);
                MatchNextToken(TokenType.id);
                sb.Append(current.stringVal);
                current = scan.GetNextToken();
            }
            if (current.type == TokenType.open_par)
            {
                var pars = ParseFunCallParams();
                return new FunCall(sb.ToString(), pars);
            }
            else return new ValueRef(sb.ToString());
        }
        public List<IExpression> ParseFunCallParams()
        {
            var list = new List<IExpression>();
            MatchCurrentToken(TokenType.open_par);
            current = scan.GetNextToken();
            bool comma;
            if (current.type != TokenType.close_par)
                do
                {
                    comma = false;
                    if (current.type == TokenType.string_const) //string dopuszczalny tylko w wywołaniach funkcji
                    {
                        list.Add(new StringValue(current.stringVal));
                        current = scan.GetNextToken();
                    }
                    else list.Add(ParseExpression());

                    if (current.type == TokenType.comma)
                    {
                        current = scan.GetNextToken();
                        comma = true;
                    }
                } while (comma);       
            MatchCurrentToken(TokenType.close_par);
            current = scan.GetNextToken();
            return list;
        }
        public void MatchCurrentToken(TokenType type)
        {
            if (current.type == type)
                return; //true;
            throw new TokenMismatchException($"Expected {type} token at line {current.lineNr}, character {current.startPos}, but found {current.type} instead.");
            //return false;
        }
        public void MatchNextToken(TokenType type)
        {
            current = scan.GetNextToken();
            MatchCurrentToken(type);
        }
    }
}
