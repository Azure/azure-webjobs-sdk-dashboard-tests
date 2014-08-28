// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Dashboard.EndToEndTests.Infrastructure
{
    public class WebJobsStorageAccount
    {
        private const string OldHostContainerName = "azure-jobs-dashboard-hosts";

        private readonly string _connectionString;
        private readonly CloudStorageAccount _storageAccount;

        public WebJobsStorageAccount(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("connectionString");
            }

            _connectionString = connectionString;
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public CloudStorageAccount CloudStorageAccount
        {
            get
            {
                return _storageAccount;
            }
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public void CreateOldHostContainer()
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(OldHostContainerName);
            container.CreateIfNotExists();
        }

        public void RemoveOldHostContainer()
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(OldHostContainerName);
            container.DeleteAndWaitForCompletion();
        }

        /// <summary>
        /// Deletes all the artifacts from the storage account
        /// </summary>
        /// <remarks>It waits for the artifacts to be deleted</remarks>
        public void Empty()
        {
            List<Task> allDeleteTasks = new List<Task>();

            allDeleteTasks.AddRange(DeleteAllContainers());
            allDeleteTasks.AddRange(DeleteAllQueues());
            allDeleteTasks.AddRange(DeleteAllTables());

            Task.WaitAll(allDeleteTasks.ToArray());
        }

        private IEnumerable<Task> DeleteAllContainers()
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            IEnumerable<CloudBlobContainer> allContainers = blobClient.ListContainers();
            return allContainers.Select(c => Task.Run(() => c.DeleteAndWaitForCompletion()));
        }

        private IEnumerable<Task> DeleteAllQueues()
        {
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            IEnumerable<CloudQueue> allQueues = queueClient.ListQueues();
            return allQueues.Select(q => Task.Run(() => q.DeleteAndWaitForCompletion()));
        }

        private IEnumerable<Task> DeleteAllTables()
        {
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            IEnumerable<CloudTable> allTables = tableClient.ListTables();
            return allTables.Select(t => Task.Run(() => t.DeleteAndWaitForCompletion()));
        }
    }
}
