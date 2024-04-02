# CSV to JSON custom connector for Power Platform

This is simple code-based connector where the method is entirely contained within the custom code block. 

Purpose of the connector is to take CSV input and return JSON output equivalent. The implementation is extremely simple and is partially based on [this Stack Overflow answer](https://stackoverflow.com/a/14198311/70347). Well-formatted CSV with the first line containing the headers is expected. Double-quoted entries are permitted and can span multiple lines.

There are two additional flags available:

* Trim whitespace. This is yet to be implemented, the idea is to trim resulting entries but to leave double-quoted entries alone.
* Skip blank lines. It's not unusual for CSV files saved from Excel to contain lines containing black entries, i.e. series of commas. This flag skips the lines that do not contain any data.

## Installation

There are two ways to install the connector:

### Manual

1. Download [script.csx](https://github.com/georged/code-connectors/blob/main/csv-to-json/script.csx) (click on **... > Download** in the top right-hand corner).
2. Sign in to [https://make.powerapps.com](https://make.powerapps.com).
3. Select target environment.
4. Select **Custom connectors** in the left navigation.
5. Select **+ New customer connector > Import an OpenAPI from URL**.
   * Enter **CSV Magic** as Connector name.
   * Copy and paste this URL: `https://raw.githubusercontent.com/georged/code-connectors/main/csv-to-json/apiDefinition.json` 
6. Select **Import** then select **Continue**.
7. Select **Code** in the navigation dropdown.
8. Flip the switch to **Code Enabled**.
9. Select **Upload** and upload **script.csx** saved earlier.
10. Select **CsvToJson** in the list of operations.
11. Select **Create connector**.

### Power Platform CLI (recommended)

What do you need?

* Audacity to use command line
* [Microsoft Power Platform CLI](https://learn.microsoft.com/power-platform/developer/cli/introduction)

#### Steps

1. Create auth profile if you don't have one already and make it active.

   ```shell
   pac auth create -n Code -u https://yoururl.crmN.dynamics.com
   pac auth select -n Code
   ```

1. Upload custom connector

   ```shell
   pac connector create --settings-file settings.json
   ```