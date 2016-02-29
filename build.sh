#!/bin/bash

dnvm use default
dnu restore
dnu build ./src/* --framework dotnet5.6 --configuration Release
dnu build ./samples/* --framework dnxcore50 --configuration Release