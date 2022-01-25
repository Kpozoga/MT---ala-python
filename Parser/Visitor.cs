using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public interface IVisitor
    {
        public void Visit(INode node);
        public void Visit(ProgramNode node);
        public void Visit(ClassNode node);
        public void Visit(FunNode node);
        public void Visit(FunCall node);
        public void Visit(IfNode node);
        public void Visit(ForNode node);
        public void Visit(WhileNode node);
        public void Visit(LogicExpression node);
        public void Visit(RelativeExpression node);
        public void Visit(AdditiveExpression node);
        public void Visit(MultiplicativeExpression node);
        public void Visit(AssignExpression node);
        public void Visit(ReturnNode node);
        public void Visit(ValueRef node);
        public void Visit(NumericValue node);
        public void Visit(StringValue node);
    }
    public interface IVisitable
    {
        public void Accept(IVisitor visitor);
    }
}
