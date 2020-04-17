using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter : IElementVisitor
    {
        private SelectTable _selectResult;
        private Element _program;
        private Scope _scope;
        private Stack<QueryPhaseArgs> _visitStack;
        private IHttpExecutor _httpExecutor;

        public ErrorsCollection Errors { get; private set; }
        public ScopeModel ScopeModel { get; set; }

        public Interpreter(Element root, string authorization) : this(root, authorization, new HttpExecutor(new HttpExecutorRaw(authorization))) { } 

        public Interpreter(Element root, string authorization, IHttpExecutor httpExecutor)
        {
            _program = root;
            _scope = new Scope();
            _visitStack = new Stack<QueryPhaseArgs>();
            _httpExecutor = httpExecutor ?? new HttpExecutor(new HttpExecutorRaw(authorization));

            Errors = new ErrorsCollection();
        }

        private QueryPhaseArgs VisitChild(Element node, QueryPhaseArgs arg)
        {
            if (node != null)
            {
                _visitStack.Push(arg);
                node.Accept(this);
                return _visitStack.Pop();
            }

            return null;
        }

        private QueryPhaseArgs VisitChild(Element node)
        {
            return VisitChild(node, new QueryPhaseArgs());
        }

        private void VisitWhereIgnoreErrors(WhereStatement statement)
        {
            // visit where to gather query variables.
            // We ignore errors as the where statement is visited in the Query
            var errors = Errors.ToArray();
            VisitChild(statement);
            Errors.Clear();
            Array.ForEach(errors, x => Errors.Add(x));
        }

        private void ValidateComparisionType(QueryArgs arg)
        {
            if (arg.Property.Type != arg.PropertyValue.Value.GetType())
            {
                string key = arg.PropertyValue.Value.GetType().Name + "-" + arg.Property.Type.Name;
                Func<object, object> map;
                if (!Conversion.Map.ContainsKey(key)) //can't convert.
                {
                    map = Conversion.NoOp;
                }
                else
                    map = Conversion.Map[key];

                var conversion = map(arg.PropertyValue.Value);
                arg.PropertyValue = new PropertyValue(conversion);
            }
        }

        private bool ValidateTypes(QueryArgs leftArg, QueryArgs rightArg, Func<PropertyValue, PropertyValue, bool> comparer)
        {
            if (leftArg.HasValue && rightArg.HasValue) //both values
            {
                if (leftArg.PropertyValue.Value.GetType() != rightArg.PropertyValue.Value.GetType())
                {
                    QueryArgs queryValue;
                    string propertyType; 
                    if(leftArg.HasProperty)
                    {
                        queryValue = rightArg;
                        propertyType = leftArg.Property.Type.Name;
                    }
                    else
                    {
                        queryValue = leftArg;
                        propertyType = rightArg.Property.Type.Name;
                    }

                    string key = queryValue.PropertyValue.Value.GetType().Name + "-" + propertyType;
                    var converstion = Conversion.Map[key](queryValue.PropertyValue.Value);
                    queryValue.PropertyValue = new PropertyValue(converstion);
                }

                return comparer(leftArg.PropertyValue, rightArg.PropertyValue);
            }
            else if (leftArg.HasValue && rightArg.HasProperty) //Give Property Value For Query
            {
                rightArg.LeftProperty = false;
                rightArg.PropertyValue = leftArg.PropertyValue;
                ValidateComparisionType(rightArg);
            }
            else if (leftArg.HasProperty && rightArg.HasValue) //Give Property Value for Query
            {
                leftArg.LeftProperty = true;
                leftArg.PropertyValue = rightArg.PropertyValue;
                ValidateComparisionType(leftArg);
            }
            //else //Two Variables, no Value
              //  throw new InvalidOperationException("Two variables with no value");

            return false;
        }

        public SelectTable Execute()
        {
            VisitChild(_program);

            return _selectResult;
        }
    }
}
