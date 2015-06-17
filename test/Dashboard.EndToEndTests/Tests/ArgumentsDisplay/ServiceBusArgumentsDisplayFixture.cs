// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Dashboard.EndToEndTests
{
    public class ServiceBusArgumentsDisplayTestsFixture : DashboardTestFixture
    {
        private NamespaceManager _namespaceManager;
        private JobHostConfiguration _hostConfiguration;
        private string _serviceBusConnectionString;

        public ServiceBusArgumentsDisplayTestsFixture()
            : base(cleanStorageAccount: true)
        {
            _hostConfiguration = new JobHostConfiguration(StorageAccount.ConnectionString)
            {
                TypeLocator = new ExplicitTypeLocator(
                    typeof(ServiceBusArgumentsDisplayFunctions),
                    typeof(DoneNotificationFunction))
            };

#if VNEXT_SDK
            ServiceBusConfiguration serviceBusConfig = new ServiceBusConfiguration();
            serviceBusConfig.ConnectionString = ServiceBusAccount;
            _serviceBusConnectionString = serviceBusConfig.ConnectionString;
            _hostConfiguration.UseServiceBus(serviceBusConfig);
#else
            _serviceBusConnectionString = ServiceBusAccount;
            _hostConfiguration.ServiceBusConnectionString = _serviceBusConnectionString;
#endif

            _namespaceManager = NamespaceManager.CreateFromConnectionString(_serviceBusConnectionString);

            // ensure we're starting in a clean state
            CleanServiceBusQueues();

            // now run the entire end to end scenario, causing all the job functions
            // to be invoked
            RunEndToEnd();
        }

        private void RunEndToEnd()
        {
            // create the initial messgage that starts the function chain
            CreateStartMessage();

            using (JobHost host = new JobHost(_hostConfiguration))
            using (DoneNotificationFunction._doneEvent = new ManualResetEvent(initialState: false))
            {
                host.Start();
                DoneNotificationFunction._doneEvent.WaitOne();
                host.Stop();
            }
        }

        private void CreateStartMessage()
        {
            _namespaceManager.CreateQueue(ServiceBusArgumentsDisplayFunctions.StartQueueName);

            QueueClient queueClient = QueueClient.CreateFromConnectionString(_serviceBusConnectionString, ServiceBusArgumentsDisplayFunctions.StartQueueName);

            using (Stream stream = new MemoryStream())
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.Write(ServiceBusArgumentsDisplayFunctions.StartMessageText);
                writer.Flush();
                stream.Position = 0;

                queueClient.Send(new BrokeredMessage(stream) { ContentType = "text/plain" });
            }

            queueClient.Close();
        }

        private void CleanServiceBusQueues()
        {
            if (_namespaceManager.QueueExists(ServiceBusArgumentsDisplayFunctions.StartQueueName))
            {
                _namespaceManager.DeleteQueue(ServiceBusArgumentsDisplayFunctions.StartQueueName);
            }

            if (_namespaceManager.QueueExists(ServiceBusArgumentsDisplayFunctions.FirstOutQueue))
            {
                _namespaceManager.DeleteQueue(ServiceBusArgumentsDisplayFunctions.FirstOutQueue);
            }

            if (_namespaceManager.SubscriptionExists(ServiceBusArgumentsDisplayFunctions.TopicName, ServiceBusArgumentsDisplayFunctions.SubscriptionName))
            {
                _namespaceManager.DeleteSubscription(ServiceBusArgumentsDisplayFunctions.TopicName, ServiceBusArgumentsDisplayFunctions.SubscriptionName);
            }

            if (_namespaceManager.TopicExists(ServiceBusArgumentsDisplayFunctions.TopicName))
            {
                _namespaceManager.DeleteTopic(ServiceBusArgumentsDisplayFunctions.TopicName);
            }
        }
    }
}
