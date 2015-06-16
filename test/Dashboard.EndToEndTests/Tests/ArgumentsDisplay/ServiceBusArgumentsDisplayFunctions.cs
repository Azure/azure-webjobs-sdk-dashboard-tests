// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace Dashboard.EndToEndTests
{
    public class ServiceBusArgumentsDisplayFunctions
    {
        public const string StartQueueName = "servicebus-start";
        public const string FirstOutQueue = "servicebus-output1";
        public const string TopicName = "servicebus-topic";
        public const string SubscriptionName = "servicebus-topic-sub";
        public const string StartMessageText = "E2E";

        // this is checked at the end of the test
        public static string ResultMessage;

        public static readonly MethodInfo SBQueue2SBQueueMethodInfo = typeof(ServiceBusArgumentsDisplayFunctions).GetMethod("SBQueue2SBQueue");
        public static readonly MethodInfo SBQueue2SBTopicMethodInfo = typeof(ServiceBusArgumentsDisplayFunctions).GetMethod("SBQueue2SBTopic");
        public static readonly MethodInfo SBTopicListenerMethodInfo = typeof(ServiceBusArgumentsDisplayFunctions).GetMethod("SBTopicListener");

        // Passes a service bus message from a queue to another queue
        public static void SBQueue2SBQueue(
            [ServiceBusTrigger(StartQueueName)] string start,
            [ServiceBus(FirstOutQueue)] out string message)
        {
            message = start + "-SBQueue2SBQueue";
        }

        // Passes a service bus message from a queue to topic using a brokered message
        public static void SBQueue2SBTopic(
            [ServiceBusTrigger(FirstOutQueue)] string message,
            [ServiceBus(TopicName)] out BrokeredMessage output)
        {
            message = message + "-SBQueue2SBTopic";

            Stream stream = new MemoryStream();
            TextWriter writer = new StreamWriter(stream);
            writer.Write(message);
            writer.Flush();
            stream.Position = 0;

            output = new BrokeredMessage(stream)
            {
                ContentType = "text/plain"
            };
        }

        public static void SBTopicListener(
            [ServiceBusTrigger(TopicName, SubscriptionName)] string message,
            [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            ResultMessage = message + "-SBTopicListener";
            done = "x";
        }
    }
}
