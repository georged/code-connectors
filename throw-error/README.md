
# Throw Error custom connector for Power Platform

This is simple code-based connector where the method is entirely contained within the custom code block.

The purpose of the connector is to throw an error with a custom message and status code based on the inputs provided. Its intended use is to make the flow fail on custom business logic. It should be used in flows with appropriate error handling in place.

## Installation

There are two ways to install the connector:

### Manual

1. Download [script.csx](./script.csx) (click on **... > Download** in the top right-hand corner).
2. Sign in to [https://make.powerautomate.com](https://make.powerautomate.com).
3. Select target environment.
4. Select **Custom connectors** in the left navigation.
5. Select **+ New customer connector > Import an OpenAPI from URL**.
   * Enter **Throw Error** as Connector name.
   * Copy and paste the direct URL to [apiDefinition.json](./apiDefinition.json)
6. Select **Import** then select **Continue**.
7. Select **Code** tab in the navigation dropdown.
8. Flip the switch to **Code Enabled**.
9. Select **Upload** and upload **script.csx** saved earlier.
10. Select **ThrowError** in the list of operations.
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
