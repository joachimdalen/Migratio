using System.Collections.Generic;
using Migratio.Core.Contracts;
using Migratio.Core.Models;
using Npgsql;

namespace Migratio.Core.Database
{
    public class PostgreDb : IDatabaseProvider
    {
        private DbConnectionInfo ConnectionInfo { get; set; }

        /// <inheritdoc />
        public void SetConnectionInfo(DbConnectionInfo info)
        {
            ConnectionInfo = info;
        }

        private bool TableExists(string table)
        {
            var result = false;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(Queries.CheckIfTableExistsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("tableSchema", ConnectionInfo.Schema);
                    cmd.Parameters.AddWithValue("tableName", table);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) result = reader.GetBoolean(0);
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public bool MigrationTableExists()
        {
            return TableExists("MIGRATIONS");
        }

        /// <inheritdoc />
        public bool SeedingTableExists()
        {
            return TableExists("SEEDERS");
        }

        /// <inheritdoc />
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
                        while (reader.Read()) result = reader.GetInt32(0);
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public Migration[] GetAppliedMigrations()
        {
            var list = new List<Migration>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.GetAppliedMigrationsQuery.Replace("@tableSchema", ConnectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(new Migration
                            {
                                MigrationId = reader.GetString(0),
                                Iteration = reader.GetInt32(1)
                            });
                    }
                }
            }

            return list.ToArray();
        }

        /// <inheritdoc />
        public Seed[] GetAppliedSeeders()
        {
            var list = new List<Seed>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.GetAppliedSeedersQuery.Replace("@tableSchema", ConnectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(new Seed
                            {
                                SeedId = reader.GetString(0)
                            });
                    }
                }
            }

            return list.ToArray();
        }

        /// <inheritdoc />
        public Migration[] GetAppliedScriptsForLatestIteration()
        {
            var latestIteration = GetLatestIteration();
            var list = new List<Migration>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.GetScriptsForLatestIterationQuery.Replace("@tableSchema", ConnectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("currentIteration", latestIteration);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(new Migration
                            {
                                MigrationId = reader.GetString(0),
                                Iteration = reader.GetInt32(1)
                            });
                    }
                }
            }

            return list.ToArray();
        }

        /// <inheritdoc />
        public int CreateMigrationTable()
        {
            int result;
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.CreateMigrationsTableQuery.Replace("@tableSchema", ConnectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }
        
        /// <inheritdoc />
        public int CreateSeedersTable()
        {
            int result;
            using (var conn = GetConnection())
            {
                conn.Open();
                var query = Queries.CreateSeedersTableQuery.Replace("@tableSchema", ConnectionInfo.Schema);
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    result = cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        /// <inheritdoc />
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

        private NpgsqlConnection GetConnection()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Database = ConnectionInfo.Database,
                Username = ConnectionInfo.Username,
                Host = ConnectionInfo.Host,
                Port = ConnectionInfo.Port,
                Password = ConnectionInfo.Password
            };

            return new NpgsqlConnection(builder.ToString());
        }
    }
}