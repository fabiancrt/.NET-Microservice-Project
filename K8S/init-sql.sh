#!/bin/bash
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Fabian20MSSQL" -Q "ALTER LOGIN sa WITH PASSWORD = 'Fabian20MSSQL';"
