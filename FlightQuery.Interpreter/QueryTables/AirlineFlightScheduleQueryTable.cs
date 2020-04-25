using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlightQuery.Interpreter.QueryTables
{
    public class AirlineFlightScheduleQueryTable : QueryTable
    {
        public AirlineFlightScheduleQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "AirlineFlightSchedules"; } }

        public override TableBase Create()
        {
            return new AirlineFlightScheduleQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(AirlineFlightSchedule)));
        }

        protected override void ValidateArgs()
        {
            base.ValidateArgs();
           
            if(QueryArgs.ContainsVariable("origin"))
            {
                QueryArgs["origin"].PropertyValue = new PropertyValue(((string)(QueryArgs["origin"].PropertyValue.Value ?? "")).ToUpper());
            }

            if (QueryArgs.ContainsVariable("destination"))
            {
                QueryArgs["destination"].PropertyValue = new PropertyValue(((string)(QueryArgs["destination"].PropertyValue.Value ?? "")).ToUpper());
            }

            if (QueryArgs.ContainsVariable("ident"))
            {
                QueryArgs["ident"].PropertyValue = new PropertyValue(((string)(QueryArgs["ident"].PropertyValue.Value ?? "")).ToUpper());
            }

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
                    var startDate = (DateTime)Conversion.ConvertLongToDateTime(param.PropertyValue.Value);
                    var endDate = startDate.AddDays(7); //no end date we just assume a week forward
                    QueryArgs.Add(new QueryArgs { Variable = "endDate", PropertyValue = new PropertyValue(Conversion.ConvertDateTimeToLong(endDate)) });
                }
                else if( (param is QueryLessThan && param.LeftProperty) || (param is QueryGreaterThan && !param.LeftProperty) )
                {
                    param.Variable = "endDate";
                    QueryArgs.Add(new QueryArgs { Variable = "startDate", PropertyValue = new PropertyValue(Conversion.MinUnixDate) });
                }
                else if(param is EqualQueryArg)
                {
                    param.Variable = "startDate";
                    var startDate = (DateTime)Conversion.ConvertLongToDateTime(param.PropertyValue.Value);
                    var endDate = startDate.AddMinutes(1); 
                    startDate = startDate.AddMinutes(-1);

                    param.PropertyValue = new PropertyValue(Conversion.ConvertDateTimeToLong(startDate));
                    QueryArgs.Add(new QueryArgs { Variable = "endDate", PropertyValue = new PropertyValue(Conversion.ConvertDateTimeToLong(endDate)) });
                }
            }
            else //two departureTImes
            {
                foreach (var param in QueryArgs.Args.Where(x => x.Variable == "departuretime"))
                {
                    if ((param is QueryGreaterThan && param.LeftProperty) || (param is QueryLessThan && !param.LeftProperty))
                    {
                        param.Variable = "startDate";
                    }
                    else if ((param is QueryLessThan && param.LeftProperty) || (param is QueryGreaterThan && !param.LeftProperty))
                    {
                        param.Variable = "endDate";
                    }
                }
            }

        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.AirlineFlightSchedule(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(AirlineFlightSchedule));
                
            var rows = new List<Row>();
            if (result.Data != null && result.Error == null)
            {
                foreach (var d in result.Data)
                {
                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray()};
        }
    }
}
