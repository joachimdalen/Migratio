# Migratio

| Migratio is a Powershell module to handle database migrations and seeder. It supports rollout, rollback and seeding data. |
| :-----------------------------------------------------------------------------------------------------------------------: |
|               Have the need to use variables or secrets in your script? **Migratio** supports that as well.               |

| ⚠️ Migratio is far from complete and tested, so some things might not work as expected. Please take care if you decide to use this before a release. |
| :--------------------------------------------------------------------------------------------------------------------------------------------------: |

## Supported databases

So far Migratio only supports PostgreSQL, but MSSQL is alo planned. Submit a feature request in the issues if you wish support for a different database.

## Cmdlets

| Name                                  | Description                                                                                     |
| ------------------------------------- | ----------------------------------------------------------------------------------------------- |
| New-MigratioMigration                 | Create a new rollout and rollback migration                                                     |
| New-MigratioMigrationTable            | Create a new migration table in the database                                                    |
| New-MigratioSeeder                    | Create a new seeder migration                                                                   |
|                                       |                                                                                                 |
| Get-UsedMigratioVariables             | Get a list over used variables for a migration file. See [Variables](##Variables) for more info |
| Get-MigratioLatestIteration           | Get the latest iteration of migrations applied                                                  |
| Get-MigratioProcessedMigrations       | Get all the applied migrations                                                                  |
| Get-MigratioScriptsForLatestIteration | Get all the applied migrations for the latest iteration                                         |
|                                       |                                                                                                 |
| Invoke-MigratioRollout                | Run a rollout of migrations that is not applied yet                                             |
| Invoke-MigratioRollback               | Run a rollback of the latest iteration of migrations                                            |

## Options

### Common options

The following options are shared between the following cmdlets

- `New-MigratioMigrationTable`
- `Invoke-MigratioRollout`
- `Invoke-MigratioRollback`
- `Get-MigratioLatestIteration`
- `Get-MigratioProcessedMigrations`
- `Get-MigratioScriptsForLatestIteration`

| Option   | Type   | Mandatory | Default     |
| -------- | ------ | --------- | ----------- |
| Password | string | Yes       | None        |
| Username | string | Yes       | None        |
| Database | string | Yes       | None        |
| Port     | int    | No        | `5432`      |
| Host     | string | No        | `127.0.0.1` |
| Schema   | string | No        | `public`    |

---

The following options are shared between the following cmdlets

- `Invoke-MigratioRollout`
- `Invoke-MigratioRollback`
- `New-MigratioMigration`
- `New-MigratioSeeder`

| Option           | Type   | Mandatory | Default      |
| ---------------- | ------ | --------- | ------------ |
| MigrationRootDir | string | No        | `migrations` |

## Variables

Want to use environment variables in your migration scripts? Well, Migratio supports that. Simply insert `${{YOUR_VARIABLE}}` and Migratio will replace the value during migration, seeding or rollback when the `TransformVariables` option is set.

```sql
CREATE USER applicationUser WITH ENCRYPTED PASSWORD '${{APP_USER_PASSWORD}}';
```
