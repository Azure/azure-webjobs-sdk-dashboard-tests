// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Microsoft.Azure.WebJobs;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class FunctionWithoutInvocationsTestsFixture : DashboardTestFixture
    {
        public FunctionWithoutInvocationsTestsFixture()
            : base(cleanStorageAccount: true)
        {
            JobHostConfiguration hostConfiguration = new JobHostConfiguration(StorageAccount.ConnectionString)
            {
                TypeLocator = new ExplicitTypeLocator(typeof(SingleFunction))
            };

            JobHost host = new JobHost(hostConfiguration);
            host.Start();
            host.Dispose();
        }

        public string FunctionId { get; set; }
    }

    public class FunctionWithoutInvocationsTests : DashboardTestClass<FunctionWithoutInvocationsTestsFixture>
    {
        private string _functionDefinitionId;

        public FunctionWithoutInvocationsTests(FunctionWithoutInvocationsTestsFixture fixture) : base (fixture)
        {
            if (fixture.FunctionId == null)
            {
                Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

                MethodInfo functionInfo = typeof(SingleFunction).GetMethod("Function");
                fixture.FunctionId = fixture.StorageAccount.MethodInfoToFunctionDefinitionId(functionInfo);
            }

            _functionDefinitionId = fixture.FunctionId;
        }

        [Fact]
        public void FunctionsPage_Functions()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            FunctionsSection functionsSection = page.FunctionsSection;
            functionsSection.Table.WaitForDataToLoad();

            IEnumerable<TableRow> functionsRows = functionsSection.Table.BodyRows;
            Assert.Equal(1, functionsRows.Count());

            FunctionsTableRow functionRow = functionsRows.Single() as FunctionsTableRow;
            Assert.NotNull(functionRow);

            Assert.Equal("SingleFunction.Function", functionRow.FunctionName);
            Assert.Equal(0, functionRow.SuccessCount);
            Assert.Equal(0, functionRow.FailCount);
            Assert.Equal(
                Dashboard.BuildFullUrl(FunctionDefinitionPage.ConstructRelativePath(_functionDefinitionId)),
                functionRow.FunctionLink.Href);
        }

        [Fact]
        public void FunctionsPage_Invocations()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            InvocationsSection section = page.InvocationsSection;
            ValidateEmptyInvocationsTable(() => section.Table);
        }

        [Fact]
        public void FunctionDefinitionPage_Invocations()
        {
            FunctionDefinitionPage page = Dashboard.GoToFunctionDefinitionPage(_functionDefinitionId);

            DefinitionSection section = page.DefinitionSection;
            ValidateEmptyInvocationsTable(() => section.Table);
        }

        [Fact]
        public void FunctionDefinitionPage_RunButton()
        {
            FunctionDefinitionPage page = Dashboard.GoToFunctionDefinitionPage(_functionDefinitionId);

            DefinitionSection section = page.DefinitionSection;

            Link runButton = section.RunFunctionLink;
            Assert.True(runButton.IsUserAccesible);
            Assert.Equal("Run function", runButton.Text);
            Assert.Equal(
                Dashboard.BuildFullUrl(RunFunctionPage.ConstructRelativePath(_functionDefinitionId)),
                runButton.Href);
        }

        private void ValidateEmptyInvocationsTable(Func<InvocationsTable> tableResolver)
        {
            tableResolver().WaitForDataToLoad();

            IEnumerable<TableRow> invocationsRows = tableResolver().BodyRows;
            Assert.Equal(1, invocationsRows.Count());

            TableRow singleRow = invocationsRows.Single();
            IEnumerable<TableCell> cells = singleRow.Cells;
            Assert.Equal(1, cells.Count());

            TableCell singleCell = cells.Single();
            Assert.Equal("No functions have run recently.", singleCell.RawElement.Text);
        }
    }
}
