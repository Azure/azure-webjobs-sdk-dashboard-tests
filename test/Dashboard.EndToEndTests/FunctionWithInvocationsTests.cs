// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.TestCommon;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class FunctionWithInvocationsTestsFixture : DashboardTestFixture
    {
        public FunctionWithInvocationsTestsFixture()
            : base(cleanStorageAccount: true)
        {
            JobHostConfiguration hostConfiguration = new JobHostConfiguration(StorageAccount.ConnectionString)
            {
                TypeLocator = new SimpleTypeLocator(typeof(SingleFunction))
            };

            JobHost host = new JobHost(hostConfiguration);
            host.Call(typeof(SingleFunction).GetMethod("Function"), new { fail = false });
            host.Call(typeof(SingleFunction).GetMethod("Function"), new { fail = false });
            try
            {
                host.Call(typeof(SingleFunction).GetMethod("Function"), new { fail = true });
            }
            catch (InvalidOperationException)
            {
                // The exception is thrown by the function
            }
        }

        public string FunctionId { get; set; }
    }

    public class FunctionWithInvocationsTests : DashboardTestClass<FunctionWithInvocationsTestsFixture>
    {
        private string _functionDefinitionId;

        public override void SetFixture(FunctionWithInvocationsTestsFixture data)
        {
            base.SetFixture(data);

            if (data.FunctionId == null)
            {
                Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

                MethodInfo functionInfo = typeof(SingleFunction).GetMethod("Function");
                data.FunctionId = data.StorageAccount.MethodInfoToFunctionDefinitionId(functionInfo);
            }

            _functionDefinitionId = data.FunctionId;
        }

        [Fact]
        public void FunctionsPage_Functions()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            FunctionsSection section = page.FunctionsSection;
            section.Table.WaitForDataToLoad();

            IEnumerable<TableRow> functionsRows = section.Table.BodyRows;
            Assert.Equal(1, functionsRows.Count());

            FunctionsTableRow functionRow = functionsRows.Single() as FunctionsTableRow;
            Assert.NotNull(functionRow);

            // TODO: assert link
            Assert.Equal("SingleFunction.Function", functionRow.FunctionName);
            Assert.Equal(2, functionRow.SuccessCount);
            Assert.Equal(1, functionRow.FailCount);
        }

        [Fact]
        public void FunctionPage_Invocations()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            InvocationsSection section = page.InvocationsSection;
            ValidateInvocationsTableRows(() => section.Table);
        }

        [Fact]
        public void FunctionDefinitionPage_Invocations()
        {
            FunctionDefinitionPage page = Dashboard.GoToFunctionDefinitionPage(_functionDefinitionId);

            DefinitionSection section = page.DefinitionSection;
            ValidateInvocationsTableRows(() => section.Table);
        }

        private void ValidateInvocationsTableRows(Func<InvocationsTable> tableResolver)
        {
            tableResolver().WaitForDataToLoad();

            IEnumerable<TableRow> invocationsRows = tableResolver().BodyRows;
            Assert.Equal(3, invocationsRows.Count());

            InvocationsTableRow row = invocationsRows.ElementAt(0) as InvocationsTableRow;
            Assert.Equal("SingleFunction.Function (True)", row.InvocationDisplayText);
            Assert.Equal(JobStatus.Failed, row.Status);
            Assert.False(string.IsNullOrWhiteSpace(row.CompletionTime));
            Assert.False(string.IsNullOrWhiteSpace(row.RunningTime));

            row = invocationsRows.ElementAt(1) as InvocationsTableRow;
            Assert.Equal("SingleFunction.Function (False)", row.InvocationDisplayText);
            Assert.Equal(JobStatus.Success, row.Status);
            Assert.False(string.IsNullOrWhiteSpace(row.CompletionTime));
            Assert.False(string.IsNullOrWhiteSpace(row.RunningTime));

            row = invocationsRows.ElementAt(2) as InvocationsTableRow;
            Assert.Equal("SingleFunction.Function (False)", row.InvocationDisplayText);
            Assert.Equal(JobStatus.Success, row.Status);
            Assert.False(string.IsNullOrWhiteSpace(row.CompletionTime));
            Assert.False(string.IsNullOrWhiteSpace(row.RunningTime));
        }
    }
}
