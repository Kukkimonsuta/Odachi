#!/bin/bash

dnvm use default
dnu restore
dnu build ./src/* ./samples/* --framework dotnet5.4 --configuration Release