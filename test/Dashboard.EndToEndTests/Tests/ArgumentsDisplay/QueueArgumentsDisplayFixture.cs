// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Dashboard.EndToEndTests
{
    public class QueueArgumentsDisplayTestsFixture : DashboardTestFixture
    {
        public QueueArgumentsDisplayTestsFixture()
            : base(cleanStorageAccount: true)
        {
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.ByteMethodInfo);
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.CloudQueueMessageMethodInfo);
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.ICollectorMethodInfo);
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.NullOutputMethodInfo);
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.PocoMethodInfo);
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.StringMethodInfo);
            InvokeQueueFunctionAndWaitForResult(QueueArgumentsDisplayFunctions.SingletonMethodInfo);
        }

        public static string CreateQueueName(MethodInfo function, bool input)
        {
            return "queue-" + function.Name.ToLowerInvariant() + (input ? "-in" : "-out");
        }

        private void InvokeQueueFunctionAndWaitForResult(MethodInfo function, CloudQueueMessage message = null)
        {
            if (message == null)
            {
                message = new CloudQueueMessage(POCO.JsonSample);
            }

            string queueName = CreateQueueName(function, input: true);

            CloudQueueClient queueClient = StorageAccount.CloudStorageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            queue.AddMessage(message);

            JobHostConfiguration hostConfiguration = new JobHostConfiguration(StorageAccount.ConnectionString)
            {
                TypeLocator = new ExplicitTypeLocator(
                    typeof(QueueArgumentsDisplayFunctions),
                    typeof(DoneNotificationFunction))
            };

            using (JobHost host = new JobHost(hostConfiguration))
            using (DoneNotificationFunction._doneEvent = new ManualResetEvent(initialState: false))
            {
                host.Start();
                DoneNotificationFunction._doneEvent.WaitOne();
                host.Stop();
            }
        }
    }
}
