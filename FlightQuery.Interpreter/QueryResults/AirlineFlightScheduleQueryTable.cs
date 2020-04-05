using FlightQuery.Interpreter.Common;
using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlightQuery.Interpreter.QueryResults
{
    public class AirlineFlightScheduleQueryTable : QueryTable
    {
        public AirlineFlightScheduleQueryTable(IHttpExecutor httpExecutor) : base(httpExecutor) { }

        protected override string TableName { get { return "AirlineFlightSchedule"; } }

        public override TableBase Create()
        {
            return new AirlineFlightScheduleQueryTable(HttpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(AirlineFlightSchedule)) };
        }

        protected override bool ValidateArgs()
        {
            base.ValidateArgs();
           
            var departTimeCount = QueryArgs.Args.Where(x => x.Variable == "departuretime").Count();
            if (departTimeCount > 2)
                throw new InvalidOperationException("Can only have 2 departureTime");

            if(QueryArgs.ContainsVariable("ident"))
            {
                var ident = QueryArgs["ident"];
                var match = Regex.Match(ident.PropertyValue.Value.ToString(), @"(\D+)(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var airline = match.Groups[1].Value;
                    var flightno = match.Groups[2].Value;
                    QueryArgs.Add(new QueryArgs { Variable = "airline", PropertyValue = new PropertyValue(airline) });
                    QueryArgs.Add(new QueryArgs { Variable = "flightno", PropertyValue = new PropertyValue(flightno) });
                    QueryArgs.Remove(ident);
                }
            }

            if (departTimeCount == 1)
            {
                var param = QueryArgs["departuretime"];
                if ( (param is QueryGreaterThan && param.LeftProperty) || (param is QueryLessThan && !param.LeftProperty))
                {
                    param.Variable = "startDate";
                    QueryArgs.Add(new QueryArgs { Variable = "endDate", PropertyValue = new PropertyValue(Conversion.MaxUnixDate) });
                }
                else if( (param is QueryLessThan && param.LeftProperty) || (param is QueryGreaterThan && !param.LeftProperty) )
                {
                    param.Variable = "endDate";
                    QueryArgs.Add(new QueryArgs { Variable = "startDate", PropertyValue = new PropertyValue(Conversion.MinUnixDate) });
                }
            }
            else //two departureTImes
            {

            }

            return true;
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.AirlineFlightSchedule(args);
            if (result.Error != null)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(AirlineFlightSchedule));
                
            var rows = new List<Row>();
            if (result.Data != null)
            {
                foreach (var d in result.Data)
                {
                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable() { Rows = rows.ToArray(), Descriptor = tableDescriptor };
        }
    }
}
