#!/bin/sh

mkdir /app/data
chown medlink: /app/data
chmod 700 /app/data

su medlink -s /bin/sh -c 'dotnet medlink.dll --urls=http://0.0.0.0:5001/'
