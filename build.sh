#!/bin/bash

dnvm use default
dnu restore
dnu build ./src/* ./samples/* --framework dnxcore50 --configuration Release