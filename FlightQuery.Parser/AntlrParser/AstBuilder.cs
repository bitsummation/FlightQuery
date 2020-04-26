using Antlr4.Runtime;
using FlightQuery.Sdk.SqlAst;
using static FlightQuery.Parser.AntlrParser.SqlParser;

namespace FlightQuery.Parser.AntlrParser
{
    internal class AstBuilder : SqlParserBaseVisitor<Element>
    {
        private ParseInfo CreateParseInfo(ParserRuleContext context)
        {
            return new ParseInfo(context.start.Line, context.start.Column);
        }

        public override Element VisitProgram(SqlParser.ProgramContext context)
        {
            var program = new ProgramElement(CreateParseInfo(context));
            for (var i = 0; i < context.ChildCount; i++)
                program.Children.Add(Visit(context.GetChild(i)));

            return program;
        }

        public override Element VisitQueryStatementExp(SqlParser.QueryStatementExpContext context)
        {
            var query = new QueryStatement(CreateParseInfo(context));

            query.Children.Add(Visit(context.s));
            query.Children.Add(Visit(context.f));
            if(context.w != null)
                query.Children.Add(Visit(context.w));

            return query;
        }   

        public override Element VisitSelectStatementExp(SqlParser.SelectStatementExpContext context)
        {
            var select = new SelectStatement(CreateParseInfo(context));

            for (var i = 0; i < context.ChildCount; i++)
            {
                var child = Visit(context.GetChild(i));
                if (child != null)
                    select.Children.Add(child);
            }

            return select;
        }

        public override Element VisitSelectStarStatementExp(SqlParser.SelectStarStatementExpContext context)
        {
            return new SelectStatement(CreateParseInfo(context)) {All = true };
        }

        public override Element VisitFromStatementExp(SqlParser.FromStatementExpContext context)
        {
            var from = new FromStatement(CreateParseInfo(context)) { Name = context.t != null ? context.t.Text : null, Alias = context.a != null ? context.a.Text : null };
            if(context.j != null)
                from.Children.Add(Visit(context.j));

            return from;
        }

        public override Element VisitFromNestedQueryExp(FromNestedQueryExpContext context)
        {
            var from = new NestedFromStatement(CreateParseInfo(context)) { Alias = context.a != null ? context.a.Text : null };
            from.Children.Add(Visit(context.q));
            if (context.j != null)
                from.Children.Add(Visit(context.j));

            return from;
        }

        public override Element VisitInnerJoinStatementExp(SqlParser.InnerJoinStatementExpContext context)
        {
            var join = new InnerJoinStatement(CreateParseInfo(context)) { Name = context.t == null ? "" : context.t.Text, Alias = context.a != null ? context.a.Text : null } ;
            if(context.b != null)
                join.Children.Add(Visit(context.b));
            if (context.j != null)
                join.Children.Add(Visit(context.j));

            return join;
        }

        public override Element VisitWhereStatementExp(SqlParser.WhereStatementExpContext context)
        {
            var whereStatement = new WhereStatement(CreateParseInfo(context));
            whereStatement.Children.Add(Visit(context.b));

            return whereStatement;
        }

        public override Element VisitSelectArgsExp(SqlParser.SelectArgsExpContext context)
        {
            var selectArg = new SelectArgExpression(CreateParseInfo(context));
            
            if (context.a != null)
            {
                selectArg.Children.Add(new AsExpression(CreateParseInfo(context)) { Alias = context.a.Text});
            }

            selectArg.Children.Add(Visit(context.GetChild(0)));

            return selectArg;
        }

        public override Element VisitCaseStatementExp(SqlParser.CaseStatementExpContext context)
        {
            var caseStatement = new CaseStatement(CreateParseInfo(context));

            int c = 1;
            var child = context.GetChild(c);
            while(child is WhenExpressionExpContext)
            {
                caseStatement.Children.Add(Visit(child));
                c++;
                child = context.GetChild(c);
            }
           
            if(context.s != null)
            {
                caseStatement.Children.Add(Visit(context.s));
            }

            return caseStatement;
        }

        public override Element VisitWhenExpressionExp(WhenExpressionExpContext context)
        {
            var whenExpresion = new WhenExpression(CreateParseInfo(context));
            whenExpresion.Children.Add(Visit(context.b));
            whenExpresion.Children.Add(Visit(context.s));

            return whenExpresion;
        }

        public override Element VisitSelectVariableIdExp(SqlParser.SelectVariableIdExpContext context)
        {
            return new SingleVariableExpression(CreateParseInfo(context)) { Id = context.GetText() };
        }

        public override Element VisitSelectVariableMemberExp(SqlParser.SelectVariableMemberExpContext context)
        {
            return new MemberVariableExpression(CreateParseInfo(context)) { Alias = context.GetChild(0).GetText(), Id = context.GetChild(2).GetText() };
        }

        public override Element VisitBoolOperatorStatementExp(SqlParser.BoolOperatorStatementExpContext context)
        {
            var returnElement = Visit(context.l);
            if(context.o != null)
            {
                var o = Visit(context.o) as BooleanExpression;
                o.Left = returnElement;
                returnElement = o;
                o.Right = Visit(context.r);
            }

            return returnElement;
        }

        public override Element VisitBoolStatementExp(SqlParser.BoolStatementExpContext context)
        {
            var left = Visit(context.l);
            int x = 2;
            while (x < context.ChildCount)
            {
                var right = Visit(context.GetChild(x));
                left = new OrExpression(CreateParseInfo(context)) { Left = left, Right = right };
                x += 2;
            }

            return left;
        }

        public override Element VisitAndStatementExp(SqlParser.AndStatementExpContext context)
        {
            var left = Visit(context.l);
            int x = 2;
            while(x < context.ChildCount)
            {
                var right = Visit(context.GetChild(x));
                left = new AndExpression(CreateParseInfo(context)) { Left = left, Right = right };
                x += 2;
            }
            
            return left;
        }

        public override Element VisitBoolTermParenStatementExpr(SqlParser.BoolTermParenStatementExprContext context)
        {
            return Visit(context.b);
        }

        public override Element VisitNotEqualStatementExp(SqlParser.NotEqualStatementExpContext context)
        {
            return new NotEqualsExpression(CreateParseInfo(context));
        }

        public override Element VisitEqualsStatmentExp(SqlParser.EqualsStatmentExpContext context)
        {
            return new EqualsExpression(CreateParseInfo(context));
        }

        public override Element VisitGreaterThanStatementExp(SqlParser.GreaterThanStatementExpContext context)
        {
            return new GreaterThanExpression(CreateParseInfo(context));
        }

        public override Element VisitLessThanStatementExp(SqlParser.LessThanStatementExpContext context)
        {
            return new LessThanExpression(CreateParseInfo(context));
        }

        public override Element VisitIntegerExp(SqlParser.IntegerExpContext context)
        {
            return new LongLiteral(CreateParseInfo(context)) { Value = long.Parse(context.GetText()) };
        }

        public override Element VisitStringLiteralExp(SqlParser.StringLiteralExpContext context)
        {
            return new StringLiteral(CreateParseInfo(context)) { Value = context.GetText().Replace("'", "").Replace("\"", "")};
        }
    }
}
