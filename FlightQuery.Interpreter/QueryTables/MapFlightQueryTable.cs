﻿using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class MapFlightQueryTable : QueryTable
    {
        public MapFlightQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "MapFlight"; } }

        public override TableBase Create()
        {
            return new MapFlightQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(MapFlight)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetMapFlight(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            var dto = result.Data;
            if(QueryArgs.ContainsVariable("ident"))
                dto.ident = (string)QueryArgs["ident"].PropertyValue.Value;

            long mapHeight = 480;
            if (QueryArgs.ContainsVariable("mapHeight"))
                mapHeight = (long)QueryArgs["mapHeight"].PropertyValue.Value;

            long mapWidth = 640;
            if (QueryArgs.ContainsVariable("mapWidth"))
                mapWidth = (long)QueryArgs["mapWidth"].PropertyValue.Value;

            dto.mapHeight = mapHeight;
            dto.mapWidth = mapWidth;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(MapFlight));

            var rows = new List<Row>();
            if (result.Error == null)
                rows.Add(new Row() { Values = ToValues(dto, tableDescriptor) });

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
