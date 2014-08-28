// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Dashboard.EndToEndTests.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.TestCommon;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class FunctionsPageTests : IUseFixture<DashboardTestFixture>
    {
        private WebJobsDashboard _dashboard;
        private WebJobsStorageAccount _storageAccount;

        public void SetFixture(DashboardTestFixture data)
        {
            _storageAccount = data.StorageAccount;

            data.StorageAccount.Empty();
            data.Server.SetStorageConnectionString(data.StorageAccount.ConnectionString);
            _dashboard = data.CreateDashboard();
        }

        [Fact]
        public void SingleFunctionIsIndexed()
        {
            _storageAccount.Empty();

            JobHostConfiguration hostConfiguration = new JobHostConfiguration(_storageAccount.ConnectionString)
            {
                TypeLocator = new SimpleTypeLocator(typeof(SingleFunction))
            };

            JobHost host = new JobHost(hostConfiguration);
            host.Start();
            host.Dispose();

            _dashboard.GoTo(typeof(FunctionsPage));
            FunctionsPage page = _dashboard.FunctionsPage;

            // Functions table
            FunctionsSection functionsSection = page.FunctionsSection;
            functionsSection.Table.WaitForDataToLoad();

            IEnumerable<TableRow> functionsRows = functionsSection.Table.BodyRows;
            Assert.Equal(1, functionsRows.Count());

            FunctionsTableRow functionRow = functionsRows.Single() as FunctionsTableRow;
            Assert.NotNull(functionRow);

            // TODO: assert link
            Assert.Equal("SingleFunction.Function", functionRow.FunctionName);
            Assert.Equal(0, functionRow.SuccessCount);
            Assert.Equal(0, functionRow.FailCount);

            // Invocations table
            InvocationsSection invocationsSection = page.InvocationsSection;
            invocationsSection.Table.WaitForDataToLoad();

            IEnumerable<TableRow> invocationsRows = invocationsSection.Table.BodyRows;
            Assert.Equal(1, invocationsRows.Count());

            TableRow singleRow = invocationsRows.Single();
            IEnumerable<TableCell> cells = singleRow.Cells;
            Assert.Equal(1, cells.Count());

            TableCell singleCell = cells.Single();
            Assert.Equal("No functions have run recently.", singleCell.RawElement.Text);
        }

        [Fact]
        public void AllStatsShowForSingleFunction()
        {
            _storageAccount.Empty();

            JobHostConfiguration hostConfiguration = new JobHostConfiguration(_storageAccount.ConnectionString)
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
            catch(InvalidOperationException)
            {
                // The exception is thrown by the function
            }

            _dashboard.GoTo(typeof(FunctionsPage));
            FunctionsPage page = _dashboard.FunctionsPage;

            // Functions table
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

            // Invocations table
            InvocationsSection invocationsSection = page.InvocationsSection;
            invocationsSection.Table.WaitForDataToLoad();

            IEnumerable<TableRow> invocationsRows = invocationsSection.Table.BodyRows;
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

        private class SingleFunction
        {
            [NoAutomaticTrigger]
            public static void Function(bool fail)
            {
                if (fail)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
