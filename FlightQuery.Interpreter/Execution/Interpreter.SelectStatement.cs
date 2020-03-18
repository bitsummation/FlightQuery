using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(SelectStatement statement)
        {
            var executedTables = _scope.FetchAllExecutedTablesSameLevel();
            for (int selectIndex = 0; selectIndex < statement.Args.Length; selectIndex++)
            {
                Array.ForEach(executedTables, (x) => x.SelectIndex = selectIndex);
                VisitChild(statement.Args[selectIndex]);
            }

            var descriptors = new List<SelectColumn>();

            for (int selectIndex = 0; selectIndex < statement.Args.Length; selectIndex++)
            {
                var table = executedTables.Where(x => x.Descriptor.Properties.Where(x => x.SelectedIndex == selectIndex).SingleOrDefault() != null).Single();
                var prop = table.Descriptor.Properties.Where(x => x.SelectedIndex == selectIndex).Single();
                int propIndex = table.Descriptor.GetDataRowIndex(prop.Name);

                descriptors.Add(new SelectColumn() {Table = table, PropDescriptor = prop, PropIndex = propIndex });
            }

            var rows = new List<Row>();
            var rowCount = executedTables.First().Rows.Length;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) //select data for only those columns selected
            {
                var values = new List<PropertyValue>();
                for (int selectIndex = 0; selectIndex < statement.Args.Length; selectIndex++)
                {
                    var selectedColumn = descriptors[selectIndex];
                    values.Add(selectedColumn.Table.Rows[rowIndex].Values[selectedColumn.PropIndex]);
                }

                rows.Add(new Row() { Values = values.ToArray() });
            }

            TableDescriptor tableDescriptor = descriptors.Select(x => x.PropDescriptor).ToArray();
            var selectTable = new ExecutedTable() { Rows = rows.ToArray(), Descriptor = tableDescriptor };
            _selectResult = ToSelectTable(selectTable);
        }

        private SelectTable ToSelectTable(ExecutedTable table)
        {
            var select = new SelectTable();
            select.Columns = table.Descriptor.Properties.Select(x => x.Name).ToArray();

            var list = new List<SelectRow>();
            foreach(var row in table.Rows)
            {
                var selectRow = new SelectRow();
                selectRow.Values = row.Values.Select(x => x.Value).ToArray();
                list.Add(selectRow);
            }

            select.Rows = list.ToArray();

            return select;
        }


        private class SelectColumn
        {
            public TableBase Table { get; set; }
            public PropertyDescriptor PropDescriptor { get; set; }
            public int PropIndex { get; set; }
        }

    }
}
