// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Dashboard.EndToEndTests
{
    public class QueueArgumentsDisplayFunctions
    {
        public static readonly MethodInfo NullOutputMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("NullOutput");
        public static readonly MethodInfo ByteMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("Byte");
        public static readonly MethodInfo StringMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("String");
        public static readonly MethodInfo ICollectorMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("ICollector");
        public static readonly MethodInfo PocoMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("POCO");
        public static readonly MethodInfo CloudQueueMessageMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("CloudQueueMessage");
        public static readonly MethodInfo SingletonMethodInfo = typeof(QueueArgumentsDisplayFunctions).GetMethod("Singleton");

        [Singleton("TestScope")]
        public static async Task Singleton(
            [QueueTrigger("queue-singleton-in")] string input,
            [Queue("queue-singleton-out")] IAsyncCollector<string> output,
            [Queue(DoneNotificationFunction.DoneQueueName)] IAsyncCollector<string> done)
        {
            await output.AddAsync(input);
            await done.AddAsync("x");
        }

        public static void NullOutput(
            [QueueTrigger("queue-nulloutput-in")] string input,
            [Queue("queue-nulloutput-out")] out string output,
            [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = null;
            done = "x";
        }

        public static void Byte(
            [QueueTrigger("queue-byte-in")] byte[] input,
            [Queue("queue-byte-out")] out byte[] output,
            [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = input;
            done = "x";
        }

        public static void String(
            [QueueTrigger("queue-string-in")] string input,
            [Queue("queue-string-out")] out string output,
            [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = input;
            done = "x";
        }

        public static async Task ICollector(
            [QueueTrigger("queue-icollector-in")] string input,
            [Queue("queue-icollector-out")] IAsyncCollector<string> output,
            [Queue(DoneNotificationFunction.DoneQueueName)] IAsyncCollector<string> done)
        {
            foreach (char c in input)
            {
                await output.AddAsync(c.ToString());
            }

            await done.AddAsync("x");
        }

        public static void POCO(
           [QueueTrigger("queue-poco-in")] POCO input,
           [Queue("queue-poco-out")] out POCO output,
           [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = input;
            done = "x";
        }

        public static void CloudQueueMessage(
           [QueueTrigger("queue-cloudqueuemessage-in")] CloudQueueMessage input,
           [Queue("queue-cloudqueuemessage-out")] out CloudQueueMessage output,
           [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = input;
            done = "x";
        }
    }
}
