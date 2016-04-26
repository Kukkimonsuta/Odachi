# !! Commands were reworked into tools in RC2, this needs to be updated and doesn't work right now. !!

# Odachi.Localization.Extraction.Commands

`xgettext`-like command line interface using `Odachi.Localization.Extraction`.

## How to install

To install run: `dnu commands install Odachi.Localization.Extraction.Commands`

To install nightly version run: `dnu commands install Odachi.Localization.Extraction.Commands --fallbacksource https://ci.appveyor.com/nuget/odachi`

This will make command `chi-extract-locale` available in your system.

## How to use

### Command line

`chi-extract-locale [options] input [input2 [inputN]]`

* `input` Path to files that should be analyzed. Wildcards: `?`, `*`, `**`, `[regex range]`, `{group1[,group2[,groupN]]}`
* `-l, --input-format` Ignore file extensions and use specified file analyzer (`.cs`, `.cshtml`).
* `-k, --keyword` Use specified keyword.
 * Defaults: `GetString` `GetPluralString:1,2` `GetParticularString:1c,2` `GetParticularPluralString:1c,2,3`
 * Specifying at least one keyword removes all defaults.
* `-o, --output` Output to specified file instead of std out.
* `-f, --output-format` Output format (`.pot`, `.resx`)
 * Note: `.resx` supports only singular forms.

### Poedit

`File` > `Preferences` > `Extractors` > `New`

![Poedit configuration](https://www.dropbox.com/s/9x9mr9ijkaci0oa/Screenshot%202015-12-02%2002.24.30.png?dl=1)

Poedit can already handle `C#` files, however if you need to localize attributes with named parameters (for instance `[Display(Name = "Localize me")]`) you can configure the language "C#" in the same way.