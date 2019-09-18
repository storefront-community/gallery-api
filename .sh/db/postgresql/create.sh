#!/bin/bash

set -e

set -a
source $1
set +a

sqldir="$(dirname $0)/sql"

create_database_sql=$(<$sqldir//create_database.sql)

psql -h $PGHOST -p $PGPORT -U $PGUSER -c "${create_database_sql//p_dbname/$DBNAME}"
