# CSV to JSON custom connector for Power Platform

This is simple code-based connector where the method is entirely contained within the custom code block. 

Purpose of the connector is to take CSV input and return JSON output equivalent. The implementation is extremely simple and is partially based on [this Stack Overflow answer](https://stackoverflow.com/a/14198311/70347). Well-formatted CSV with the first line containing the headers is expected. Double-quoted entries are permitted and can span multiple lines.

There are two additional flags available:

* Trim whitespace. This is yet to be implemented, the idea is to trim resulting entries but to leave double-quoted entries alone.
* Skip blank lines. It's not unusual for CSV files saved from Excel to contain lines containing black entries, i.e. series of commas. This flag skips the lines that do not contain any data.

## Installation

TBD