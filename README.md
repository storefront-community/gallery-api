# Gallery API

Restaurant gallery to present dish pictures.

This is a RESTful API developed with ASP.NET Core MVC using RabbitMQ as the event bus and Amazon S3 as the file storage.

## Status

[![CircleCI](https://circleci.com/gh/storefront-community/gallery-api.svg?style=shield)](https://circleci.com/gh/storefront-community/gallery-api)
[![codecov](https://codecov.io/gh/storefront-community/gallery-api/branch/master/graph/badge.svg)](https://codecov.io/gh/storefront-community/gallery-api)

## Documentation

API documentation written with Swagger and available at the root URL (no route prefix).

The API requires the native OS dependency `ligdiplus` to resize the images.

```bash
sudo apt-get install libgdiplus
```

## Debug locally

Before you start:

- Install [.NET Core SDK](https://dotnet.microsoft.com/)
- Install [PostgreSQL](https://www.postgresql.org/) or
  [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Install [VS Code](https://code.visualstudio.com/) (or your preferred editor)

To create/drop a PotgreSQL database on Linux:

```bash
# Create database
bash .sh/db/postgresql/create.sh /path/to/file.conf

# Drop database
bash .sh/db/postgresql/drop.sh /path/to/file.conf
```

```bash
#file.conf

PGHOST=""
PGPORT=""
PGUSER=""
PGPASSWORD=""
DBNAME=""
```

## Bugs and features

Please, fell free to [open a new issue](https://github.com/storefront-community/gallery-api/issues) on GitHub.

## License

Code released under the [Apache License 2.0](https://github.com/storefront-community/gallery-api/blob/master/LICENSE)

Copyright (c) 2019-present, [Marx J. Moura](https://github.com/marxjmoura)
