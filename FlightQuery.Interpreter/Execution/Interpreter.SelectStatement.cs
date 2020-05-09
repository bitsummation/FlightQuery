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
            if (executedTables.Length == 0) //no executed tables. nothing to do
                return;

            var dynamicColumns = new List<DynamicColumn>();
            var descriptors = new List<IResult>();

            if (statement.All) //get all columns
            {
                var selectedIndex = 0;
                foreach(var executeTable in executedTables)
                {
                    foreach(var p in executeTable.Descriptor.Properties)
                    {
                        int propIndex = executeTable.Descriptor.GetDataRowIndex(p.Name);
                        descriptors.Add(new SelectColumn() { Table = executeTable, PropDescriptor = p, PropIndex = propIndex });

                        if (selectedIndex >= statement.Args.Length)
                        {
                            var selectArg = new SelectArgExpression(statement.Bounds);
                            selectArg.Children.Add(new SingleVariableExpression(statement.Bounds) { Id = p.Name });
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
                    var arg = new QueryPhaseArgs();
                    VisitChild(statement.Args[selectIndex], arg);

                    if (arg.BoolQueryArg.Table == null) //can't find selected PropertyDescriptor. Must be a statement
                    {
                        var prop = new PropertyDescriptor() { Name = arg.BoolQueryArg.Variable ?? "(No column name)" };
                        var dynamicColumn = new DynamicColumn { SelectArgExpression = statement.Args[selectIndex], PropDescriptor = prop };
                        descriptors.Add(dynamicColumn);
                        dynamicColumns.Add(dynamicColumn);
                    }
                    else
                    {
                        int propIndex = arg.BoolQueryArg.Table.Descriptor.GetDataRowIndex(arg.BoolQueryArg.Property.Name);
                        arg.BoolQueryArg.Property.Name = arg.BoolQueryArg.Variable;
                        descriptors.Add(new SelectColumn() { Table = arg.BoolQueryArg.Table, PropDescriptor = arg.BoolQueryArg.Property, PropIndex = propIndex });
                    }
                }
            }

            //need to go through every row here of select dynamic statement then add it Properties
            if (executedTables.Length > 0)
            {
                var rowCount = executedTables.First().Rows.Length;
                for (int row = 0; row < rowCount; row++)
                {
                    Array.ForEach(executedTables, (x) => x.RowIndex = row);
                    foreach (var d in dynamicColumns) //run each column. Value goes in data at selected index
                    {
                        var arg = new QueryPhaseArgs();
                        VisitChild(d.SelectArgExpression, arg);
                        d.Values.Add(arg.BoolQueryArg.PropertyValue);
                    }
                }
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
                        values.Add(selectedColumn.Fetch(rowIndex));
                    }

                    rows.Add(new Row() { Values = values.ToArray() });
                }
            }

            var finalDescriptors = descriptors.Select(x => x.Descriptor()).ToArray();
            TableDescriptor tableDescriptor;
            //if nested
            if (statement.IsNestedQuery)
            {
                var dups = finalDescriptors.GroupBy(x => x.Name).Where(g => g.Count() > 1).Select(x => x.Key).ToList();
                if (dups.Count > 1) //error
                {
                }

                tableDescriptor = finalDescriptors;
            }
            else
            {
                FinalSelectTableDescriptor ftableDescriptor = descriptors.Select(x => x.Descriptor()).ToArray();
                tableDescriptor = ftableDescriptor;
            }

            var selectTable = new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
            _visitStack.Peek().QueryTable = selectTable;

            if (!statement.IsNestedQuery)
                _selectResult.Add(ToSelectTable(selectTable));
        }

        private SelectTable ToSelectTable(ExecutedTable table)
        {
            var select = new SelectTable();
            select.Columns = table.Descriptor.Properties.Select(x => new Sdk.SelectColumn() { Name = x.Name, Type = x.Type == null ? "" : x.Type.Name.ToString().ToLower() }).ToArray();

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

        private interface IResult
        {
            PropertyValue Fetch(int rowIndex);
            PropertyDescriptor Descriptor();
        }


        private class DynamicColumn : IResult
        {
            public DynamicColumn()
            {
                Values = new List<PropertyValue>();
            }

            public SelectArgExpression SelectArgExpression { get; set; }
            public IList<PropertyValue> Values { get; private set; }
            public PropertyDescriptor PropDescriptor { get; set; }

            public PropertyDescriptor Descriptor()
            {
                return PropDescriptor;
            }

            public PropertyValue Fetch(int rowIndex)
            {
                return Values[rowIndex];
            }
        }


        private class SelectColumn : IResult
        {
            public TableBase Table { get; set; }
            public PropertyDescriptor PropDescriptor { get; set; }
            public int PropIndex { get; set; }

            public PropertyDescriptor Descriptor()
            {
                return PropDescriptor;
            }

            public PropertyValue Fetch(int rowIndex)
            {
                return Table.Rows[rowIndex].Values[PropIndex];
            }
        }

    }
}
