using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using System;
using System.Linq;

namespace FlightQuery.Interpreter.QueryResults
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

        public override void AddArg(QueryArgs args)
        {
            QueryArgs.Add(args);
        }

        public sealed override ExecutedTable Execute()
        {
            ValidateArgs();

            var args = new HttpExecuteArg() {
                Variables = QueryArgs.Args.Select(x => new HttpQueryVariabels() {Variable = x.Variable, Value = x.PropertyValue.Value.ToString()  }),
                TableName = TableName
            };

            return ExecuteCore(args);
        }

        protected abstract ExecutedTable ExecuteCore(HttpExecuteArg args);

        protected virtual bool ValidateArgs()
        {
            //make sure args that are required are there.
            var missingRequiredParams = Descriptor.RequiredProperties.Select(x => x.Name).Except(QueryArgs.Args.Select(x => x.Variable)).ToArray();
            if (missingRequiredParams.Length > 0)
            {
                foreach(var param in missingRequiredParams)
                    Errors.Add(new RequiredMissingParameter(param));
            }
               
            //Convert Datetime Args to unix time
            foreach (var param in QueryArgs.Args.Where(x => x.PropertyValue.Value != null).Where(x => x.PropertyValue.Value.GetType() == typeof(DateTime)))
            {
                string key = param.PropertyValue.Value.GetType().Name + "-" + typeof(long).Name;
                var converstion = Conversion.Map[key](param.PropertyValue.Value);
                param.PropertyValue = new PropertyValue(converstion);
            }

            return true;
        }
    
    }
}
