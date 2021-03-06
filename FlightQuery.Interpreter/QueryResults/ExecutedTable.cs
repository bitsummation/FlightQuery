﻿
using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.QueryResults
{
    public class ExecutedTable : TableBase
    {
        public ExecutedTable(TableDescriptor descriptor) : base(descriptor)
        { }

        public override bool HasExecuted { get { return true; } }

        public override void AddArg(QueryArgs args)
        {
        }

        public override TableBase Create()
        {
            return this;
        }

        public override ExecutedTable Execute(LimitStatement statement)
        {
            return this;
        }
    }
}
