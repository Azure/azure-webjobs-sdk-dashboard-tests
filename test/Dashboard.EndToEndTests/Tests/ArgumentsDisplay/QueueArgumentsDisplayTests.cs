// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.Infrastructure;
using Dashboard.EndToEndTests.Infrastructure.DashboardData;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class QueueArgumentsDisplayTests : DashboardTestClass<QueueArgumentsDisplayTestsFixture>
    {
        private WebJobsStorageAccount _storageAccount;

        public QueueArgumentsDisplayTests(QueueArgumentsDisplayTestsFixture fixture) : base (fixture)
        {
            Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

            _storageAccount = fixture.StorageAccount;
        }

        [Fact]
        public void Queue_ByteArguments()
        {
            FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.ByteMethodInfo);
        }

        [Fact]
        public void Queue_CloudQueueMessageArguments()
        {
            FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.CloudQueueMessageMethodInfo);
        }

        [Fact]
        public void Queue_ICollectorArguments()
        {
            FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.ICollectorMethodInfo);
        }

        [Fact]
        public void Queue_NullOutputArguments()
        {
            FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.NullOutputMethodInfo);
        }

        [Fact]
        public void Queue_Singleton()
        {
            FunctionInvocationPage page = FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.SingletonMethodInfo);

            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;
            FunctionArgumentsTableRow[] rows = arguments.BodyRows.Cast<FunctionArgumentsTableRow>().ToArray();

            // verify the implicit singleton parameter
            FunctionArgumentsTableRow argumentRow = rows[3];
            Assert.Equal("(singleton)", argumentRow.Name);
            Assert.Equal("ScopeId: TestScope", argumentRow.Value.TextValue);

            Regex regex = new Regex(@"Lock acquired. Wait time: About (\d*) milliseconds. Lock duration: About (\d*) milliseconds.");
            Assert.True(regex.IsMatch(argumentRow.Notes));
        }

        [Fact]
        public void Queue_POCOArguments()
        {
            FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.PocoMethodInfo);
        }

        [Fact]
        public void Queue_StringArguments()
        {
            FunctionWithQueueArgumentsTest(QueueArgumentsDisplayFunctions.StringMethodInfo);
        }

        private FunctionInvocationPage FunctionWithQueueArgumentsTest(MethodInfo function)
        {
            InvocationDetails invocation = _storageAccount
                .MethodInfoToInvocations(function)
                .Single();

            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(invocation.Id);
            FunctionArgumentsTable arguments = page.DetailsSection.ArgumentsTable;

            FunctionArgumentsTableRow[] rows = arguments
                .BodyRows
                .Cast<FunctionArgumentsTableRow>()
                .ToArray();

            string queueName = string.Format("queue-{0}-in", function.Name.ToLowerInvariant());
            Assert.Equal(string.Format("New queue message detected on '{0}'.", queueName), page.TriggerReason);

            FunctionArgumentsTableRow argumentRow = rows[0];
            Assert.Equal("input", argumentRow.Name);
            Assert.Equal(POCO.JsonSample, argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);

            argumentRow = rows[1];
            Assert.Equal("output", argumentRow.Name);
            Assert.Equal(
                QueueArgumentsDisplayTestsFixture.CreateQueueName(function, input: false),
                argumentRow.Value.TextValue);
            Assert.Equal(string.Empty, argumentRow.Notes);

            return page;
        }
    }
}
