using System;
using System.Collections.Generic;
using System.Text;
using Scanner;

namespace Parser
{
    public class TokenMismatchException : Exception
    {
        //public TokenType ExpectedType;
        //public Token ActualToken;
        //public TokenMismatchException(Token actual, TokenType expected = TokenType.unknown) {
        //    this.ActualToken = actual;
        //    this.ExpectedType = expected;
        //    string s = $"Unexpected token {actual.type} at line {actual.lineNr}, character {actual.startPos}.";
        //    if (expected != TokenType.unknown)
        //        s += $" Expected token was {expected}.";
        //    //this.Message = s;
        //}
        public TokenMismatchException() {}
        public TokenMismatchException(string message) : base(message) {}
        public TokenMismatchException(string message, Exception inner) : base(message, inner) {}
    }
    public interface IParser
    {
        public INode Parse();
    }
    public class Parser : IParser
    {
        public ITokenator scanner;
        public Token current;
        public List<string> classes = new List<string>();
        public List<string> functions = new List<string>();
        public Parser(ITokenator scanner)
        {
            this.scanner = scanner;
            current = this.scanner.GetNextToken();
        }
        public INode Parse()
        {
            List<INode> defs = new List<INode>();
            INode def;
            do
            {
                def = TryParseClassDef();
                if (def == null)
                    def = TryParseFunDef();
                if (def != null)
                    defs.Add(def);
                //Console.WriteLine(def);
            } while (def != null);
            return new ProgramNode(defs);
        }

