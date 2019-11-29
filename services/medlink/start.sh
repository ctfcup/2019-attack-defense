#!/bin/sh

mkdir /app/data
chown medlink: /app/data -R
chmod 700 /app/data -R

su medlink -s /bin/sh -c 'dotnet medlink.dll --urls=http://0.0.0.0:5001/'
