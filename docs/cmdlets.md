# :desktop_computer: Cmdlets

| Name                                                                | Description                                                                                    |
| ------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| [New-MgMigration](#New-MgMigration)                                 | Create a new rollout and rollback migration                                                    |
| [New-MgMigrationTable](#New-MgMigrationTable)                       | Create a new migration table in the database                                                   |
| [New-MgSeeder](#New-MgSeeder)                                       | Create a new seeder migration                                                                  |
|                                                                     |                                                                                                |
| [Get-MgUsedVariables](#Get-MgUsedVariables)                         | Get a list over used variables for a migration file. See [Variables](#Variables) for more info |
| [Get-MgLatestIteration](#Get-MgLatestIteration)                     | Get the latest iteration of migrations applied                                                 |
| [Get-MgProcessedMigrations](#Get-MgProcessedMigrations)             | Get all the applied migrations                                                                 |
| [Get-MgScriptsForLatestIteration](#Get-MgScriptsForLatestIteration) | Get all the applied migrations for the latest iteration                                        |
|                                                                     |                                                                                                |
| [Invoke-MgRollout](#Invoke-MgRollout)                               | Run a rollout of migrations that is not applied yet                                            |
| [Invoke-MgRollback](#Invoke-MgRollback)                             | Run a rollback of the latest iteration of migrations                                           |

> In the documentation, optional parameters is wrapped in `[]`

## New-MgMigration

Creates a new migration file in the rollout folder and a rollback migration in the rollback folder. Migratio expects the file names to be the same for both migrations.
The command below will create a file structure looking something like

```
base/
  rollout/
    20210210_171649_create_users_table.sql
    ...
  rollback/
    20210210_171649_create_users_table.sql
    ...
```

```powershell
> New-MgMigration -Name "Add users table" [
    -ConfigFile "/dev/project/migratio.yml"
    -MigrationRootDir "/path/to/dir"
  ]
```

### Input

| Option           | Type   | From config | Mandatory | Default | Comment                                                                                                                                 |
| ---------------- | ------ | ----------- | --------- | ------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| ConfigFile       | string | No          | No        | None    | Path to Migratio configuration file. See [Configuration File](#configuration-file)                                                      |
| Name             | string | No          | Yes       | None    | Name of the migration. Example above will become `add_users_table`                                                                      |
| MigrationRootDir | string | Yes         | No        | None    | Specifies the root directory of migrations if using default directory naming. Equivalent to setting the base option in Migratio config. |

---

## New-MgMigrationTable

Creates a new migration table in the database. This table is responsible for keeping track over which migrations has been applied to the database.

```powershell
> New-MgMigrationTable [
    -ConfigFile "/dev/project/migratio.yml"
    -Username "dbuser"
    -Database "MyDb"
    -Port 1234
    -Host "localhost"
    -Schema "db"
  ]
```

### Input

| Option     | Type   | From config | Mandatory | Default     | Comment                                                                                                |
| ---------- | ------ | ----------- | --------- | ----------- | ------------------------------------------------------------------------------------------------------ |
| ConfigFile | string | No          | No        | None        | Path to Migratio configuration file. See [Configuration File](#configuration-file)                     |
| Username   | string | Yes         | Yes\*     | None        | Username of database user                                                                              |
| Database   | string | Yes         | Yes\*     | None        | Specifies the name of the database to connect to                                                       |
| Port       | int    | Yes         | Yes\*     | `5432`      | Specifies the port on the database server to connect to                                                |
| Host       | string | Yes         | Yes\*     | `127.0.0.1` | Specifies the hostname or ip address of the machine to connect to                                      |
| Schema     | string | Yes         | Yes\*     | `public`    | Specifies the default database schema. Only valid for Postgres                                         |
| Password   | string | Yes         | Yes       | None        | Password for the database user. Only settable from configuration file or env variable `MG_DB_PASSWORD` |

`*Mandatory if no configuration file path is given. If both is given, CLI values will override configuration file`

## New-MgSeeder

Creates a data seeder file. Currently there is no logic to revert seeders. The command below will create a file structure looking something like

```
base/
  rollout/
    ...
  rollback/
    ...
  seeders/
    20210210_171649_add_default_config_values.sql
```

```powershell
> New-MgSeeder -Name "Add default config values" [
    -ConfigFile "/dev/project/migratio.yml"
    -MigrationRootDir "/path/to/dir"
  ]
```

### Input

| Option           | Type   | From config | Mandatory | Default | Comment                                                                                                                                 |
| ---------------- | ------ | ----------- | --------- | ------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| ConfigFile       | string | No          | No        | None    | Path to Migratio configuration file. See [Configuration File](#configuration-file)                                                      |
| Name             | string | No          | Yes       | None    | Name of the seeder. Example above will become `add_default_config_values`                                                               |
| MigrationRootDir | string | Yes         | No        | None    | Specifies the root directory of migrations if using default directory naming. Equivalent to setting the base option in Migratio config. |

## Get-MgUsedVariables

Returns all [variable](#variables) keys that exists in a script.

```powershell
> Get-MgUsedVariables -MigrationFile "/dev/project/migrations/rollout/20210210_171649_add_default_config_values.sql" [
    -ConfigFile "/dev/project/migratio.yml"
  ]
```

### Input

| Option        | Type   | From config | Mandatory | Default | Comment                                                                            |
| ------------- | ------ | ----------- | --------- | ------- | ---------------------------------------------------------------------------------- |
| ConfigFile    | string | No          | No        | None    | Path to Migratio configuration file. See [Configuration File](#configuration-file) |
| MigrationFile | string | No          | Yes       | None    | File path to the SQL migration to get used variables for                           |

## Get-MgLatestIteration

Get the latest applied iteration. Starts at 1 and increments with 1 for each rollout.

```powershell
> Get-MgLatestIteration [
    -Username "dbuser"
    -Database "MyDb"
    -Port 1234
    -Host "localhost"
    -Schema "db"
  ]
```

### Input

| Option     | Type   | From config | Mandatory | Default     | Comment                                                                                                |
| ---------- | ------ | ----------- | --------- | ----------- | ------------------------------------------------------------------------------------------------------ |
| ConfigFile | string | No          | No        | None        | Path to Migratio configuration file. See [Configuration File](#configuration-file)                     |
| Username   | string | Yes         | Yes\*     | None        | Username of database user                                                                              |
| Database   | string | Yes         | Yes\*     | None        | Specifies the name of the database to connect to                                                       |
| Port       | int    | Yes         | Yes\*     | `5432`      | Specifies the port on the database server to connect to                                                |
| Host       | string | Yes         | Yes\*     | `127.0.0.1` | Specifies the hostname or ip address of the machine to connect to                                      |
| Schema     | string | Yes         | Yes\*     | `public`    | Specifies the default database schema. Only valid for Postgres                                         |
| Password   | string | Yes         | Yes       | None        | Password for the database user. Only settable from configuration file or env variable `MG_DB_PASSWORD` |

`*Mandatory if no configuration file path is given. If both is given, CLI values will override configuration file`

## Get-MgProcessedMigrations

Get a list of all applied migrations and the iteration they were applied in.

```powershell
> Get-MgProcessedMigrations [
    -ConfigFile "/dev/project/migratio.yml"
    -Username "dbuser"
    -Database "MyDb"
    -Port 1234
    -Host "localhost"
    -Schema "db"
  ]
```

### Input

| Option     | Type   | From config | Mandatory | Default     | Comment                                                                                                |
| ---------- | ------ | ----------- | --------- | ----------- | ------------------------------------------------------------------------------------------------------ |
| ConfigFile | string | No          | No        | None        | Path to Migratio configuration file. See [Configuration File](#configuration-file)                     |
| Username   | string | Yes         | Yes\*     | None        | Username of database user                                                                              |
| Database   | string | Yes         | Yes\*     | None        | Specifies the name of the database to connect to                                                       |
| Port       | int    | Yes         | Yes\*     | `5432`      | Specifies the port on the database server to connect to                                                |
| Host       | string | Yes         | Yes\*     | `127.0.0.1` | Specifies the hostname or ip address of the machine to connect to                                      |
| Schema     | string | Yes         | Yes\*     | `public`    | Specifies the default database schema. Only valid for Postgres                                         |
| Password   | string | Yes         | Yes       | None        | Password for the database user. Only settable from configuration file or env variable `MG_DB_PASSWORD` |

`*Mandatory if no configuration file path is given. If both is given, CLI values will override configuration file`

## Get-MgScriptsForLatestIteration

Get a list of all applied migrations for the latest iteration.

```powershell
> Get-MgScriptsForLatestIteration [
    -ConfigFile "/dev/project/migratio.yml"
    -Username "dbuser"
    -Database "MyDb"
    -Port 1234
    -Host "localhost"
    -Schema "db"
  ]
```

### Input

| Option     | Type   | From config | Mandatory | Default     | Comment                                                                                                |
| ---------- | ------ | ----------- | --------- | ----------- | ------------------------------------------------------------------------------------------------------ |
| ConfigFile | string | No          | No        | None        | Path to Migratio configuration file. See [Configuration File](#configuration-file)                     |
| Username   | string | Yes         | Yes\*     | None        | Username of database user                                                                              |
| Database   | string | Yes         | Yes\*     | None        | Specifies the name of the database to connect to                                                       |
| Port       | int    | Yes         | Yes\*     | `5432`      | Specifies the port on the database server to connect to                                                |
| Host       | string | Yes         | Yes\*     | `127.0.0.1` | Specifies the hostname or ip address of the machine to connect to                                      |
| Schema     | string | Yes         | Yes\*     | `public`    | Specifies the default database schema. Only valid for Postgres                                         |
| Password   | string | Yes         | Yes       | None        | Password for the database user. Only settable from configuration file or env variable `MG_DB_PASSWORD` |

`*Mandatory if no configuration file path is given. If both is given, CLI values will override configuration file`

## Invoke-MgRollout

Run a rollout of scripts that has not been applied.

```powershell
> Invoke-MgRollout [
    -ConfigFile "/dev/project/migratio.yml"
    -MigrationRootDir "/path/to/dir"
    -ReplaceVariables
    -CreateTableIfNotExist
    -Username "dbuser"
    -Database "MyDb"
    -Port 1234
    -Host "localhost"
    -Schema "db"
  ]
```

### Input

| Option                | Type   | From config | Mandatory | Default     | Comment                                                                                                                                 |
| --------------------- | ------ | ----------- | --------- | ----------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| ConfigFile            | string | No          | No        | None        | Path to Migratio configuration file. See [Configuration File](#configuration-file)                                                      |
| MigrationRootDir      | string | Yes         | No        | None        | Specifies the root directory of migrations if using default directory naming. Equivalent to setting the base option in Migratio config. |
| ReplaceVariables      | bool   | Yes         | No        | False       | Replace variables in migrations during rollout                                                                                          |
| CreateTableIfNotExist | bool   | No          | No        | False       | Create the migration table if it does not exist in the database.                                                                        |
| Username              | string | Yes         | Yes\*     | None        | Username of database user                                                                                                               |
| Database              | string | Yes         | Yes\*     | None        | Specifies the name of the database to connect to                                                                                        |
| Port                  | int    | Yes         | Yes\*     | `5432`      | Specifies the port on the database server to connect to                                                                                 |
| Host                  | string | Yes         | Yes\*     | `127.0.0.1` | Specifies the hostname or ip address of the machine to connect to                                                                       |
| Schema                | string | Yes         | Yes\*     | `public`    | Specifies the default database schema. Only valid for Postgres                                                                          |
| Password              | string | Yes         | Yes       | None        | Password for the database user. Only settable from configuration file or env variable `MG_DB_PASSWORD`                                  |

`*Mandatory if no configuration file path is given. If both is given, CLI values will override configuration file`

## Invoke-MgRollback

Run rollback scripts for the latest iteration

```powershell
> Invoke-MgRollback [
    -ConfigFile "/dev/project/migratio.yml"
    -MigrationRootDir "/path/to/dir"
    -Username "dbuser"
    -Database "MyDb"
    -Port 1234
    -Host "localhost"
    -Schema "db"
  ]
```

### Input

| Option           | Type   | From config | Mandatory | Default     | Comment                                                                                                                                 |
| ---------------- | ------ | ----------- | --------- | ----------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| ConfigFile       | string | No          | No        | None        | Path to Migratio configuration file. See [Configuration File](#configuration-file)                                                      |
| MigrationRootDir | string | Yes         | No        | None        | Specifies the root directory of migrations if using default directory naming. Equivalent to setting the base option in Migratio config. |
| Username         | string | Yes         | Yes\*     | None        | Username of database user                                                                                                               |
| Database         | string | Yes         | Yes\*     | None        | Specifies the name of the database to connect to                                                                                        |
| Port             | int    | Yes         | Yes\*     | `5432`      | Specifies the port on the database server to connect to                                                                                 |
| Host             | string | Yes         | Yes\*     | `127.0.0.1` | Specifies the hostname or ip address of the machine to connect to                                                                       |
| Schema           | string | Yes         | Yes\*     | `public`    | Specifies the default database schema. Only valid for Postgres                                                                          |
| Password         | string | Yes         | Yes       | None        | Password for the database user. Only settable from configuration file or env variable `MG_DB_PASSWORD`                                  |

`*Mandatory if no configuration file path is given. If both is given, CLI values will override configuration file`

---
