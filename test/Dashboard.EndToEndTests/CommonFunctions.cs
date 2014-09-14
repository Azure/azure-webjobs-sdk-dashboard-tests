// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Azure.WebJobs;

namespace Dashboard.EndToEndTests
{
    public class SingleFunction
    {
        public static readonly MethodInfo FunctionMethodInfo = typeof(SingleFunction).GetMethod("Function");

        [NoAutomaticTrigger]
        public static void Function(bool fail, bool logOnSuccess, TextWriter log)
        {
            if (fail)
            {
                log.WriteLine("This function will always fail in this case");
                throw new InvalidOperationException();
            }
            else if (logOnSuccess)
            {
                log.WriteLine("logOnSuccess is enabled");
            }
        }
    }

    public class DoneNotificationFunction
    {
        public const string DoneQueueName = "queue-done";

        public static ManualResetEvent _doneEvent;

        public static void DoneTrigger(
            [QueueTrigger(DoneQueueName)] string message)
        {
            _doneEvent.Set();
        }
    }

    public class POCO
    {
        public const string JsonSample = @"{""StringValue"":""str"",""IntValue"":42}";

        public string StringValue { get; set; }
        public int IntValue { get; set; }
    }
}
