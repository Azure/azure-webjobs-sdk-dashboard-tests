// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dashboard.EndToEndTests.Infrastructure.DashboardData;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Dashboard.EndToEndTests.Infrastructure
{
    public class WebJobsStorageAccount
    {
        public const string DashboardContainerName = "azure-jobs-dashboard";
        public const string HostsContainerName = "azure-jobs-hosts";
        public const string HostOutputContainerName = "azure-jobs-host-output";
        public const string OldHostContainerName = "azure-jobs-dashboard-hosts";

        private const string FunctionsByFunctionIndexPrefix = "functions/recent/by-function/";
        private const string FunctionsInstancesIndexPrefix = "functions/instances/";
        private const string HostsIndexPrefix = "hosts/azure-jobs-host-";
        private const string HostIdsIndexPrefix = "ids";

        private readonly string _connectionString;
        private readonly CloudStorageAccount _storageAccount;

        private readonly CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _dashboardContainer;
        private readonly CloudBlobContainer _hostsContainer;

        public WebJobsStorageAccount(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("connectionString");
            }

            _connectionString = connectionString;
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _dashboardContainer = _blobClient.GetContainerReference(DashboardContainerName);
            _hostsContainer = _blobClient.GetContainerReference(HostsContainerName);
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
            CloudBlobContainer container = _blobClient.GetContainerReference(OldHostContainerName);
            container.CreateIfNotExists();
        }

        public void RemoveOldHostContainer()
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(OldHostContainerName);
            container.DeleteAndWaitForCompletion();
        }

        public IEnumerable<InvocationDetails> MethodInfoToInvocations(MethodInfo methodInfo)
        {
            string functionDefinitionId = MethodInfoToFunctionDefinitionId(methodInfo);
            if (functionDefinitionId == null)
            {
                yield break;
            }

            IEnumerable<IListBlobItem> invocationIndices = _dashboardContainer.ListBlobs(
                FunctionsByFunctionIndexPrefix + functionDefinitionId,
                useFlatBlobListing: true);
            foreach (CloudBlockBlob invocationIndexBlob in invocationIndices.OfType<CloudBlockBlob>())
            {
                string invocationId = invocationIndexBlob.Name.Split('_').Last();

                CloudBlockBlob invocationBlob = _dashboardContainer.GetBlockBlobReference(FunctionsInstancesIndexPrefix + invocationId);
                if (!invocationBlob.Exists())
                {
                    continue;
                }

                string blobContent = invocationBlob.DownloadText();
                yield return JsonConvert.DeserializeObject<InvocationDetails>(blobContent);
            }
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

            if (!_dashboardContainer.Exists())
            {
                return null;
            }

            CloudBlockBlob hostBlob = _dashboardContainer.GetBlockBlobReference(HostsIndexPrefix + hostId);
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

            if (!_hostsContainer.Exists())
            {
                return null;
            }

            CloudBlockBlob hostBlob = _hostsContainer.GetBlockBlobReference(string.Format(HostIdsIndexPrefix + "/{0}/{1}",
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
