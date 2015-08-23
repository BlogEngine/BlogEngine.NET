using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlogEngine.Core.Providers
{
    /// <summary>
    /// Helper class for working with DbConnections.
    /// </summary>
    /// <remarks>
    /// 
    /// This class is meant to reduce the amount of repeated code in database provider classes by pulling all the common actions
    /// into one spot. This should remove the many repetitive null checks on connections/parameters.
    /// 
    /// This class handles the creation of DbConnection, setting its connection string, and opening the connection if possible.
    /// 
    /// Usage is simple:
    /// using(var helper = new ConnectionHelper(provider)) {
    ///     if (helper.HasConnection) {
    ///         // do stuff
    ///     }
    /// }
    /// 
    /// Note: This class throws a NullReferenceException if its Provider.CreateParameter() method returns a null object.
    /// All of the methods in the DbBlogProvider class require parameterized queries, and previously each parameter
    /// created was being checked for null before proceeding. It's better to fail fast in this instance, to help creaters
    /// of custom implementations figure out what's wrong.
    /// 
    /// </remarks>
    public sealed class DbConnectionHelper : IDisposable
    {

        #region "Constructors"

        /// <summary>
        /// Creates a new DbConnectionHelper instance from the given ConnectionStringSettings.
        /// </summary>
        /// <param name="settings"></param>
        public DbConnectionHelper(ConnectionStringSettings settings) : this(settings.ProviderName, settings.ConnectionString)
        {
        }

        /// <summary>
        /// Creates a new DbConnectionHelper instance from the given provider name and database connection string..
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="connectionString"></param>
        public DbConnectionHelper(string providerName, string connectionString)
        {
            this._dbProvFactory = DbProviderFactories.GetFactory(providerName);
            this._connection = this._dbProvFactory.CreateConnection();

            this._hasConnection = (this._connection != null);
            if (this._hasConnection)
            {
                this._connection.ConnectionString = connectionString;
                this._connection.Open();
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Returns the DbConnection of this instance.
        /// </summary>
        public DbConnection Connection
        {
            get { return this._connection; }
        }
        private DbConnection _connection;

        /// <summary>
        /// Gets whether the Connection of this ConnectionHelper instance is null.
        /// </summary>
        public bool HasConnection
        {
            get { return this._hasConnection; }
        }

        private bool _hasConnection;

        /// <summary>
        /// Gets the DbProviderFactory used by this ConnectionHelper instance.
        /// </summary>
        public DbProviderFactory Provider
        {
            get { return this._dbProvFactory; }
        }
        private DbProviderFactory _dbProvFactory;

        #endregion

        #region "Methods"

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("ConnectionHelper");
            }
        }

        /// <summary>
        /// Uses this ConnectionHelper instance's connection to create and return a new DbCommand instance.
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateCommand()
        {
            this.CheckDisposed();
            return this.Connection.CreateCommand();
        }

        /// <summary>
        /// Users this ConnectionHelper instance's connection to create and return a new DbCommand with the given command text. CommandType is automatically set to CommandType.Text.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DbCommand CreateTextCommand(string commandText)
        {
            var command = this.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;

            return command;
        }


        /// <summary>
        /// Uses this ConnectionHelper's Provider to create a DbParameter instance with the given parameter name and value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string parameterName, object value)
        {
            return CreateParameter(parameterName, value, null, null);
        }

        /// <summary>
        /// Uses this ConnectionHelper's Provider to create a DbParameter instance with the given parameter name and value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The DB type.</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string parameterName, object value, DbType dbType)
        {
            return CreateParameter(parameterName, value, dbType, null);
        }

        /// <summary>
        /// Uses this ConnectionHelper's Provider to create a DbParameter instance with the given parameter name and value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The DB type.</param>
        /// <param name="size">The size/length of the parameter.</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string parameterName, object value, DbType? dbType, int? size)
        {
            this.CheckDisposed();

            var param = this.Provider.CreateParameter();

            if (param == null)
            {
                throw new NullReferenceException("DbBlogProvider");
            }
            else
            {
                param.ParameterName = parameterName;
                param.Value = value;

                if (dbType.HasValue)
                    param.DbType = dbType.Value;

                if (size.HasValue)
                    param.Size = size.Value;

                return param;
            }
        }

        #endregion

        #region "IDisposable"

        private bool isDisposed;

        private void Dispose(bool disposing)
        {
            try
            {
                if (!this.isDisposed && disposing)
                {
                    if (this._connection != null)
                    {
                        this._connection.Dispose();
                    }
                }
            }
            finally
            {
                this._dbProvFactory = null;
                this._connection = null;
                this._hasConnection = false;
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Disposes this DbConnectionHelper and its underlying connection.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
