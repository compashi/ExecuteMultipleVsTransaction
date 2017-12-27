using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System.Diagnostics;

namespace ExecuteMultipleVsTransaction
{
    class Program
    {
        private static CrmServiceClient crmSvc = null;
        private static int numberOfBatches = int.Parse(ConfigurationManager.AppSettings["NumberOfBatches"]);
        private static int recordsPerBatch = int.Parse(ConfigurationManager.AppSettings["RecordsPerBatch"]);

        static void Main(string[] args)
        {
            Connect();

            Console.WriteLine("Testing ExecuteMultiple & ExecuteTransaction for {0} cycles with {1} records per cycle.\n",
                numberOfBatches, recordsPerBatch);

            // Execute batches with ExecuteMultiple
            Console.WriteLine("Running ExecuteMultiple");
            decimal emRuntime = ExecuteBatch("ExecuteMultiple");

            // Execute batches with ExecuteTransaction
            Console.WriteLine("Running ExecuteTransaction");
            decimal etRuntime = ExecuteBatch("ExecuteTransaction");

            // Show simple statistcs
            Console.WriteLine("ExecuteMultiple:     {0}ms per batch (avg)", emRuntime / numberOfBatches);
            Console.WriteLine("                     {0:0.00} records per second", 
                ((numberOfBatches * recordsPerBatch) / emRuntime) * 1000);

            Console.WriteLine("ExecuteTransaction:  {0}ms per batch (avg)", etRuntime / numberOfBatches);
            Console.WriteLine("                     {0:0.00} records per second\n", 
                ((numberOfBatches * recordsPerBatch) / etRuntime) * 1000);

            Console.WriteLine("ExecuteTransaction vs. ExecuteMultiple: {0:0.00}%\n", 
                (100 - (etRuntime / emRuntime) * 100));

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        private static void Connect()
        {
            crmSvc = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString);

            if (crmSvc.IsReady)
            {
                Console.WriteLine("Connected to {0} ({1}), Version {2}", crmSvc.ConnectedOrgFriendlyName, 
                    crmSvc.ConnectedOrgUniqueName, crmSvc.ConnectedOrgVersion);
            }
            else
            {
                Console.WriteLine("Could not connect to Dynamics 365: {0}", crmSvc.LastCrmError);
                System.Environment.Exit(1);
            }
        }

        private static decimal ExecuteBatch(String batchType)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (batchType.Equals("ExecuteMultiple"))
            {
                for (int i = 0; i < numberOfBatches; i++)
                {
                    CreateAccountsExecuteMultiple(crmSvc);
                }
            }
            else
            {
                for (int i = 0; i < numberOfBatches; i++)
                {
                    CreateAccountsExecuteTransaction(crmSvc);
                }
            }

            sw.Stop();

            Console.WriteLine("{0} took {1}ms per batch (avg)\n", batchType, sw.ElapsedMilliseconds / numberOfBatches);

            return sw.ElapsedMilliseconds;
        }

        private static void CreateAccountsExecuteMultiple(CrmServiceClient crmSvc)
        {
            // https://msdn.microsoft.com/en-us/library/jj863604.aspx
            ExecuteMultipleRequest executeMultipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            EntityCollection preparedRecords = PrepareRecordsToCreate();
            foreach (var entity in preparedRecords.Entities)
            {
                CreateRequest createRequest = new CreateRequest { Target = entity };
                executeMultipleRequest.Requests.Add(createRequest);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            crmSvc.Execute(executeMultipleRequest);
            sw.Stop();

            Console.WriteLine("Creating {0} Accounts took {1}ms", recordsPerBatch, sw.ElapsedMilliseconds);

        }

        private static void CreateAccountsExecuteTransaction(CrmServiceClient crmSvc)
        {
            // https://msdn.microsoft.com/en-us/library/mt634414.aspx
            ExecuteTransactionRequest executeTransactionRequest = new ExecuteTransactionRequest()
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };

            EntityCollection preparedRecords = PrepareRecordsToCreate();
            foreach (var entity in preparedRecords.Entities)
            {
                CreateRequest createRequest = new CreateRequest { Target = entity };
                executeTransactionRequest.Requests.Add(createRequest);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            crmSvc.Execute(executeTransactionRequest);
            sw.Stop();
            Console.WriteLine("Creating {0} Accounts took {1}ms", recordsPerBatch, sw.ElapsedMilliseconds);
        }


        private static EntityCollection PrepareRecordsToCreate()
        {
            List<Entity> accounts = new List<Entity>();
            for (int i = 0; i < recordsPerBatch; i++)
            {
                Entity account = new Entity("account");
                account["name"] = "Sample Account";
                account["address1_city"] = "Berlin";
                account["address1_country"] = "Germany";
                account["description"] = "A Sample Account";
                accounts.Add(account);
            }

            EntityCollection entities = new EntityCollection(accounts);
            return entities;
        }
    }
}