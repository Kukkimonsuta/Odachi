#!/bin/bash

# source: https://github.com/enricosada/fsharp-dotnet-cli-samples

# Download script to install dotnet cli
curl -L --create-dirs "https://raw.githubusercontent.com/dotnet/cli/v1.0.0-preview2/scripts/obtain/dotnet-install.sh" -o ./tools/dotnet-install.sh
find ./tools -name "*.sh" -exec chmod +x {} \;

export DOTNET_INSTALL_DIR="$PWD/.dotnetcli"
# use bash to workaround bug https://github.com/dotnet/cli/issues/1725
sudo bash ./tools/dotnet-install.sh --channel preview --version 1.0.0-preview2-003121 --install-dir "$DOTNET_INSTALL_DIR" --no-path --verbose
# add dotnet to PATH
export PATH="$DOTNET_INSTALL_DIR:$PATH"