#!/bin/bash

if [ $# -ne 2 ]
then
	echo "Usage: $0 {Rebuild|Build} {Release|Debug}"
	exit 1
fi

command=$1
target=$2

if [ "$target" = Release ]; then
  echo Targeg is Release!!
elif [ "$target" = Debug ]; then
  echo Targeg is Debug!!
else
  echo "Invalid Target type. You can use {Release|Debug}."
  exit 2
fi

if [ "$command" = Rebuild ]; then
  docker rm hakoniwa-core-builder
  docker run -t -v "$(pwd)":/hakoniwa-core --name hakoniwa-core-builder mcr.microsoft.com/dotnet/sdk:5.0 /bin/bash -c "
    cp -rf ./hakoniwa-core ./.hakoniwa-core
    dotnet restore ./.hakoniwa-core/impl/asset/server/csharp/HakoniwaCore/Hakoniwa.csproj
    dotnet build --configuration ${target} --output /hakoniwa-core/dst ./.hakoniwa-core/impl/asset/server/csharp/HakoniwaCore/Hakoniwa.csproj
    "
elif [ "$command" = Build ]; then
  docker start hakoniwa-core-builder
  docker exec -it hakoniwa-core-builder /bin/bash -c "
    cp -rf ./hakoniwa-core ./.hakoniwa-core
    dotnet build --configuration ${target} --output /hakoniwa-core/dst ./.hakoniwa-core/impl/asset/server/csharp/HakoniwaCore/Hakoniwa.csproj
    "
else
  echo "Invalid Command type. You can use {Rebuild|Build}."
  exit 3
fi