#!/bin/bash

set -e

set -a
source $1
set +a

sqldir="$(dirname $0)/sql"

close_connections_sql=$(<$sqldir/close_connections.sql)
drop_database_sql=$(<$sqldir/drop_database.sql)

psql -h $PGHOST -p $PGPORT -U $PGUSER -c "${close_connections_sql//p_dbname/$DBNAME}"
psql -h $PGHOST -p $PGPORT -U $PGUSER -c "${drop_database_sql//p_dbname/$DBNAME}"
