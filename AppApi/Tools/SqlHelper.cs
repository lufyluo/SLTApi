using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AppApi.Tools
{
    public class SqlHelper<TEntity> where TEntity : class, new()
    {
        public static DataSet GetDataReader(string sql, string connstr)
        {
            using (SqlConnection connection =
                new SqlConnection(connstr))
            {
                var dataset = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    sql, connection);
                adapter.SelectCommand.CommandTimeout = 180;
                adapter.Fill(dataset);
                adapter.Dispose();
                return dataset;
            }
        }
        public static TEntity Map(DataRow row)
        {
            //Step 1 - Get the Column Names
            var columnNames = row.Table.Columns
                .Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToList();

            //Step 2 - Get the Property Data Names
            var properties = (typeof(TEntity)).GetProperties()
                .ToList();

            //Step 3 - Map the data
            TEntity entity = new TEntity();
            foreach (var prop in properties)
            {
                var columnName = columnNames.FirstOrDefault(n => n.ToLower().Equals(prop.Name.ToLower()));
                Map(typeof(TEntity), row, prop, columnName, entity);
            }

            return entity;
        }
        public static void Map(Type type, DataRow row, PropertyInfo prop, string columnName, object entity)
        {
            if (!String.IsNullOrWhiteSpace(columnName) && row.Table.Columns.Contains(columnName))
            {
                var propertyValue = row[columnName];
                if (propertyValue != DBNull.Value)
                {
                    ParsePrimitive(prop, entity, row[columnName]);
                }
            }

        }
        private static void ParsePrimitive(PropertyInfo prop, object entity, object value)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(entity, value.ToString().Trim(), null);
            }
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, int.Parse(value.ToString()), null);
                }
            }
        }
    }
}