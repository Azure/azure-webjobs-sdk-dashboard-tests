// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.Infrastructure;
using Dashboard.EndToEndTests.Infrastructure.DashboardData;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class ServiceBusArgumentsDisplayTests : DashboardTestClass<ServiceBusArgumentsDisplayTestsFixture>
    {
        private WebJobsStorageAccount _storageAccount;

        public ServiceBusArgumentsDisplayTests(ServiceBusArgumentsDisplayTestsFixture fixture)
            : base(fixture)
        {
            Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

            _storageAccount = fixture.StorageAccount;
        }

        [Fact]
        public void VerifySBQueue2SBQueueInvocation()
        {
            FunctionInvocationPage page = GetInvocationPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBQueueMethodInfo);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            Assert.Equal(string.Format("New service bus message detected on '{0}.", ServiceBusArgumentsDisplayFunctions.StartQueueName), page.TriggerReason);

            FunctionArgumentsTableRow argumentRow = rows[0];
            Assert.Equal("start", argumentRow.Name);
            Assert.Equal(ServiceBusArgumentsDisplayFunctions.StartMessageText, argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);

            argumentRow = rows[1];
            Assert.Equal("message", argumentRow.Name);
            Assert.Equal(ServiceBusArgumentsDisplayFunctions.FirstOutQueue, argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);
        }

        [Fact]
        public void VerifySBQueue2SBTopicInvocation()
        {
            FunctionInvocationPage page = GetInvocationPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBTopicMethodInfo);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            Assert.Equal(string.Format("New service bus message detected on '{0}.", ServiceBusArgumentsDisplayFunctions.FirstOutQueue), page.TriggerReason);

            FunctionArgumentsTableRow argumentRow = rows[0];
            Assert.Equal("message", argumentRow.Name);
            Assert.Equal("E2E-SBQueue2SBQueue", argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);

            argumentRow = rows[1];
            Assert.Equal("output", argumentRow.Name);
            Assert.Equal(ServiceBusArgumentsDisplayFunctions.TopicName, argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);
        }

        [Fact]
        public void VerifySBTopicListenerInvocation()
        {
            FunctionInvocationPage page = GetInvocationPage(ServiceBusArgumentsDisplayFunctions.SBTopicListenerMethodInfo);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            Assert.Equal(string.Format("New service bus message detected on '{0}/Subscriptions/{1}.", ServiceBusArgumentsDisplayFunctions.TopicName, ServiceBusArgumentsDisplayFunctions.SubscriptionName), page.TriggerReason);

            FunctionArgumentsTableRow argumentRow = rows[0];
            Assert.Equal("message", argumentRow.Name);
            Assert.Equal("E2E-SBQueue2SBQueue-SBQueue2SBTopic", argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);

            argumentRow = rows[1];
            Assert.Equal("done", argumentRow.Name);
            Assert.Equal(DoneNotificationFunction.DoneQueueName, argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);
        }

        private FunctionInvocationPage GetInvocationPage(MethodInfo method)
        {
            InvocationDetails[] invocations = _storageAccount.MethodInfoToInvocations(method).ToArray();
            InvocationDetails invocation = invocations.Single();

            return Dashboard.GoToFunctionInvocationPage(invocation.Id);
        }
    }
}
