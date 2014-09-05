// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

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

        public string MethodInfoToFunctionDefinitionId(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            string hostId = AssemblyToHostId(methodInfo.DeclaringType.Assembly);
            if (hostId == null)
            {
                return null;
            }

            string methodName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;

            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer dashboardContainer = blobClient.GetContainerReference("azure-jobs-dashboard");
            if (!dashboardContainer.Exists())
            {
                return null;
            }

            CloudBlockBlob hostBlob = dashboardContainer.GetBlockBlobReference("hosts/azure-jobs-host-" + hostId);
            if (!hostBlob.Exists())
            {
                return null;
            }

            string blobContent = hostBlob.DownloadText();

            HostInfo hostInfo = JsonConvert.DeserializeObject<HostInfo>(blobContent);
            FunctionInfo functionInfo = hostInfo.Functions.SingleOrDefault(f => methodName == f.FullName);
            if (functionInfo == null)
            {
                return null;
            }

            return functionInfo.Id;
        }

        public string AssemblyToHostId(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer hostsContainer = blobClient.GetContainerReference("azure-jobs-hosts");
            if (!hostsContainer.Exists())
            {
                return null;
            }

            CloudBlockBlob hostBlob = hostsContainer.GetBlockBlobReference(string.Format("ids/{0}/{1}",
                _storageAccount.Credentials.AccountName,
                assembly.FullName));
            if (!hostBlob.Exists())
            {
                return null;
            }

            string hostId = hostBlob.DownloadText();

            return string.IsNullOrWhiteSpace(hostId) ? null : hostId;
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
