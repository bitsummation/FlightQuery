﻿using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryTables;
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
                _scope.AddTable("airlineflightschedules", new AirlineFlightScheduleQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(AirlineFlightSchedule))));
                _scope.AddTable("getflightid", new GetFlightIdQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(GetFlightId))));
                _scope.AddTable("flightinfoex", new FlightInfoExQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightInfoEx))));
                _scope.AddTable("airportinfo", new AirportInfoQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(AirportInfo))));
                _scope.AddTable("airlineinfo", new AirlineInfoQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(AirlineInfo))));
                _scope.AddTable("inboundflightinfo", new InboundFlightInfoQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(InboundFlightInfo))));
                _scope.AddTable("inflightinfo", new InFlightInfoQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(InFlightInfo))));
                _scope.AddTable("scheduled", new ScheduledQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Scheduled))));
                _scope.AddTable("arrived", new ArrivedQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Arrived))));
                _scope.AddTable("gethistoricaltrack", new GetHistoricalTrackQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(GetHistoricalTrack))));
                _scope.AddTable("enroute", new EnrouteQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Enroute))));
                _scope.AddTable("departed", new DepartedQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Departed))));
                _scope.AddTable("mapflight", new MapFlightQueryTable(_httpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(MapFlight))));

                foreach (var c in program.Statements)
                    VisitChild(c);
            }
        }
    }
}
