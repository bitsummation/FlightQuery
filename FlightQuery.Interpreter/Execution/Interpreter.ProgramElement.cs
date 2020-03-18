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
                _scope.AddTable("flightId", new FlightIdQueryTable(_httpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightId)) } );

                foreach (var c in program.Statements)
                    VisitChild(c);
            }
        }
    }
}
