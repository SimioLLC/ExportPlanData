using SimioAPI;
using SimioAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
/// <summary>
/// Similar to the log helpers, this provides utility methods
/// to convert from Simio Tables to Microsoft DataTable
/// </summary>
public static class TableHelpers
{
    private static CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
    private static string _dateTimeFormatString = string.Empty;

    /// <summary>
    /// Convert a Simio Table to a DataTable.
    /// The table can contain either property or State values
    /// The returned result is a Microsoft DataTabe.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="sqlColumnInfoList"></param>
    /// <returns></returns>
    internal static DataTable ConvertSimioTableToDataTable(ITable table, List<GridDataColumnInfo> sqlColumnInfoList)
    {
        List<string[]> tableList = new List<string[]>();
        int rowNumber = 0;

        // get all column names
        List<string> colNames = new List<string>();

        // get property column names
        List<string> propColNames = new List<string>();

        // get property column names
        List<string> colDataTypes = new List<string>();
        List<string> stateColDataTypes = new List<string>();

        // get column data
        foreach (var col in table.Columns)
        {
            foreach (var sqlColumnInfo in sqlColumnInfoList)
            {
                if (sqlColumnInfo.Name == col.Name)
                {
                    colNames.Add(col.Name);
                    propColNames.Add(col.Name);
                    colDataTypes.Add(GetSimioTableColumnType(col));
                }
            }
        }

        // get state column names
        List<string> stateColNames = new List<string>();
        foreach (var stateCol in table.StateColumns)
        {
            foreach (var dbColumnName in sqlColumnInfoList)
            {
                if (dbColumnName.Name == stateCol.Name)
                {
                    colNames.Add(stateCol.Name);
                    stateColNames.Add(stateCol.Name);
                    stateColDataTypes.Add(GetSimioTableStateColumnType(stateCol));
                }
            }
        }
        tableList.Add(colNames.ToArray());

        // Get Row Data
        foreach (var row in table.Rows)
        {
            rowNumber++;
            int arrayIdx = -1;
            List<string> thisRow = new List<string>();
            // get properties
            foreach (var array in propColNames)
            {
                arrayIdx++;
                if (row.Properties[array.ToString()].Value != null)
                    thisRow.Add(GetFormattedStringValue(row.Properties[array.ToString()].Value, colDataTypes[arrayIdx]));
                else thisRow.Add(GetFormattedStringValue("", colDataTypes[arrayIdx]));
            }
            arrayIdx = -1;
            // get states
            foreach (var array in stateColNames)
            {
                arrayIdx++;
                if (table.StateRows[rowNumber - 1].StateValues[array.ToString()].PlanValue != null)
                    thisRow.Add(GetFormattedStringValue(table.StateRows[rowNumber - 1].StateValues[array.ToString()].PlanValue.ToString(), stateColDataTypes[arrayIdx]));
                else thisRow.Add(GetFormattedStringValue("", stateColDataTypes[arrayIdx]));
            }
            tableList.Add(thisRow.ToArray());
        }

        // New table.
        var dataTable = new DataTable();
        dataTable.TableName = table.Name;

        // Get max columns.
        int columns = 0;
        foreach (var array in tableList)
        {
            if (array.Length > columns)
            {
                columns = array.Length;
            }
        }

        // Add columns.
        for (int cc = 0; cc < columns; cc++)
        {
            var array = tableList[0];
            dataTable.Columns.Add(array[cc]);
        }

        // Remove Column Headings
        if (tableList.Count > 0)
        {
            tableList.RemoveAt(0);
        }

        // sort rows
        //var sortedList = list.OrderBy(x => x[0]).ThenBy(x => x[3]).ToList();

        // Add rows.
        foreach (var array in tableList)
        {
            dataTable.Rows.Add(array);
        }

        return dataTable;
    }


    /// <summary>
    /// Convert a Simio Table to a DataTable.
    /// Note that the Simio table can have both Property columns (Columns)
    /// and State variable columns (StateColumns) 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    internal static DataTable ConvertSimioTableToDataTable(ITable table)
    {
        List<string[]> tableList = new List<string[]>();
        int rowNumber = 0;

        // get all column names
        List<string> colNames = new List<string>();

        // get property column names
        List<string> propColNames = new List<string>();

        // get property column names
        List<string> colDataTypes = new List<string>();
        List<string> stateColDataTypes = new List<string>();

        // get column data
        foreach (var col in table.Columns)
        {
            colNames.Add(col.Name);
            propColNames.Add(col.Name);
            colDataTypes.Add(GetSimioTableColumnType(col));
        }

        // get state column names
        List<string> stateColNames = new List<string>();
        foreach (var stateCol in table.StateColumns)
        {
            colNames.Add(stateCol.Name);
            stateColNames.Add(stateCol.Name);
            stateColDataTypes.Add(GetSimioTableStateColumnType(stateCol));
        }
        tableList.Add(colNames.ToArray());

        // Get Row Data
        foreach (var row in table.Rows)
        {
            rowNumber++;
            int arrayIdx = -1;
            List<string> thisRow = new List<string>();
            // get properties
            foreach (var array in propColNames)
            {
                arrayIdx++;
                if (row.Properties[array.ToString()].Value != null)
                    thisRow.Add(GetFormattedStringValue(row.Properties[array.ToString()].Value, colDataTypes[arrayIdx]));
                else thisRow.Add(GetFormattedStringValue("", colDataTypes[arrayIdx]));
            }
            arrayIdx = -1;
            // get states
            foreach (var array in stateColNames)
            {
                arrayIdx++;
                if (table.StateRows[rowNumber - 1].StateValues[array.ToString()].PlanValue != null)
                    thisRow.Add(GetFormattedStringValue(table.StateRows[rowNumber - 1].StateValues[array.ToString()].PlanValue.ToString(), stateColDataTypes[arrayIdx]));
                else thisRow.Add(GetFormattedStringValue("", stateColDataTypes[arrayIdx]));
            }
            tableList.Add(thisRow.ToArray());
        }

        // New table.
        var dataTable = new DataTable();
        dataTable.TableName = table.Name;

        // Get max columns.
        int columns = 0;
        foreach (var array in tableList)
        {
            if (array.Length > columns)
            {
                columns = array.Length;
            }
        }

        // Add columns.
        for (int cc = 0; cc < columns; cc++)
        {
            var array = tableList[0];
            dataTable.Columns.Add(array[cc]);
        }

        // Remove Column Headings
        if (tableList.Count > 0)
        {
            tableList.RemoveAt(0);
        }

        // sort rows
        //var sortedList = list.OrderBy(x => x[0]).ThenBy(x => x[3]).ToList();

        // Add rows.
        foreach (var array in tableList)
        {
            dataTable.Rows.Add(array);
        }

        return dataTable;
    }

