using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter : IElementVisitor
    {
        private SelectTable _selectResult;
        private Element _program;
        private Scope _scope;
        private Stack<QueryPhaseArgs> _visitStack;
        private IHttpExecutor _httpExecutor;
        private bool _intellisense;

        public ErrorsCollection Errors { get; private set; }
        public ScopeModel ScopeModel { get; set; }

        public Interpreter(Element root, string authorization) : this(root, authorization, ExecuteFlags.Run, new HttpExecutor(new HttpExecutorRaw(authorization))) { } 

        public Interpreter(Element root, string authorization, ExecuteFlags flags, IHttpExecutor httpExecutor)
        {
            _program = root;
            _scope = new Scope();
            _visitStack = new Stack<QueryPhaseArgs>();
            _httpExecutor = httpExecutor ?? new HttpExecutor(new HttpExecutorRaw(authorization));
            _intellisense = ((flags & ExecuteFlags.Intellisense) == ExecuteFlags.Intellisense);

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

        private void ValidateComparisionType(QueryArgs arg)
        {
            if (arg.Property.Type != arg.PropertyValue.Value.GetType())
            {
                string key = arg.PropertyValue.Value.GetType().Name + "-" + arg.Property.Type.Name;
                var converstion = Conversion.Map[key](arg.PropertyValue.Value);
                arg.PropertyValue = new PropertyValue(converstion);
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
