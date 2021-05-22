namespace Migratio.Core.Database
{
    public static class Queries
    {
        public const string CheckIfTableExistsQuery =
            "SELECT EXISTS (SELECT FROM information_schema.tables WHERE  table_schema = @tableSchema AND table_name = @tableName);";

        public const string GetLatestIterationQuery =
            "SELECT \"ITERATION\" FROM \"MIGRATIONS\" ORDER BY \"ITERATION\" DESC LIMIT 1;";

        public const string GetAppliedMigrationsQuery =
            "SELECT \"MIGRATION_ID\", \"ITERATION\" FROM \"@tableSchema\".\"MIGRATIONS\";";

        public const string GetScriptsForLatestIterationQuery =
            "SELECT \"MIGRATION_ID\", \"ITERATION\" FROM \"@tableSchema\".\"MIGRATIONS\" WHERE \"ITERATION\" = @currentIteration;";

        public const string NewMigrationQuery =
            "INSERT INTO \"@tableSchema\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('@migrationName', @currentIteration);";

        public const string DeleteMigrationQuery =
            "DELETE FROM \"@tableSchema\".\"MIGRATIONS\" WHERE \"MIGRATION_ID\" = '@migrationName' AND \"ITERATION\" = '@currentIteration';";

        public static readonly string CreateMigrationsTableQuery =
            "CREATE TABLE \"@tableSchema\".\"MIGRATIONS\" (\"MIGRATION_ID\" TEXT NOT NULL, \"ITERATION\" INTEGER NOT NULL, CONSTRAINT \"PK_MIGRATIONS\" PRIMARY KEY (\"MIGRATION_ID\"));";

        public const string GetAppliedSeedersQuery =
            "SELECT \"SEED_ID\" FROM \"@tableSchema\".\"SEEDERS\";";

        public const string NewSeedersQuery =
            "INSERT INTO \"@tableSchema\".\"SEEDERS\" (\"SEED_ID\") VALUES ('@seederName');";

        public static readonly string CreateSeedersTableQuery =
            "CREATE TABLE \"@tableSchema\".\"SEEDERS\" (\"SEED_ID\" TEXT NOT NULL, CONSTRAINT \"PK_SEEDERS\" PRIMARY KEY (\"SEED_ID\"));";
    }
}