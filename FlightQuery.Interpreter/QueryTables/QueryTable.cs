using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.QueryTables
{
    public abstract class QueryTable : TableBase
    {
        protected QueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(descriptor)
        {
            HttpExecutor = httpExecutor;
            QueryArgs = new VariableContainer<QueryArgs>();
        }

        protected IHttpExecutor HttpExecutor { get; private set; }

        public VariableContainer<QueryArgs> QueryArgs { get; private set; }

        public override bool HasExecuted { get { return false; } }
        protected abstract string TableName { get; }

        public sealed override void AddArg(QueryArgs args)
        {
            QueryArgs.Add(args);
        }

        public sealed override ExecutedTable Execute(LimitStatement statement)
        {
            LimitQuery(statement);
            ValidateArgs();

            var args = new HttpExecuteArg() {
                Variables = QueryArgs.Args.Select(x => new HttpQueryVariabels() {Variable = x.Variable, Value = x.PropertyValue.Value.ToString()  }),
                TableName = TableName
            };

            return ExecuteCore(args);
        }

        protected abstract ExecutedTable ExecuteCore(HttpExecuteArg args);

        protected virtual void LimitQuery(LimitStatement statement)
        {
        }

        private void ValidateRequired()
        {
            var required = Descriptor.RequiredProperties;
            var missingParams = new List<string[]>();
            
            foreach (var key in required.Keys) //look in each group and make sure each is there
            {
                var missingRequiredParams = required[key].Select(x => x.Name).Except(QueryArgs.Args.Select(x => x.Variable)).ToArray();
                if (missingRequiredParams.Length == 0) // all required params in group are there.
                {
                    missingParams = new List<string[]>();
                    break;
                }

                if (missingRequiredParams.Length > 0)
                    missingParams.Add(missingRequiredParams);
            }

            if (missingParams.Count > 0)
            {
                foreach (var param in missingParams.First())
                    Errors.Add(new RequiredMissingParameter(param));
            }
        }

        protected virtual void ValidateArgs()
        {
            ValidateRequired();

            //Convert Datetime Args to unix time
            foreach (var param in QueryArgs.Args.Where(x => x.PropertyValue.Value != null).Where(x => x.PropertyValue.Value.GetType() == typeof(DateTime)))
            {
                string key = param.PropertyValue.Value.GetType().Name + "-" + typeof(long).Name;
                var converstion = Conversion.Map[key](param.PropertyValue.Value);
                param.PropertyValue = new PropertyValue(converstion);
            }
        }
    
    }
}
