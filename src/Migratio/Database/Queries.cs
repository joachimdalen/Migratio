namespace Migratio.Database
{
    public static class Queries
    {
        public static readonly string CreateMigrationsTableQuery =
            "CREATE TABLE \"@tableSchema\".\"MIGRATIONS\" (\"MIGRATION_ID\" TEXT NOT NULL, \"ITERATION\" INTEGER NOT NULL, CONSTRAINT \"PK_MIGRATIONS\" PRIMARY KEY (\"MIGRATION_ID\"));";

        public const string CheckIfMigrationTableExistsQuery =
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
    }
}