        public INode TryParseClassDef()
        {
            if (current.type != TokenType.class_def)
                return null;
            List<INode> nodes = new List<INode>();
            MatchNextToken(TokenType.id);
            string name = current.stringVal;
            MatchNextToken(TokenType.open_brace);
            current = scanner.GetNextToken();
            INode node;
            do
            {
                node = TryParseVarDef(name + '.');
                if (node == null)
                    node = TryParseFunDef(name + '.');
                if (node != null)
                    nodes.Add(node);
            } while (node != null) ;
            MatchCurrentTokenAndConsume(TokenType.close_brace);
            if (classes.Contains(name)) throw new Exception($"Class {name} already defined");
            classes.Add(name);
            return new ClassNode(name, nodes);
        }
        public INode TryParseVarDef(string prefix = "")
        {
            if (current.type != TokenType.id)
                return null;
            string varName = prefix + current.stringVal;
            MatchNextToken(TokenType.assign);
            current = scanner.GetNextToken();
            IExpression exp = ParseExpression();
            return new AssignExpression("=", new ValueRef(varName), exp);
        }
        public INode TryParseFunDef(string prefix = "")
        {
            if (current.type != TokenType.fun_def)
                return null;
            MatchNextToken(TokenType.id);
            string name = prefix + current.stringVal;
            current = scanner.GetNextToken();
            var args = ParseFunArgs();
            var body = ParseFunBody();
            if (functions.Contains(name+args.Count)) throw new Exception($"Function {name} with {args.Count} arguments already defined");
            functions.Add(name+args.Count);
            return new FunNode(name, args, body);
        }
        public List<string> ParseFunArgs()
        {
            MatchCurrentTokenAndConsume(TokenType.open_par);
            var args = new List<string>();
            while (current.type == TokenType.id)
            {
                args.Add(current.stringVal);
                current = scanner.GetNextToken();                
                if (current.type == TokenType.comma)
                    MatchNextToken(TokenType.id);
            }
            MatchCurrentTokenAndConsume(TokenType.close_par);
            return args;
        }
        public List<INode> ParseFunBody()
        {
            MatchCurrentTokenAndConsume(TokenType.open_brace);
            List<INode> nodes = ParseBlockBody();
            INode ret = TryParseReturn();
            if (ret != null)
                nodes.Add(ret);
            MatchCurrentTokenAndConsume(TokenType.close_brace);
            return nodes;
        }
        public List<INode> ParseBlock()
        {
            MatchCurrentTokenAndConsume(TokenType.open_brace);
            List<INode> nodes = ParseBlockBody();
            MatchCurrentTokenAndConsume(TokenType.close_brace);
            return nodes;
        }
        public List<INode> ParseBlockBody()
        {
            List<INode> nodes = new List<INode>();
            INode node;
            do
            {
                node = TryParseLine();
                if (node == null)
                    node = TryParseIf();
                if (node == null)
                    node = TryParseWhile();
                if (node == null)
                    node = TryParseFor();
                //switch (current.type)
                //{
                //    case TokenType.if_token: break;
                //    case TokenType.while_token: break;
                //    case TokenType.for_token: break;
                //    case TokenType.id: node = ParseLine(); break;

                //    //default:  ParseExpression(); break;
                //    default: throw new TokenMismatchException($"Unexpected token {current.type} at line {current.lineNr}, character {current.startPos}");
                //}
                if (node!=null)
                    nodes.Add(node);
            }while (node != null) ;
            return nodes;
        }
        public INode TryParseLine()
        {
            var exp = TryParseValueRefOrFunCall();
            if (exp == null)
                return null;
            if (exp is ValueRef varRef)
            {
                if(isAssignOp(current.type))
                {
                    var op = current.stringVal;
                    current = scanner.GetNextToken();
                    var right = ParseExpression();
                    exp = new AssignExpression(op, varRef, right);
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
        public INode TryParseFor()
        {
            if (current.type != TokenType.for_token)
                return null;
            current = scanner.GetNextToken();
            string it = string.Empty;
            IExpression from, to;
            if (current.type == TokenType.id)
            {
                it = current.stringVal;
                MatchNextToken(TokenType.assign);
                current = scanner.GetNextToken();
            }
            from = ParseExpression();
            if (from == null)
                ThrowExpressionExpectedErr();
            MatchCurrentTokenAndConsume(TokenType.colon);
            to = ParseExpression();
            if (to == null)
                ThrowExpressionExpectedErr();
            return new ForNode(it, from, to, ParseBlock());
        }
        public INode TryParseIf()
        {
            if (current.type != TokenType.if_token)
                return null;
            //MatchNextToken(TokenType.open_par);
            current = scanner.GetNextToken();
            IExpression cond = ParseLogicExpression();
            if (cond == null)
                ThrowExpressionExpectedErr();
            //MatchCurrentTokenAndConsume(TokenType.close_par);
            return new IfNode(cond, ParseBlock());
        }
        public INode TryParseWhile()
        {
            if (current.type != TokenType.while_token)
                return null;
            //MatchNextToken(TokenType.open_par);
            current = scanner.GetNextToken();
            IExpression cond = ParseLogicExpression();
            if (cond == null)
                ThrowExpressionExpectedErr();
            //MatchCurrentTokenAndConsume(TokenType.close_par);
            return new WhileNode(cond, ParseBlock());
        }
        public INode TryParseReturn()
        {
            if (current.type != TokenType.return_token)
                return null;
            current = scanner.GetNextToken();
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
                current = scanner.GetNextToken();
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
                current = scanner.GetNextToken();
                var right = ParsePrimaryExpression();
                if (right == null)
                    throw new TokenMismatchException($"Missing expression after {op} operator at line {current.lineNr}, character {current.startPos}");
                left = new MultiplicativeExpression(op, left, right);
            }
            return left;
        }
        public IExpression ParseLogicExpression()
        {
            IExpression left = ParseRelativeExpression();
            if (left == null)
                return null;
            while (isConjunctionOp(current.type))
            {
                string op = current.stringVal;
                current = scanner.GetNextToken();
                var right = ParseRelativeExpression();
                if (right == null)
                    throw new TokenMismatchException($"Missing expression after {op} operator at line {current.lineNr}, character {current.startPos}");
                left = new LogicExpression(op, left, right);
            }
            return left;
        }
        public IExpression ParseRelativeExpression()
        {
            IExpression left = ParseExpression();
            if (left == null)
                return null;
            if (isRelativeOp(current.type))
            {
                string op = current.stringVal;
                current = scanner.GetNextToken();
                var right = ParseExpression();
                if (right == null)
                    throw new TokenMismatchException($"Missing expression after {op} operator at line {current.lineNr}, character {current.startPos}");
                left = new RelativeExpression(op, left, right);
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
        public bool isConjunctionOp(TokenType type)
        {
            switch (type)
            {
                case TokenType.and:
                case TokenType.or: return true;
                default: return false;
            }
        }
        public bool isRelativeOp(TokenType type)
        {
            switch (type)
            {
                case TokenType.less:
                case TokenType.less_or_equal:
                case TokenType.greater:
                case TokenType.greater_or_equal:
                case TokenType.equal:
                case TokenType.not_equal: return true;
                default: return false;
            }
        }
        public IExpression ParsePrimaryExpression()
        {
            int modifier = 1;
            bool changemod = false;
            while (current.type == TokenType.minus)
            {
                changemod = true;
                modifier *= -1;
                current = scanner.GetNextToken();
            }
            IExpression exp;
            switch (current.type)
            {
                case TokenType.open_par:
                    {
                        current = scanner.GetNextToken();
                        exp = ParseExpression();
                        MatchCurrentToken(TokenType.close_par);
                        current = scanner.GetNextToken();
                        break;
                    }
                case TokenType.int_number:
                    {
                        exp = new NumericValue(current.intVal);
                        current = scanner.GetNextToken();
                        break;
                    }
                case TokenType.id:
                    {
                        exp = TryParseValueRefOrFunCall();
                        break;
                    }
                default: exp = null; break;
                //default: throw new TokenMismatchException($"Unexpected token {current.type} at line {current.lineNr}, character {current.startPos}");
            }
            if (exp != null && changemod)
                exp.Mod = modifier;           
            return exp;
        }
        public IExpression TryParseValueRefOrFunCall()
        {
            if (current.type != TokenType.id)
                return null;
            StringBuilder sb = new StringBuilder();
            sb.Append(current.stringVal);
            current = scanner.GetNextToken();
            while (current.type == TokenType.dot)
            {
                sb.Append(current.stringVal);
                MatchNextToken(TokenType.id);
                sb.Append(current.stringVal);
                current = scanner.GetNextToken();
            }
            var prs = TryParseFunCallParams();
            if (prs == null)
                return new ValueRef(sb.ToString());
            return new FunCall(sb.ToString(), prs);
        }
        public List<IExpression> TryParseFunCallParams()
        {                    
            if (current.type != TokenType.open_par)
                return null;
            current = scanner.GetNextToken();
            var list = new List<IExpression>();
            IExpression exp;
            bool comma;
            do
            {
                comma = false;
                if (current.type == TokenType.string_const) //string dopuszczalny tylko w wywołaniach funkcji
                {
                    exp = new StringValue(current.stringVal);
                    current = scanner.GetNextToken();
                }
                else exp = ParseExpression();
                if (current.type == TokenType.comma)
                {
                    current = scanner.GetNextToken();
                    comma = true;
                }
                if (exp != null)
                    list.Add(exp);
            } while (comma);
            MatchCurrentTokenAndConsume(TokenType.close_par);          
            return list;
        }
        public void MatchCurrentToken(TokenType type)
        {
            if (current.type == type)
                return; //true;
            throw new TokenMismatchException($"Expected {type} token at line {current.lineNr}, character {current.startPos}, but found {current.type} instead.");
            //return false;
        }
        public void MatchCurrentTokenAndConsume(TokenType type)
        {
            MatchCurrentToken(type);
            current = scanner.GetNextToken();
        }
        public void MatchNextToken(TokenType type)
        {
            current = scanner.GetNextToken();
            MatchCurrentToken(type);
        }
        public void ThrowExpressionExpectedErr()
        {
            throw new TokenMismatchException($"Missing expression at line {current.lineNr}, character {current.startPos}. Found {current.type} token.");
        }
    }
}
