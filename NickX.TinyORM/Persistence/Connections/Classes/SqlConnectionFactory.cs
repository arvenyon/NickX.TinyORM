using NickX.TinyORM.Mapping.Classes.Annotation;
using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Persistence.Connections.Interfaces;
using NickX.TinyORM.Persistence.PersistenceUtils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace NickX.TinyORM.Persistence.Connections.Classes
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        #region IConnectionFactory
        public IMapping Mapping { get; set; }

        public SqlConnection Create()
        {
            if (_create && !_structureChecked)
                CreateStructure();
            return new SqlConnection(_connectionString);
        }
        #endregion

        #region Fields
        private string _connectionString;
        private bool _create;
        private string _masterConnectionString;
        private bool _structureChecked;
        private string _database;
        #endregion

        #region Ctor
        public SqlConnectionFactory(string connectionString, IMapping mapping, bool create)
        {
            this.Mapping = mapping;
            _connectionString = connectionString;
            _create = create;

            var builder = new SqlConnectionStringBuilder(connectionString);
            _database = builder.InitialCatalog;

            builder.InitialCatalog = "master";
            _masterConnectionString = builder.ConnectionString;
        }

        public SqlConnectionFactory(string connectionString, bool create) : this(connectionString, new AnnotationMapping(), create) { }
        #endregion

        #region Database Structure
        void CreateStructure()
        {
            // Check Database
            var sDbExists = string.Format(@"if db_id('{0}') is not null select 1 else select 0", _database);
            var sCreateDb = string.Format(@"create database [{0}]", _database);

            using (var con = new SqlConnection(_masterConnectionString))
            {
                con.Open();

                // Check if database exists
                var dbExists = (int)new SqlCommand(sDbExists, con).ExecuteScalar() == 1;

                // Create If Not Exists
                if (!dbExists)
                    new SqlCommand(sCreateDb, con).ExecuteNonQuery();
            }

            // Check Table & Columns
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                foreach (var table in Mapping.Tables)
                {
                    var sTableExists = string.Format(@"if (exists (select * from information_schema.tables where table_catalog = '{0}' and table_name = '{1}')) select 1 else select 0", _database, table.TableName);
                    var tableExists = (int)new SqlCommand(sTableExists, con).ExecuteScalar() == 1;

                    if (!tableExists)
                    {
                        var lines = new List<string>();

                        // add primary key column
                        if (table.PrimaryKey != null)
                        {
                            string addCol = BuildColumnStatement(table.PrimaryKey) + " primary key";
                            lines.Add(addCol);
                        }

                        // add all remaining columns
                        foreach (var column in table.Columns)
                        {
                            string addCol = BuildColumnStatement(column);
                            lines.Add(addCol);
                        }

                        // add foreign key constraints
                        foreach (var foreignKey in table.ForeignKeys)
                        {
                            // resolve column name
                            var columnName = foreignKey.BoundProperty.Name;
                            var tableName = foreignKey.ReferencedType.Name;
                            var refColumnName = foreignKey.ReferencedProperty.Name;

                            var colDef = table.Columns.SingleOrDefault(c => c.Property == foreignKey.BoundProperty);
                            if (colDef != null)
                                columnName = colDef.ColumnName;

                            // resolve table name
                            var tableDef = Mapping.Tables.SingleOrDefault(t => t.Type == foreignKey.ReferencedType);
                            if (tableDef != null)
                            {
                                tableName = tableDef.TableName;

                                // resolve referenced property name
                                {
                                    var bColDef = tableDef.Columns.SingleOrDefault(c => c.Property == foreignKey.ReferencedProperty);
                                    if (bColDef != null)
                                        refColumnName = bColDef.ColumnName;
                                }
                            }
                            string addFk = string.Format("foreign key([{0}]) references [{1}]({2})", columnName, tableName, refColumnName);
                            lines.Add(addFk);
                        }

                        var sCreateTable = string.Format(@"create table [{0}]({1})", table.TableName, string.Join(',', lines));
                        new SqlCommand(sCreateTable, con).ExecuteNonQuery();
                    }
                    else
                    {
                        // TODO: Add functionality -> add foreign key even if table already exists
                        foreach (var column in table.Columns)
                        {
                            // Check if Column Exists
                            var sColExists = string.Format(@"if COL_LENGTH('{0}.dbo.{1}','{2}') is null select 0 else select 1", _database, table.TableName, column.ColumnName);
                            var colExists = (int)new SqlCommand(sColExists, con).ExecuteScalar() == 1;

                            if (!colExists)
                            {
                                string addCol = BuildColumnStatement(column);
                                var sAddCol = string.Format(@"alter table [{0}] add {1}", table.TableName, addCol);
                                new SqlCommand(sAddCol, con).ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            _structureChecked = true;
        }

        private string BuildColumnStatement(IColumnDefinition columnDefinition)
        {
            // add support for length
            string sqlType = columnDefinition.Property.ToSqlType();
            if (columnDefinition.Property.PropertyType == typeof(string))
            {
                var length = columnDefinition.ColumnLength;
                var sLength = string.Empty;
                if (length == default)
                    sLength = "max";
                else
                    sLength = length.ToString();

                sqlType += string.Format("({0})", sLength);
            }
            
            string addCol = string.Format("[{0}] {1}", columnDefinition.ColumnName, sqlType);

            switch (columnDefinition.DefaultValue)
            {
                case DefaultValues.AutoIncrement:
                    addCol += " identity(1,1)";
                    break;
                case DefaultValues.UniqueIdentifier:
                    addCol += " default newsequentialid()";
                    break;
                case DefaultValues.Timestamp:
                    addCol += " default getdate()";
                    break;
                case DefaultValues.Custom:
                    if (columnDefinition.CustomDefaultValue == null)
                        throw new InvalidOperationException("DefaultValue cannot be set to custom while CustomDeafultValue is null");

                    addCol += string.Format(" default '{0}'", columnDefinition.CustomDefaultValue);
                    break;
            }

            if (!columnDefinition.AllowsNull)
                addCol += " not null";

            return addCol;
        }
        #endregion
    }
}
