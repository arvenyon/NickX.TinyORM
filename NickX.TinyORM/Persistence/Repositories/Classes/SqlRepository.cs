using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Persistence.Connections.Classes;
using NickX.TinyORM.Persistence.PersistenceUtils;
using NickX.TinyORM.Persistence.Queries;
using NickX.TinyORM.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace NickX.TinyORM.Persistence.Repositories.Classes
{
    public class SqlRepository<T> : IRepository<T> where T : class, new()
    {
        private SqlConnectionFactory _conFactory;
        private ITableDefinition _table;

        //Specific Column Groups
        private List<IColumnDefinition> _selectColumns;
        private List<IColumnDefinition> _updateColumns;
        private List<IColumnDefinition> _insertColumns;

        public SqlRepository(SqlConnectionFactory conFactory)
        {
            _conFactory = conFactory;
            _table = _conFactory.Mapping.Tables.SingleOrDefault(t => t.Type == typeof(T));

            if (_table == null)
                throw new InvalidOperationException(string.Format("Requested Type {0} is not mapped!", typeof(T).Name));

            _selectColumns = new List<IColumnDefinition>();
            _selectColumns.AddRange(_table.Columns);
            _selectColumns.Add(_table.PrimaryKey);

            _updateColumns = new List<IColumnDefinition>();
            _updateColumns.AddRange(_table.Columns);

            _insertColumns = new List<IColumnDefinition>();
            _insertColumns.AddRange(_table.Columns.Where(c => c.DefaultValue == DefaultValues.None));
        }


        #region QueryCondition
        public QueryConditionBuilder<T> CreateQueryConditionBuilder()
        {
            return new QueryConditionBuilder<T>(_conFactory.Mapping);
        }
        #endregion

        #region CRUD
        public IEnumerable<T> All()
        {
            List<T> retVal = new();

            var columns = new List<string>();
            foreach (var column in _selectColumns)
            {
                var sCol = string.Format("[{0}] as '{1}'", column.ColumnName, column.Property.Name);
                columns.Add(sCol);
            }
            var sColumns = string.Join(',', columns);

            string sQuery = string.Format(@"select {0} from [{1}]", sColumns, _table.TableName);
            using (var con = _conFactory.Create())
            {
                con.Open();
                using (var cmd = new SqlCommand(sQuery, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            retVal.Add(reader.ConvertToObject<T>());
                    }
                }
            }

            return retVal;
        }

        //public void BulkDelete(T[] entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public void BulkInsert(T[] entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public void BulkUpdate(T[] entity)
        //{
        //    throw new NotImplementedException();
        //}

        public void Delete(T entity)
        {
            // get key value
            var key = _table.PrimaryKey.Property.GetValue(entity);
            key = key.ConvertValueForSql();

            // build delete statement
            var query = string.Format(@"delete from [{0}] where [{1}] = '{2}'", _table.TableName, _table.PrimaryKey.ColumnName, key);

            // create new connection
            using (var con = _conFactory.Create())
            {
                // open connection
                con.Open();

                // execute delete statement
                new SqlCommand(query, con).ExecuteNonQuery();
            }
        }

        public bool Exists(QueryConditionBuilder<T> queryConditionBuilder)
        {
            // get query condition 
            var queryCondition = queryConditionBuilder.Query;

            // build full query
            var statement = string.Format(@"select count(*) from [{0}] where {1}", _table.TableName, queryCondition);

            // create new connection
            using (var con = _conFactory.Create())
            {
                // open connection
                con.Open();

                // return true if count is greather than zero, else false
                return (int)new SqlCommand(statement, con).ExecuteScalar() > 0;
            }
        }

        public object Insert(T entity)
        {
            // get columns & values
            var insertColumns = new List<string>();
            var insertValues = new List<string>();

            foreach (var col in _insertColumns)
            {
                // convert value for sql
                object val = col.Property.GetValue(entity);
                if (col.Property.PropertyType.IsEnum)
                {
                    val = (int)col.Property.GetValue(entity);
                }
                val = val.ConvertValueForSql();
                

                // build parts
                insertColumns.Add(string.Format(@"[{0}]", col.ColumnName));
                insertValues.Add(string.Format(@"'{0}'", val));
            }

            // build query parts
            var qInsertColumns = string.Join(",", insertColumns);
            var qInsertValues = string.Join(",", insertValues);

            // build full statement
            var query = string.Format("insert into [{0}]({1}) output inserted.[{2}] values ({3})", 
                _table.TableName, 
                qInsertColumns,
                _table.PrimaryKey.ColumnName,
                qInsertValues);

            // create new connection
            using (var con = _conFactory.Create())
            {
                // open connection
                con.Open();

                // create new sqlcommand & execute non query
                var key = new SqlCommand(query, con).ExecuteScalar();

                // return converted for .NET
                return key.ConvertValueFromSql(_table.PrimaryKey.Property.PropertyType);
            }
        }

        public IEnumerable<T> Multiple(QueryConditionBuilder<T> queryConditionBuilder)
        {
            // Init return value
            List<T> retVal = new();

            // Get query condition
            var queryCondition = queryConditionBuilder.Query;

            // Build full query
            string sQuery = string.Format(@"select {0} from [{1}] where {2}", BuildSelectColumns(), _table.TableName, queryCondition);

            // create connection
            using (var con = _conFactory.Create())
            {
                // Open connection
                con.Open();

                // init new sql command using built query & created connection
                using (var cmd = new SqlCommand(sQuery, con))
                {
                    // read data & convert to object
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            retVal.Add(reader.ConvertToObject<T>());
                    }
                }
            }

            // return converted object collection
            return retVal;
        }

        public T Single(object key)
        {
            // convert key to sql compatible value
            key = key.ConvertValueForSql();

            // get select columns string
            var selectColumns = BuildSelectColumns();

            // build full statement
            var query = string.Format("select top 1 {0} from [{1}] where [{2}] = '{3}'", selectColumns, _table.TableName, _table.PrimaryKey.ColumnName, key);

            // create new connection
            using (var con = _conFactory.Create())
            {
                // open connection
                con.Open();

                List<T> found = new();
                using (var cmd = new SqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            found.Add(reader.ConvertToObject<T>());
                    }
                }
                if (found.Count > 0)
                    return found[0];
                return null;
            }
        }

        public T Single(QueryConditionBuilder<T> queryConditionBuilder)
        {
            // get select columns
            var selectColumns = BuildSelectColumns();

            // build full statement
            var query = string.Format("select top 1 {0} from {1} where {2}", selectColumns, _table.TableName, queryConditionBuilder.Query);

            // create new connection
            using (var con = _conFactory.Create())
            {
                // open connection
                con.Open();

                List<T> found = new();
                using (var cmd = new SqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            found.Add(reader.ConvertToObject<T>());
                    }
                }
                if (found.Count > 0)
                    return found[0];
                return null;
            }
        }

        public void Update(T entity)
        {
            // get update columns & values
            List<string> updateColumns = new();

            // build
            foreach (var col in _updateColumns)
            {
                var val = col.Property.GetValue(entity).ConvertValueForSql();
                updateColumns.Add(string.Format("[{0}] = '{1}'", col.ColumnName, val));
            }
            string qUpdateColumns = string.Join(",", updateColumns);

            // build full statement
            var key = _table.PrimaryKey.Property.GetValue(entity).ConvertValueForSql();
            string query = string.Format("update [{0}] set {2} where [{3}] = '{4}'",
                _table.TableName,
                qUpdateColumns,
                _table.PrimaryKey.ColumnName,
                key);

            // create new connection
            using (var con = _conFactory.Create())
            {
                // open connection
                con.Open();

                // init new command & execute
                new SqlCommand(query, con).ExecuteNonQuery();
            }
        } 
        #endregion

        private string BuildSelectColumns()
        {
            // init return value
            var columns = new List<string>();

            // iterate through all columns which should be included in select queries
            foreach (var column in _selectColumns)
            {
                // build column part
                var sCol = string.Format("[{0}] as '{1}'", column.ColumnName, column.Property.Name);

                // add to list
                columns.Add(sCol);
            }

            // return list items joined with commata
            return string.Join(',', columns);
        }
    }
}
