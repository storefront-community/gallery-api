#!/bin/bash

set -e

tar_gz="https://dotnetcli.blob.core.windows.net/dotnet/Sdk/2.2.401/dotnet-sdk-2.2.401-linux-x64.tar.gz"

curl -SL -o dotnet.tar.gz $tar_gz
sudo mkdir -p /usr/share/dotnet
sudo tar -zxf dotnet.tar.gz -C /usr/share/dotnet
sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
