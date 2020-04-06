using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.Model.V2;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(ProgramElement program)
        {
            using (var s = _scope.Push())
            {
                _scope.AddTable("airlineflightschedules", new AirlineFlightScheduleQueryTable(_httpExecutor) {Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(AirlineFlightSchedule)) } );
                _scope.AddTable("getflightid", new GetFlightIdQueryTable(_httpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(GetFlightId)) } );
                _scope.AddTable("flightinfoex", new FlightInfoExQueryTable(_httpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightInfoEx)) });
                _scope.AddTable("airportinfo", new AirportInfoQueryTable(_httpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(AirportInfo)) });

                foreach (var c in program.Statements)
                    VisitChild(c);
            }
        }
    }
}
