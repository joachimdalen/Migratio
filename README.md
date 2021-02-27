![header-image](./images/header.png)

| Migratio is a PowerShell module to handle database migrations and seeding. It supports rollout, rollback and seeding data. |
| :------------------------------------------------------------------------------------------------------------------------: |
|               Have the need to use variables or secrets in your script? **Migratio** supports that as well.                |

| ⚠️ Migratio is far from complete and tested, so some things might not work as expected. Please take care if you decide to use this before a release. |
| :--------------------------------------------------------------------------------------------------------------------------------------------------: |

## Supported databases

So far Migratio only supports PostgreSQL, but MSSQL is alo planned. Submit a feature request in the issues if you wish support for a different database.

## Cmdlets

| Name                            | Description                                                                                    |
| ------------------------------- | ---------------------------------------------------------------------------------------------- |
| New-MgMigration                 | Create a new rollout and rollback migration                                                    |
| New-MgMigrationTable            | Create a new migration table in the database                                                   |
| New-MgSeeder                    | Create a new seeder migration                                                                  |
|                                 |                                                                                                |
| Get-MgUsedVariables             | Get a list over used variables for a migration file. See [Variables](#Variables) for more info |
| Get-MgLatestIteration           | Get the latest iteration of migrations applied                                                 |
| Get-MgProcessedMigrations       | Get all the applied migrations                                                                 |
| Get-MgScriptsForLatestIteration | Get all the applied migrations for the latest iteration                                        |
|                                 |                                                                                                |
| Invoke-MgRollout                | Run a rollout of migrations that is not applied yet                                            |
| Invoke-MgRollback               | Run a rollback of the latest iteration of migrations                                           |

## Options

### Common options

The following options are shared between the following cmdlets

- `New-MgMigrationTable`
- `Invoke-MgRollout`
- `Invoke-MgRollback`
- `Get-MgLatestIteration`
- `Get-MgProcessedMigrations`
- `Get-MgScriptsForLatestIteration`

| Option   | Type   | Mandatory | Default     |
| -------- | ------ | --------- | ----------- |
| Username | string | Yes       | None        |
| Database | string | Yes       | None        |
| Port     | int    | No        | `5432`      |
| Host     | string | No        | `127.0.0.1` |
| Schema   | string | No        | `public`    |

---

The following options are shared between the following cmdlets

- `Invoke-MgRollout`
- `Invoke-MgRollback`
- `New-MgMigration`
- `New-MgSeeder`

| Option           | Type   | Mandatory | Default      |
| ---------------- | ------ | --------- | ------------ |
| MigrationRootDir | string | No        | `migrations` |

## Variables

Want to use environment variables in your migration scripts? Well, Migratio supports that. Simply insert `${{YOUR_VARIABLE}}` and Migratio will replace the value during migration, seeding or rollback when the `ReplaceVariables` option is set.

```sql
CREATE USER applicationUser WITH ENCRYPTED PASSWORD '${{APP_USER_PASSWORD}}';
```

## Configuration File
```yaml
directories:
  base: /dev/migrations # Path to base directory containing subfolders
  rollout: /dev/migrations/rollout # Path to rollout scripts
  rollback: /dev/migrations/rollback # Path to rollback scripts
  seeders: /dev/migrations/seeders # Path to seeder scripts
envMapping: # Mapping/Translation of variables and env variables
  MG_DB_PASSWORD: DB_USERNAME
envFile: './backend.env' # Path to env file
auth: # DB auth options
  postgres:
    host: 'localhost'
    port: 1234
    database: 'TestDB'
    username: 'postgres'
    password: '${{MG_DB_PASSWORD}}' # Will use DB_USERNAME under lookup (ref: envMapping)
replaceVariables: true # Replace variables in rollout scripts

```