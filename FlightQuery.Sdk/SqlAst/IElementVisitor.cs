namespace FlightQuery.Sdk.SqlAst
{
    public interface IElementVisitor
    {
        void Visit(ProgramElement element);
        void Visit(QueryStatement statement);
        void Visit(SelectStatement statement);
        void Visit(FromStatement statement);
        void Visit(WhereStatement statement);
        void Visit(InnerJoinStatement statement);

        void Visit(SelectArgExpression expression);
        void Visit(CaseStatement statement);
        void Visit(WhenExpression expression);

        void Visit(AndExpression expression);
        void Visit(OrExpression expression);
        void Visit(EqualsExpression expression);
        void Visit(NotEqualsExpression expression);
        void Visit(GreaterThanExpression expression);
        void Visit(LessThanExpression expression);
        
        void Visit(MemberVariableExpression expression);
        void Visit(SingleVariableExpression expression);

        void Visit(LongLiteral literal);
        void Visit(StringLiteral literal);
    }
}
