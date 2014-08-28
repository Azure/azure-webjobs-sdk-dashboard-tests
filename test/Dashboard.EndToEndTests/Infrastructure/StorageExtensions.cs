// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Dashboard.EndToEndTests.Infrastructure
{
    public static class StorageExtensions
    {
        public static void DeleteAndWaitForCompletion(this CloudBlobContainer container)
        {
            RepeatUntilNotFound(() => container.Delete());
        }

        public static void DeleteAndWaitForCompletion(this CloudQueue queue)
        {
            RepeatUntilNotFound(() => queue.Delete());
        }

        public static void DeleteAndWaitForCompletion(this CloudTable table)
        {
            RepeatUntilNotFound(() => table.Delete());
        }

        private static void RepeatUntilNotFound(Action action)
        {
            RepeatUntilNotFound(action, TimeSpan.FromSeconds(60));
        }

        private static void RepeatUntilNotFound(Action action, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime) <= timeout)
            {
                try
                {
                    action();
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode == 404)
                    {
                        return;
                    }
                }

                Thread.Sleep(1000);
            }

            throw new TimeoutException("The item is still there after timeout");
        }
    }
}
