using Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public class Variable
    {
        public string StringVal;
        public int IntVal;
        public string Type;
        public Dictionary<string, Variable> Attributes = new Dictionary<string, Variable>();
        public Variable(string type) {
            this.Type = type;
        }
        public Variable(string type, int val)
        {
            this.Type = type;
            this.IntVal = val;
        }
        public Variable(string type, string val)
        {
            this.Type = type;
            this.StringVal = val;
        }
        public void Assign(Variable r)
        {
            StringVal = r.StringVal;
            IntVal = r.IntVal;
            Type = r.Type;
            Attributes = new Dictionary<string, Variable>(r.Attributes);
        }
        public Variable GetAttribute(string key)
        {
            var splitted = key.Split('.');
            if (Attributes.ContainsKey(splitted[0]))
                if (splitted.Length == 1)
                    return Attributes[splitted[0]];
                else return Attributes[splitted[0]].GetAttribute(key.Substring(key.IndexOf('.') + 1));
            else return null;
        }
        public void SetAttribute(string key, Variable var)
        {
            var splitted = key.Split('.');
            if (splitted.Length == 1)
                Attributes[key] = var;
            else if (Attributes.ContainsKey(splitted[0]))
                Attributes[splitted[0]].SetAttribute(key.Substring(key.IndexOf('.') + 1), var);
            else throw new Exception($"Variable does not contain attribute {splitted[0]}");
        }

        public override string ToString()
        {
            if (Type == "int")
                return IntVal.ToString();
            else return "class." + Type;
        }
    }
    public class Scope
    {
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public Scope prev;
        public Scope() { }
        public Variable this[string key]
        {
            get {
                var splitted = key.Split('.');
                if (variables.ContainsKey(splitted[0]))
                    if(splitted.Length==1)
                        return variables[key];
                    else return variables[splitted[0]].GetAttribute(key.Substring(key.IndexOf('.') + 1));
                else if (prev != null)
                    return prev[key];
                else return null;
            }
            set {
                var splitted = key.Split('.');
                if (splitted.Length == 1)
                    variables[key] = value;
                else if (variables.ContainsKey(splitted[0]))
                    variables[splitted[0]].SetAttribute(key.Substring(key.IndexOf('.') + 1), value);
                else if (prev != null)
                    prev[key] = value;
                else throw new Exception($"Variable {splitted[0]} not defined in current scope");             
            }
        }
        public bool Contains(string key)
        {
            if (variables.ContainsKey(key))
                return true;
            if (prev != null)
                return prev.Contains(key);
            return false;
        }
    }
    public class Interpreter : IVisitor
    {
        public Dictionary<string, FunNode> functions = new Dictionary<string, FunNode>();
        public Dictionary<string, ClassNode> classes = new Dictionary<string, ClassNode>();
        public Scope global = new Scope();
        public List<Scope> contexts = new List<Scope>();
        public Stack<Variable> valStack = new Stack<Variable>();
        public Scope GetCurrentContext()
        {
            if (contexts.Count == 0)
                return null;
            else return contexts[contexts.Count-1];
        }
        public void CreateNewContext()
        {
            var scope = new Scope();
            scope.prev = global;
            contexts.Add(scope);
        }
        public void ExitCurrentContext()
        {
            if (contexts.Count == 0)
                throw new Exception("Context not set");
            contexts.RemoveAt(contexts.Count-1);
        }
        public void CreateNewScope()
        {
            if (contexts.Count == 0)
                throw new Exception("Context not set");
            var currentcontext = contexts[contexts.Count - 1];
            var newScope = new Scope();
            newScope.prev = currentcontext;
            contexts[contexts.Count - 1] = newScope;
        }
        public void ExitCurrentScope()
        {
            if (contexts.Count == 0)
                throw new Exception("Context not set");
            var currentcontext = contexts[contexts.Count - 1];
            if (currentcontext == null)
                throw new Exception("No scope to exit in current context");
            var prevScope = currentcontext.prev;
            currentcontext.prev = null;
            contexts[contexts.Count - 1] = prevScope;
        }
        public void Print(FunCall node)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < node.funParams.Count; i++)
            {
                Visit(node.funParams[i]);
                var val = valStack.Pop();
                if (val.Type == "string")
                    sb.Append(val.StringVal);
                else if (val.Type == "ref")
                {
                    var varref = GetCurrentContext()[val.StringVal];                    
                    if (varref == null)
                        throw new Exception($"Variable {val.StringVal} not defined in current scope");

                    int mod = val.IntVal;
                    val.Assign(varref);
                    val.IntVal *= mod;

                    sb.Append(val);
                }
                else if (val.Type == "int")
                    sb.Append(val.IntVal);
                else throw new Exception("Unknown error in print function");
            }
            Console.WriteLine(sb.ToString());
        }
        public void Visit(INode node)
        {
            node.Accept(this);
        }
        public void Visit(ProgramNode node)
        {
            CreateNewContext();
            foreach (var child in node.Children)
                Visit(child);
            Visit(new FunCall("main", new List<IExpression>()));
        }

        public void Visit(ClassNode node)
        {
            classes[node.name] = node;
            global[node.name] = new Variable("class");//, node.name);
            foreach (var child in node.Children)
                Visit(child);
        }

        public void Visit(FunNode node)
        {
            functions[node.name + node.args.Count] = node;            
        }

        public void Visit(FunCall node)
        {
            if (node.name=="print")
            {
                Print(node);
                return;
            }
            if (classes.ContainsKey(node.name))
            {
                InvokeConstructor(node);
                return;
            }
            var splitted = node.name.Split('.');
            if (!functions.ContainsKey(node.name + node.funParams.Count))
                if(splitted.Length > 1)
                {
                    InvokeMethod(node);
                    return;
                }
            else throw new Exception($"Function {node.name} accepting {node.funParams.Count} parameters not defined");
            CreateNewContext();
            var con = GetCurrentContext();
            var fun = functions[node.name + node.funParams.Count];
            for (int i = 0; i < node.funParams.Count; i++)
            {
                Visit(node.funParams[i]);
                var val = valStack.Pop();

                if (val.Type == "ref")
                {
                    var varref = contexts[contexts.Count - 2][val.StringVal];
                    if (varref == null)
                        throw new Exception($"No variable {val.StringVal} in current context");
                    con[fun.args[i]] = varref;
                }
                else if (val.Type == "int")
                    con[fun.args[i]] = val;
            }
            foreach (var child in fun.Children)
                Visit(child);
            ExitCurrentContext();
        }

        private void InvokeConstructor(FunCall node)
        {
            string name = $"{node.name}.__init__{node.funParams.Count + 1}";
            if (!functions.ContainsKey(name))
                throw new Exception($"Class {node.name} doesn't contain constructor accepting {node.funParams.Count + 1} parameters");
            var self = new Variable("unknown");
            self.Assign(global[node.name]);
            self.Type = node.name;
            CreateNewContext();
            var con = GetCurrentContext();
            var fun = functions[name];
            con[fun.args[0]] = self;
            for (int i = 0; i < node.funParams.Count; i++)
            {
                Visit(node.funParams[i]);
                var val = valStack.Pop();

                if (val.Type == "ref")
                {
                    var varref = contexts[contexts.Count - 2][val.StringVal];
                    if (varref == null)
                        throw new Exception($"No variable {val.StringVal} in current context");
                    con[fun.args[i + 1]] = varref;
                }
                else if (val.Type == "int")
                    con[fun.args[i + 1]] = val;
            }
            foreach (var child in fun.Children)
                Visit(child);
            ExitCurrentContext();
            valStack.Push(self);
        }

        public void InvokeMethod(FunCall node)
        {
            var localizator = node.name.Substring(0, node.name.LastIndexOf('.'));
            var methodName = node.name.Substring(node.name.LastIndexOf('.') + 1);
            
            var self = GetCurrentContext()[localizator];
            var name = $"{self.Type}.{methodName}{node.funParams.Count + 1}";
            if (!functions.ContainsKey(name))
                throw new Exception($"Class {self.Type} doesn't contain method {methodName} accepting {node.funParams.Count+1} parameters");
            CreateNewContext();
            var con = GetCurrentContext();
            var fun = functions[name];
            con[fun.args[0]] = self;
            for (int i = 0; i < node.funParams.Count; i++)
            {
                Visit(node.funParams[i]);
                var val = valStack.Pop();

                if (val.Type == "ref")
                {
                    var varref = contexts[contexts.Count - 2][val.StringVal];
                    if (varref == null)
                        throw new Exception($"No variable {val.StringVal} in current context");
                    con[fun.args[i+1]] = varref;
                }
                else if (val.Type == "int")
                    con[fun.args[i+1]] = val;
            }
            foreach (var child in fun.Children)
                Visit(child);
            ExitCurrentContext();
        }

        public void Visit(IfNode node)
        {
            Visit(node.cond);
            var cond = valStack.Pop();
            if (cond.Type != "bool") //maybe bool type variable?
                throw new Exception("Expression is not of boolean value");
            if (cond.IntVal > 0)
            {
                CreateNewScope();
                foreach (var child in node.Children)
                    Visit(child);
                ExitCurrentScope();
            }
        }

        public void Visit(ForNode node)
        {
            var con = GetCurrentContext();

            Visit(node.from);
            var from = valStack.Pop();
            if (from.Type == "ref")
            {
                int mod = from.IntVal;
                from.Assign(con[from.StringVal]);
                from.IntVal *= mod;
            }

            Visit(node.to);
            var to = valStack.Pop();
            if (to.Type == "ref")
            {
                int mod = to.IntVal;
                to.Assign(con[to.StringVal]);
                to.IntVal *= mod;
            }

            if (from.Type != "int" || to.Type != "int")
                throw new Exception("Iterator must be of integer value");
            bool explicitIterator = node.iterator != string.Empty;
            while (from.IntVal <= to.IntVal)
            {
                CreateNewScope();
                if (explicitIterator)
                    GetCurrentContext()[node.iterator] = from;
                foreach (var child in node.Children)
                    Visit(child);
                ExitCurrentScope();
                from.IntVal += 1;
            }
        }

        public void Visit(WhileNode node)
        {
            var con = GetCurrentContext();

            Visit(node.cond);
            var cond = valStack.Pop();
            if (cond.Type != "bool") //maybe bool type variable?
                throw new Exception("Expression is not of boolean value");

            while (cond.IntVal>0)
            {
                CreateNewScope();
                foreach (var child in node.Children)
                    Visit(child);
                ExitCurrentScope();
                Visit(node.cond);
                cond = valStack.Pop();
            }
        }

        public void Visit(LogicExpression node)
        {
            var con = GetCurrentContext();

            Visit(node.left);
            var left = valStack.Pop();
            if (left.Type == "ref")
            {
                int mod = left.IntVal;
                left.Assign(con[left.StringVal]);
                left.IntVal *= mod;
            }

            if (left.IntVal > 0 && node.op == "||")
            {
                valStack.Push(left);
                return;
            }
            if (left.IntVal <= 0 && node.op == "&&")
            {
                valStack.Push(left);
                return;
            }

            Visit(node.right);
            var right = valStack.Pop();
            if (right.Type == "ref")
            {
                int mod = right.IntVal;
                right.Assign(con[right.StringVal]);
                right.IntVal *= mod;
            }

            if (left == null || right == null)
                throw new Exception("Variable not defined");
            ValidateOperation(node.op, left.Type, right.Type);

            switch (node.op)
            {
                case "&&": left.IntVal = (left.IntVal > 0) && (right.IntVal > 0) ? 1 : 0; break;
                case "||": left.IntVal = (left.IntVal > 0) || (right.IntVal > 0) ? 1 : 0; break;
                default: throw new Exception("Unexpected operator");
            }
            valStack.Push(left);
        }

        public void Visit(RelativeExpression node)
        {
            var con = GetCurrentContext();

            Visit(node.left);
            var left = valStack.Pop();
            if (left.Type == "ref")
            {
                int mod = left.IntVal;
                left.Assign(con[left.StringVal]);
                left.IntVal *= mod;
            }

            Visit(node.right);
            var right = valStack.Pop();
            if (right.Type == "ref")
            {
                int mod = right.IntVal;
                right.Assign(con[right.StringVal]);
                right.IntVal *= mod;
            }

            if (left == null || right == null)
                throw new Exception("Variable not defined");
            ValidateOperation(node.op, left.Type, right.Type);

            switch (node.op)
            {
                case "<": left.IntVal = left.IntVal < right.IntVal ? 1 : 0; break;
                case "<=": left.IntVal = left.IntVal <= right.IntVal ? 1 : 0; break;
                case ">": left.IntVal = left.IntVal > right.IntVal ? 1 : 0; break;
                case ">=": left.IntVal = left.IntVal >= right.IntVal ? 1 : 0; break;
                case "==": left.IntVal = left.IntVal == right.IntVal ? 1 : 0; break;
                case "!=": left.IntVal = left.IntVal != right.IntVal ? 1 : 0; break;
                default: throw new Exception("Unexpected operator");
            }
            left.Type = "bool";
            valStack.Push(left);
        }

        public void Visit(AdditiveExpression node)
        {
            var con = GetCurrentContext();

            Visit(node.left);
            var left = valStack.Pop();
            if (left.Type == "ref")
            {
                int mod = left.IntVal;
                left.Assign(con[left.StringVal]);
                left.IntVal *= mod;
            }

            Visit(node.right);
            var right = valStack.Pop();
            if (right.Type == "ref")
            {
                int mod = right.IntVal;
                right.Assign(con[right.StringVal]);
                right.IntVal *= mod;
            }

            if (left == null || right == null)
                throw new Exception("Variable not defined");
            ValidateOperation(node.op, left.Type, right.Type);

            switch (node.op)
            {
                case "+": left.IntVal += right.IntVal; break;
                case "-": left.IntVal -= right.IntVal; break;
                default: throw new Exception("Unexpected operator");
            }
            valStack.Push(left);
        }

        public void Visit(MultiplicativeExpression node)
        {
            var con = GetCurrentContext();

            Visit(node.left);
            var left = valStack.Pop();
            if (left.Type == "ref")
            {
                int mod = left.IntVal;
                left.Assign(con[left.StringVal]);
                left.IntVal *= mod;
            }

            Visit(node.right);
            var right = valStack.Pop();
            if (right.Type == "ref")
            {
                int mod = right.IntVal;
                right.Assign(con[right.StringVal]);
                right.IntVal *= mod;
            }

            if (left == null || right == null)
                throw new Exception("Variable not defined");
            ValidateOperation(node.op, left.Type, right.Type);

            switch (node.op)
            {
                case "*": left.IntVal *= right.IntVal; break;
                case "/": left.IntVal /= right.IntVal; break;
                case "%": left.IntVal %= right.IntVal; break;
                default: throw new Exception("Unexpected operator");
            }
            valStack.Push(left);
        }

        public void Visit(AssignExpression node)
        {
            Visit(node.left);
            var left = valStack.Pop();
            if (left.Type == "ref")
            {
                Visit(node.right);
                var right = valStack.Pop();
                var con = GetCurrentContext();
                             
                if (right.Type == "ref")
                    right = con[right.StringVal];

                var varref = con[left.StringVal];
                if (varref == null)
                {
                    varref = new Variable("unknown");
                    con[left.StringVal] = varref;
                }

                ValidateOperation(node.op, varref.Type, right.Type);

                switch (node.op)
                {
                    case "+=": varref.IntVal += right.IntVal; break;
                    case "-=": varref.IntVal -= right.IntVal; break;
                    case "*=": varref.IntVal *= right.IntVal; break;
                    case "/=": varref.IntVal /= right.IntVal; break;
                    case "%=": varref.IntVal %= right.IntVal; break;
                    case "=": {
                            if (right.Type == "int")
                                varref.Assign(right);
                            else con[left.StringVal] = right;
                            //con[left.StringVal] = right;
                            break; 
                        }
                    default: break;
                }

            }
            else throw new Exception("Left side of assign expression must be a variable");
        }

        public void Visit(ReturnNode node)
        {
            Visit(node.Children[0]);
            var child = valStack.Pop();
            if (child.Type == "ref")
            {
                int mod = child.IntVal;
                child.Assign(GetCurrentContext()[child.StringVal]);
                child.IntVal *= mod;
            }
            valStack.Push(child);
        }

        public void Visit(ValueRef node)
        {
            var v = new Variable("ref", node.name);
            v.IntVal = node.Mod;
            valStack.Push(v);
        }

        public void Visit(NumericValue node)
        {
            valStack.Push(new Variable("int", node.val * node.Mod)) ;
        }

        public void Visit(StringValue node)
        {
            valStack.Push(new Variable("string", node.val));
        }
        public void ValidateOperation(string op, string l, string r)
        {
            bool allowed;
            switch (op)
            {
                case "&&":
                case "||":
                    { allowed = (l == "bool" && r == "bool"); break; }
                case "<":
                case "<=":
                case ">":
                case ">=":
                case "==":
                case "!=":
                case "+":
                case "+=":
                case "-":
                case "-=":
                case "*":
                case "*=":
                case "/":
                case "/=":
                case "%":
                case "%=": 
                    { allowed = (l == "int" && r == "int"); break; }
                case "=":
                    { allowed = (l == "unknown" || l == r); break; }
                default: allowed = false; break;
            }
            if (!allowed)
                throw new Exception($"Operation {op} not allowed between types {l} and {r}");
        }
    }
}
