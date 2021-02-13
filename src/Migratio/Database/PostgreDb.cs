using System.Collections.Generic;
using Migratio.Contracts;
using Migratio.Models;
using Npgsql;

namespace Migratio.Database
{
    public class PostgreDb : IDatabaseProvider
    {
        private readonly DbConnectionInfo _connectionInfo;

        public PostgreDb(DbConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        private NpgsqlConnection GetConnection()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Database = _connectionInfo.Database,
                Username = _connectionInfo.Username,
                Host = _connectionInfo.Host,
                Port = _connectionInfo.Port,
                Password = _connectionInfo.Password,
            };

            return new NpgsqlConnection(builder.ToString());
        }

        /// <summary>
        /// Check if migration table exists for given database and schema
        /// </summary>
        /// <returns></returns>
        public bool MigrationTableExists()
        {
            var result = false;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(Queries.CheckIfMigrationTableExistsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("tableSchema", _connectionInfo.Schema);
                    cmd.Parameters.AddWithValue("tableName", "MIGRATIONS");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetBoolean(0);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get latest migration iteration
        /// </summary>
        /// <returns></returns>
        public int GetLatestIteration()
        {
            var result = 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(Queries.GetLatestIterationQuery, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get latest migration iteration
        /// </summary>
        /// <returns></returns>
        public Migration[] GetAppliedMigrations()
        {
            var list = new List<Migration>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.GetAppliedMigrationsQuery.Replace("@tableSchema", _connectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Migration
                            {
                                MigrationId = reader.GetString(0),
                                Iteration = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Get latest migration iteration
        /// </summary>
        /// <returns></returns>
        public Migration[] GetAppliedScriptsForLatestIteration()
        {
            var latestIteration = GetLatestIteration();
            var list = new List<Migration>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.GetScriptsForLatestIterationQuery.Replace("@tableSchema", _connectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("currentIteration", latestIteration);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Migration
                            {
                                MigrationId = reader.GetString(0),
                                Iteration = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public int CreateMigrationTable()
        {
            int result;
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.CreateMigrationsTableQuery.Replace("@tableSchema", _connectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        public int RunTransaction(string query)
        {
            int result;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        result = cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }

            return result;
        }
    }
}