// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
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
        public void SBQueue2SBQueue_VerifyInvocation()
        {
            FunctionInvocationPage page = GetInvocationPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBQueueMethodInfo);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            string expectedMessage = string.Format("New ServiceBus message detected on '{0}'.", ServiceBusArgumentsDisplayFunctions.StartQueueName);
            Assert.Equal(expectedMessage, page.TriggerReason);

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
        public void SBQueue2SBQueue_VerifyRunFunctionForm()
        {
            RunFunctionPage page = GetRunPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBQueueMethodInfo);

            Collection<FormElement> parameters = page.GetParameters();
            Assert.Equal(2, parameters.Count);

            FormElement element = parameters[0];
            Assert.Equal("start Enter the queue message body", element.Label);
            Assert.Equal(string.Empty, element.InputValue);

            element = parameters[1];
            Assert.Equal("message Enter the output entity name", element.Label);
            Assert.Equal("servicebus-output1", element.InputValue);
        }

        [Fact]
        public void SBQueue2SBQueue_VerifyReplayFunctionForm()
        {
            ReplayFunctionPage page = GetReplayPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBQueueMethodInfo);

            Collection<FormElement> parameters = page.GetParameters();
            Assert.Equal(2, parameters.Count);

            FormElement element = parameters[0];
            Assert.Equal("start Enter the queue message body", element.Label);
            Assert.Equal("E2E", element.InputValue);

            element = parameters[1];
            Assert.Equal("message Enter the output entity name", element.Label);
            Assert.Equal("servicebus-output1", element.InputValue);
        }

        [Fact]
        public void SBQueue2SBTopic_VerifyInvocation()
        {
            FunctionInvocationPage page = GetInvocationPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBTopicMethodInfo);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            string expectedMessage = string.Format("New ServiceBus message detected on '{0}'.", ServiceBusArgumentsDisplayFunctions.FirstOutQueue);
            Assert.Equal(expectedMessage, page.TriggerReason);

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
        public void SBQueue2SBTopic_VerifyRunFunctionForm()
        {
            RunFunctionPage page = GetRunPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBTopicMethodInfo);

            Collection<FormElement> parameters = page.GetParameters();
            Assert.Equal(2, parameters.Count);

            FormElement element = parameters[0];
            Assert.Equal("message Enter the queue message body", element.Label);
            Assert.Equal(string.Empty, element.InputValue);

            element = parameters[1];
            Assert.Equal("output Enter the output entity name", element.Label);
            Assert.Equal("servicebus-topic", element.InputValue);
        }

        [Fact]
        public void SBQueue2SBTopic_VerifyReplayFunctionForm()
        {
            ReplayFunctionPage page = GetReplayPage(ServiceBusArgumentsDisplayFunctions.SBQueue2SBTopicMethodInfo);

            Collection<FormElement> parameters = page.GetParameters();
            Assert.Equal(2, parameters.Count);

            FormElement element = parameters[0];
            Assert.Equal("message Enter the queue message body", element.Label);
            Assert.Equal("E2E-SBQueue2SBQueue", element.InputValue);

            element = parameters[1];
            Assert.Equal("output Enter the output entity name", element.Label);
            Assert.Equal("servicebus-topic", element.InputValue);
        }

        [Fact]
        public void SBTopicListener_VerifyInvocation()
        {
            FunctionInvocationPage page = GetInvocationPage(ServiceBusArgumentsDisplayFunctions.SBTopicListenerMethodInfo);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            string expectedMessage = string.Format("New ServiceBus message detected on '{0}/Subscriptions/{1}'.", ServiceBusArgumentsDisplayFunctions.TopicName, ServiceBusArgumentsDisplayFunctions.SubscriptionName);
            Assert.Equal(expectedMessage, page.TriggerReason);

            FunctionArgumentsTableRow argumentRow = rows[0];
            Assert.Equal("message", argumentRow.Name);
            Assert.Equal("E2E-SBQueue2SBQueue-SBQueue2SBTopic", argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);

            argumentRow = rows[1];
            Assert.Equal("done", argumentRow.Name);
            Assert.Equal(DoneNotificationFunction.DoneQueueName, argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);
        }

        [Fact]
        public void SBTopicListener_VerifyRunFunctionForm()
        {
            RunFunctionPage page = GetRunPage(ServiceBusArgumentsDisplayFunctions.SBTopicListenerMethodInfo);

            Collection<FormElement> parameters = page.GetParameters();
            Assert.Equal(2, parameters.Count);

            FormElement element = parameters[0];
            Assert.Equal("message Enter the queue message body", element.Label);
            Assert.Equal(string.Empty, element.InputValue);

            element = parameters[1];
            Assert.Equal("done Enter the output queue name", element.Label);
            Assert.Equal("queue-done", element.InputValue);
        }

        [Fact]
        public void SBTopicListener_VerifyReplayFunctionForm()
        {
            ReplayFunctionPage page = GetReplayPage(ServiceBusArgumentsDisplayFunctions.SBTopicListenerMethodInfo);

            Collection<FormElement> parameters = page.GetParameters();
            Assert.Equal(2, parameters.Count);

            FormElement element = parameters[0];
            Assert.Equal("message Enter the queue message body", element.Label);
            Assert.Equal("E2E-SBQueue2SBQueue-SBQueue2SBTopic", element.InputValue);

            element = parameters[1];
            Assert.Equal("done Enter the output queue name", element.Label);
            Assert.Equal("queue-done", element.InputValue);
        }

        private FunctionInvocationPage GetInvocationPage(MethodInfo method)
        {
            InvocationDetails invocation = _storageAccount.MethodInfoToInvocations(method).Single();

            return Dashboard.GoToFunctionInvocationPage(invocation.Id);
        }

        private RunFunctionPage GetRunPage(MethodInfo method)
        {
            string functionId = _storageAccount.MethodInfoToFunctionDefinitionId(method);

            return Dashboard.GoToRunPage(functionId);
        }

        private ReplayFunctionPage GetReplayPage(MethodInfo method)
        {
            InvocationDetails invocation = _storageAccount.MethodInfoToInvocations(method).Single();

            return Dashboard.GoToReplayPage(invocation.Id);
        }
    }
}
