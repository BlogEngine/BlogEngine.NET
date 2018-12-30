using System;
using System.Data;
using System.Linq;

namespace Gwn.BlogEngine.Library.Extensions
{
    /// <summary>
    /// DataTable extension
    /// </summary>
    public static class BeDataTableExtension
    {
        /// <summary>
        /// Sets the primary key.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="keyColumnIndex">Index of the key column.</param>
        public static void SetPrimaryKey(this DataTable table, int keyColumnIndex)
        {
            var keys = new DataColumn[1];               // Initialize key
            keys[0] = table.Columns[keyColumnIndex];    // Primary key
            table.PrimaryKey = keys;                    // Set keys as PrimaryKey
        }

        /// <summary>
        /// Finds the row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="primaryKeyValue">The wiki command parameter.</param>
        /// <returns></returns>
        public static DataRow FindRowByCommandParameter(this DataTable table, string primaryKeyValue)
        {
            try
            {
                if (table.PrimaryKey.Count() == 0)
                    return table.Select(string.Format("CommandParameter = '{0}'", primaryKeyValue)).FirstOrDefault();
                else
                    return table.Rows.Find(new[] { primaryKeyValue });
            }
            catch(Exception ex)
            {
                return table.Rows[0];
            }
        }

        /// <summary>
        /// Finds the row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="primaryKeyValue">The wiki command parameter.</param>
        /// <returns></returns>
        public static DataRow FindRowByCommand(this DataTable table, string primaryKeyValue)
        {
            if (table.PrimaryKey.Count() == 0)
                return table.Select(string.Format("Command = '{0}'", primaryKeyValue)).FirstOrDefault();
            else
                return table.Rows.Find(new[] { primaryKeyValue });
        }


    }

}
