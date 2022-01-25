using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public abstract class INode : IVisitable
    {
        public List<INode> Children { set; get; }

        public abstract void Accept(IVisitor visitor);
    }
    public abstract class IExpression : INode
    {
        int mod = 1;
        public virtual int Mod { set { mod = value; } get { return mod; } }
    }
    public class AdditiveExpression : IExpression
    {
        public string op;
        public IExpression left; //{get{return children.len>0? children[0]:null;} set{children[0]=value;}}
        public IExpression right;
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
        public override string ToString()
        {
            return $"Additive expression {op}";// + left.ToString() + "\n" + right.ToString();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class MultiplicativeExpression : IExpression
    {
        public string op;
        public IExpression left;
        public IExpression right;
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
        public override string ToString()
        {
            return $"Multiplicative expression {op}";// + left.ToString() + "\n" + right.ToString();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class RelativeExpression : IExpression
    {
        public string op;
        public IExpression left;
        public IExpression right;
        public RelativeExpression(string op, IExpression left, IExpression right)
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

        public override int Mod { set {/*exception?*/ } get { return 1; } }
        public override string ToString()
        {
            return $"Relative expression {op}";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class LogicExpression : IExpression
    {
        public string op;
        public IExpression left;
        public IExpression right;
        public LogicExpression(string op, IExpression left, IExpression right)
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

        //public override int Mod { set { throw new Exception("minus not applicable for logic expression"); } get { return 1; } }
        public override string ToString()
        {
            return $"Logic expression {op}";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class AssignExpression : IExpression
    {
        public string op;
        public ValueRef left;
        public IExpression right;
        public AssignExpression(string op, ValueRef left, IExpression right)
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
        public override string ToString()
        {
            return $"Assign expression {op}";// + left.ToString() + "\n" + right.ToString();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class NumericValue : IExpression
    {
        public int val;

        public NumericValue(int val)
        {
            this.val = val;
        }
        public override string ToString()
        {
            var m = Mod == -1 ? "-" : "";
            return $"Numeric value: {m}{val}";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class StringValue : IExpression
    {
        public string val;
        public StringValue(string val)
        {
            this.val = val;
        }
        public override string ToString()
        {
            return $"String const: \"{val}\"";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class ValueRef : IExpression
    {
        public string name;
        public ValueRef(string val)
        {
            this.name = val;
        }
        public override string ToString()
        {
            var m = Mod == -1 ? "-" : "";
            return $"Variable reference: {m}{name}";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class FunCall : IExpression
    {
        public string name;
        public List<IExpression> funParams;
        public FunCall(string name, List<IExpression> funParams)
        {
            this.name = name;
            this.funParams = funParams;
        }
        public override string ToString()
        {
            var m = Mod == -1 ? "-" : "";
            var s = $"Function call: {m}{name}() with params: ";
            foreach (var p in funParams)
            { s += (p.ToString() + ", "); }
            return s;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class ReturnNode : INode
    {
        public ReturnNode(IExpression exp)
        {
            this.Children = new List<INode>();
            if (exp is INode n)
                this.Children.Add(n);
        }

        public override string ToString()
        {
            return $"Return";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class ClassNode : INode
    {
        public string name;
        public ClassNode(string name, List<INode> list)
        {
            this.name = name;
            this.Children = list;
        }

        public override string ToString()
        {
            return $"Class {name}";
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
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
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class FunNode : INode
    {
        public string name;
        public List<string> args;
        public bool HasReturn()
        {
            if (Children.Count > 0 && Children[Children.Count - 1] is ReturnNode)
                return true;
            else return false;
        }
        public FunNode(string name, List<string> args, List<INode> list)
        {
            this.Children = list;
            this.name = name;
            this.args = args;
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder($"FunctionDef {name}, args: ");
            foreach (string arg in args)
            {
                s.Append(arg);
                s.Append(", ");
            }
            return s.ToString().TrimEnd().TrimEnd(',');
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class ForNode : INode
    {
        public string iterator;
        public IExpression from;
        public IExpression to;
        public ForNode(string iterator, IExpression from, IExpression to, List<INode> list)
        {
            this.Children = list;
            this.iterator = iterator;
            this.from = from;
            this.to = to;
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder($"For {from} : {to}");
            if (iterator != null)
                s.Append($", iterator: {iterator}");
            return s.ToString();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class IfNode : INode
    {
        public IExpression cond;
        public IfNode(IExpression cond, List<INode> list)
        {
            this.Children = list;
            this.cond = cond;
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder($"If {cond}");
            return s.ToString();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class WhileNode : INode
    {
        public IExpression cond;
        public WhileNode(IExpression cond, List<INode> list)
        {
            this.Children = list;
            this.cond = cond;
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder($"While {cond}");
            return s.ToString();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}