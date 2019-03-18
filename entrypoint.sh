#!/bin/bash

ls
set -e
run_cmd="dotnet run --urls http://*:80 --no-launch-profile"

cd src/SC.DevChallenge.Api
until dotnet ef database update; do
>&2 echo "SQL Server is starting up"
sleep 1
done

>&2 echo "SQL Server is up - executing command"
exec $run_cmd