    /// <summary>
    /// Unit Strings, such as length, volume, ...
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private static string GetUnitString(IProperty prop)
    {
        IUnitBase unitBase = prop.Unit;

        ITimeUnit timeUnit = unitBase as ITimeUnit;
        if (timeUnit != null)
        {
            return timeUnit.Time.ToString();
        }
        ITravelRateUnit travalrateunit = unitBase as ITravelRateUnit;
        if (travalrateunit != null)
        {
            return travalrateunit.TravelRate.ToString();
        }
        ILengthUnit lengthunit = unitBase as ILengthUnit;
        if (lengthunit != null)
        {
            return lengthunit.Length.ToString();
        }
        ICurrencyUnit currencyunit = unitBase as ICurrencyUnit;
        if (currencyunit != null)
        {
            return currencyunit.Currency.ToString();
        }
        IVolumeUnit volumeunit = unitBase as IVolumeUnit;
        if (volumeunit != null)
        {
            return volumeunit.Volume.ToString();
        }
        IMassUnit massunit = unitBase as IMassUnit;
        if (massunit != null)
        {
            return massunit.Mass.ToString();
        }
        IVolumeFlowRateUnit volumeflowrateunit = unitBase as IVolumeFlowRateUnit;
        if (volumeflowrateunit != null)
        {
            return volumeflowrateunit.Volume.ToString() + "/" + volumeflowrateunit.Time.ToString();
        }
        IMassFlowRateUnit massflowrateunit = unitBase as IMassFlowRateUnit;
        if (massflowrateunit != null)
        {
            return massflowrateunit.Mass.ToString() + "/" + massflowrateunit.Time.ToString();
        }
        ITravelAccelerationUnit timeaccelerationunit = unitBase as ITravelAccelerationUnit;
        if (timeaccelerationunit != null)
        {
            return timeaccelerationunit.Length.ToString() + "/" + timeaccelerationunit.Time.ToString();
        }
        ICurrencyPerTimeUnit currencepertimeunit = unitBase as ICurrencyPerTimeUnit;
        if (currencepertimeunit != null)
        {
            return currencepertimeunit.CurrencyPerTimeUnit.ToString();
        }

        return "none";
    }

    /// <summary>
    /// Format the valueString according to its Simio dataType.
    /// Note: TryParse is used to avoid the (very expensive) exception raising.
    /// </summary>
    /// <param name="valueString"></param>
    /// <param name="simioDataType"></param>
    /// <returns></returns>
    private static string GetFormattedStringValue(String valueString, String simioDataType)
    {
        DateTime dateProp = Convert.ToDateTime("2008-01-01 00:00:00");
        Double doubleProp = 0.0;
        Int64 intProp = 0;
        Boolean boolProp = false;

        switch (simioDataType)
        {
            case "int":
                {
                    if (valueString.Length > 0)
                    {
                        Int64.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out intProp);
                    }
                    valueString = intProp.ToString(_cultureInfo);
                }
                break;

            case "real":
                {
                    if (valueString.Length > 0)
                    {
                        Double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleProp);
                    }
                    valueString = doubleProp.ToString(_cultureInfo);
                }
                break;

            case "datetime":
                {
                    if (valueString.Length > 0)
                    {
                        DateTime.TryParse(valueString, out dateProp);
                    }
                    valueString = dateProp.ToString(_dateTimeFormatString);
                }
                break;
            case "bit":
                {
                    if (valueString.Length > 0)
                    {
                        Boolean.TryParse(valueString, out boolProp);
                    }
                    valueString = boolProp.ToString(_cultureInfo);
                }
                break;

            default:
                break;
        }
        return valueString;

    }

    /// <summary>
    /// Get the column type of a Simio table column.
    /// There are more types, but we are only dealing with real, int, datetime, and bit.
    /// Anything else is nvarchar(1000)
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    private static string GetSimioTableColumnType(ITableColumn col)
    {

        switch (col)
        {
            case IRealTableColumn _:
                return "real";
            case IIntegerTableColumn _:
                return "int";
            case IDateTimeTableColumn _:
                return "datetime";
            case IBooleanTableColumn _:
                return "bit";
            default:
                return "nvarchar(1000)";
        }

    }

    private static string GetSimioTableStateColumnType(ITableStateColumn stateCol)
    {

        switch (stateCol)
        {
            case IRealTableColumn _:
                return "real";
            case IIntegerTableColumn _:
                return "int";
            case IDateTimeTableColumn _:
                return "datetime";
            case IBooleanTableColumn _:
                return "bit";
            default:
                return "nvarchar(1000)";
        }
    }



}

