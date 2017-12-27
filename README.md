# Overview #

The purpose of this example, is to demonstrate a potential difference in Dynamics 365 for Customer Engagement platform performance when performing operations in batches.

The program will create Account records by utilizing ExecuteMultiple & ExecuteTransaction batching.

It will create a configurable number of records in a configurable number of batches - both for ExecuteMultiple & ExecuteTransaction.

#### Getting started

A connection string to your Dynamics 365 organization must be provided in the *App.config* file.

##### Additional configuration
Both settings are per ExecuteMultiple & ExecuteTransaction run.

*NumberOfBatches* - number of batches to run
*RecordsPerBatch* - number of records to create per batch

### Sample output

```shell
Connected to <ORG>, Version 9.0.0.3172
Testing ExecuteMultiple & ExecuteTransaction for 5 cycles with 10 records per cycle.

Running ExecuteMultiple
Creating 10 Accounts took 1387ms
Creating 10 Accounts took 1206ms
Creating 10 Accounts took 1432ms
Creating 10 Accounts took 1228ms
Creating 10 Accounts took 1129ms
ExecuteMultiple took 1281ms per batch (avg)

Running ExecuteTransaction
Creating 10 Accounts took 1022ms
Creating 10 Accounts took 1116ms
Creating 10 Accounts took 1125ms
Creating 10 Accounts took 1126ms
Creating 10 Accounts took 1126ms
ExecuteTransaction took 1105ms per batch (avg)

ExecuteMultiple:     1281,2ms per batch (avg)
                     7,81 records per second
ExecuteTransaction:  1105ms per batch (avg)
                     9,05 records per second

ExecuteTransaction vs. ExecuteMultiple: 13,75%

Done.
```

### Notes

**Do not run this script on any production environment.**

This example is not trying to demonstrate any other preferred practices or optimizations.