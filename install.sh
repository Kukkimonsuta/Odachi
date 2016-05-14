#!/bin/bash

# source: https://github.com/enricosada/fsharp-dotnet-cli-samples

# Download script to install dotnet cli
curl -L --create-dirs https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview1/scripts/obtain/install.sh -o ./tools/install.sh
find ./tools -name "*.sh" -exec chmod +x {} \;

export DOTNET_INSTALL_DIR="$PWD/.dotnetcli"
# use bash to workaround bug https://github.com/dotnet/cli/issues/1725
sudo bash ./tools/install.sh --channel "preview" --install-dir "$DOTNET_INSTALL_DIR" --no-path
# add dotnet to PATH
export PATH="$DOTNET_INSTALL_DIR:$PATH"