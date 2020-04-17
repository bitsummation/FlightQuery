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

            if (statement.All) //get all columns
            {
                var selectedIndex = 0;
                foreach(var executeTable in executedTables)
                {
                    foreach(var p in executeTable.Descriptor.Properties)
                    {
                        p.SelectedIndex.Add(selectedIndex);
                        if (selectedIndex >= statement.Args.Length)
                        {
                            var selectArg = new SelectArgExpression(statement.ParseInfo);
                            selectArg.Children.Add(new SingleVariableExpression(statement.ParseInfo) { Id = p.Name });
                            statement.Children.Add(selectArg);
                        }
                        selectedIndex++;
                    }
                }
            }
            else
            {
                for (int selectIndex = 0; selectIndex < statement.Args.Length; selectIndex++)
                {
                    Array.ForEach(executedTables, (x) => x.SelectIndex = selectIndex);
                    VisitChild(statement.Args[selectIndex].Variable);
                }
            }

            if (Errors.Count > 0) //errors no need to select at this point
                return;

            var descriptors = new List<SelectColumn>();

            for (int selectIndex = 0; selectIndex < statement.Args.Length; selectIndex++)
            {
                var table = executedTables.Where(x => x.Descriptor.Properties.Where(x => x.SelectedIndex.Contains(selectIndex)).SingleOrDefault() != null).Single();
                var prop = table.Descriptor.Properties.Where(x => x.SelectedIndex.Contains(selectIndex)).Single();
                int propIndex = table.Descriptor.GetDataRowIndex(prop.Name);
                prop.Name = statement.Args[selectIndex].As != null ? statement.Args[selectIndex].As.Alias : prop.Name;

                descriptors.Add(new SelectColumn() {Table = table, PropDescriptor = prop, PropIndex = propIndex });
            }

            var rows = new List<Row>();
            if (executedTables.Length > 0)
            {
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
            }

            FinalSelectTableDescriptor tableDescriptor = descriptors.Select(x => x.PropDescriptor).ToArray();
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
