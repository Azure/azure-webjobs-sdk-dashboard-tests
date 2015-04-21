// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Dashboard.EndToEndTests
{
    public class BlobArgumentsDisplayFixture : DashboardTestFixture
    {
        public BlobArgumentsDisplayFixture()
            : base(cleanStorageAccount: true)
        {
            InvokeBlobFunctionAndWaitForResult(BlobArgumentsDisplayFunctions.CloudBlockBlobMethodInfo);
            InvokeBlobFunctionAndWaitForResult(
                BlobArgumentsDisplayFunctions.POCOMethodInfo,
                triggerMessage: POCO.JsonSample,
                inputMessage: POCO.JsonSample);
            InvokeBlobFunctionAndWaitForResult(BlobArgumentsDisplayFunctions.StreamMethodInfo);
            InvokeBlobFunctionAndWaitForResult(BlobArgumentsDisplayFunctions.StringMethodInfo);
            InvokeBlobFunctionAndWaitForResult(BlobArgumentsDisplayFunctions.TextReaderWriterMethodInfo);
        }

        private void InvokeBlobFunctionAndWaitForResult(
            MethodInfo function, 
            string triggerMessage = null, 
            string inputMessage = null)
        {
            if (triggerMessage ==null)
            {
                triggerMessage = "trigger-content-";
            }
            if (inputMessage ==null)
            {
                inputMessage = "input-content";
            }

            string blobPartialName = function.Name.ToLowerInvariant();

            CloudBlobClient blobClient = StorageAccount.CloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(BlobArgumentsDisplayFunctions.ContainerName);
            container.CreateIfNotExists();

            container
                .GetBlockBlobReference(blobPartialName + "-trigger")
                .UploadText(triggerMessage);
            container
                .GetBlockBlobReference(blobPartialName + "-in")
                .UploadText(inputMessage);

            JobHostConfiguration hostConfiguration = new JobHostConfiguration(StorageAccount.ConnectionString)
            {
                TypeLocator = new ExplicitTypeLocator(
                    typeof(BlobArgumentsDisplayFunctions),
                    typeof(BlobArgumentsDisplayFunctions.POCOBinder),
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
