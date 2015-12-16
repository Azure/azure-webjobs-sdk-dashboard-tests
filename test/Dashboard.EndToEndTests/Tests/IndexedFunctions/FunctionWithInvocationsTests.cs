// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions.Angular;
using Dashboard.EndToEndTests.Infrastructure;
using Dashboard.EndToEndTests.Infrastructure.DashboardData;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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
                TypeLocator = new ExplicitTypeLocator(typeof(SingleFunction))
            };

            JobHost host = new JobHost(hostConfiguration);
            host.Call(SingleFunction.FunctionMethodInfo, new { fail = false, logOnSuccess = true });
            host.Call(SingleFunction.FunctionMethodInfo, new { fail = false, logOnSuccess = false });
            try
            {
                host.Call(SingleFunction.FunctionMethodInfo, new { fail = true, logOnSuccess = false });
            }
            catch (FunctionInvocationException)
            {
                // The exception is thrown by the function
            }
        }

        public string FunctionId { get; set; }

        public IEnumerable<InvocationDetails> Invocations { get; set; }
    }

    public class FunctionWithInvocationsTests : DashboardTestClass<FunctionWithInvocationsTestsFixture>
    {
        private WebJobsStorageAccount _storageAccount;
        private string _functionDefinitionId;
        private IEnumerable<InvocationDetails> _invocations;

        public FunctionWithInvocationsTests(FunctionWithInvocationsTestsFixture fixture) : base(fixture)
        {
            if (fixture.FunctionId == null)
            {
                Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

                fixture.FunctionId = fixture.StorageAccount.MethodInfoToFunctionDefinitionId(SingleFunction.FunctionMethodInfo);
                fixture.Invocations = fixture.StorageAccount.MethodInfoToInvocations(SingleFunction.FunctionMethodInfo);
            }

            _functionDefinitionId = fixture.FunctionId;
            _storageAccount = fixture.StorageAccount;
            _invocations = fixture.Invocations;
        }

        private InvocationDetails FailedInvocation
        {
            get
            {
                return _storageAccount
                    .MethodInfoToInvocations(SingleFunction.FunctionMethodInfo)
                    .Single(invocation => !invocation.Succeeded);
            }
        }

        private InvocationDetails SuccessfulInvocationWithLog
        {
            get
            {
                return _storageAccount
                    .MethodInfoToInvocations(SingleFunction.FunctionMethodInfo)
                    .Single(invocation => invocation.Succeeded && invocation.Arguments["logOnSuccess"].Value == "True");
            }
        }

        private InvocationDetails SuccessfulInvocationWithoutLog
        {
            get
            {
                return _storageAccount
                    .MethodInfoToInvocations(SingleFunction.FunctionMethodInfo)
                    .Single(invocation => invocation.Succeeded && invocation.Arguments["logOnSuccess"].Value == "False");
            }
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

        [Fact]
        public void FunctionInvocationPage_DetailsSection_Header()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(FailedInvocation.Id);

            InvocationDetailsSection section = page.DetailsSection;
            Assert.True(section.IsUserAccesible);

            Assert.Equal("Invocation Details SingleFunction.Function (True, False, )", section.Title.Text);
        }

        [Fact]
        public void FunctionInvocationPage_ReplayButton()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(FailedInvocation.Id);

            InvocationDetailsSection section = page.DetailsSection;
            Link replayButton = section.ReplayFunctionLink;
            Assert.True(replayButton.IsUserAccesible);

            Assert.Equal("Replay Function", replayButton.Text);
            // TODO: Assert the link
        }

        [Fact]
        public void FunctionInvocationPage_FailedFunction_FailureDetails()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(FailedInvocation.Id);

            InvocationDetailsSection section = page.DetailsSection;
            InvocationStatusNotification statusNotification = section.StatusNotification;
            Assert.True(statusNotification.IsUserAccesible);
            Assert.Equal(JobStatus.Failed, statusNotification.Status);

            string exceptionMessage = section.ExceptionMessage;
            Assert.NotNull(exceptionMessage);
            Assert.True(exceptionMessage.StartsWith("Microsoft.Azure.WebJobs.Host.FunctionInvocationException: Microsoft.Azure.WebJobs.Host.FunctionInvocationException: Exception while executing function: SingleFunction.Function ---> System.InvalidOperationException: Operation is not valid due to the current state of the object."));
            Assert.Equal("This function was programmatically called via the host APIs.", section.InvokeReason);
        }

        [Fact]
        public void FunctionInvocationPage_FailedFunction_Arguments()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(FailedInvocation.Id);

            InvocationDetailsSection section = page.DetailsSection;
            FunctionArgumentsTable table = section.ArgumentsTable;
            Assert.True(table.IsUserAccesible);

            Assert.Equal(3, table.BodyRows.Count());

            FunctionArgumentsTableRow tableRow = table.BodyRows.ElementAt(0) as FunctionArgumentsTableRow;
            Assert.Equal("fail", tableRow.Name);
            Assert.Equal("True", tableRow.Value.TextValue);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(1) as FunctionArgumentsTableRow;
            Assert.Equal("logOnSuccess", tableRow.Name);
            Assert.Equal("False", tableRow.Value.TextValue);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(2) as FunctionArgumentsTableRow;
            Assert.Equal("log", tableRow.Name);
            Assert.Equal("", tableRow.Value.TextValue);
            Assert.Equal("", tableRow.Notes);
        }

        [Fact]
        public void FunctionInvocationPage_SuccessfulFunction_Details()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(SuccessfulInvocationWithLog.Id);

            InvocationDetailsSection section = page.DetailsSection;
            InvocationStatusNotification statusNotification = section.StatusNotification;
            Assert.True(statusNotification.IsUserAccesible);
            Assert.Equal(JobStatus.Success, statusNotification.Status);
        }

        [Fact]
        public void FunctionInvocationPage_SuccessfulFunction_Arguments()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(SuccessfulInvocationWithLog.Id);

            InvocationDetailsSection section = page.DetailsSection;
            FunctionArgumentsTable table = section.ArgumentsTable;
            Assert.True(table.IsUserAccesible);

            FunctionArgumentsTableRow tableRow = table.BodyRows.ElementAt(0) as FunctionArgumentsTableRow;
            Assert.Equal("fail", tableRow.Name);
            Assert.Equal("False", tableRow.Value.TextValue);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(1) as FunctionArgumentsTableRow;
            Assert.Equal("logOnSuccess", tableRow.Name);
            Assert.Equal("True", tableRow.Value.TextValue);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(2) as FunctionArgumentsTableRow;
            Assert.Equal("log", tableRow.Name);
            Assert.Equal("", tableRow.Value.TextValue);
            Assert.Equal("", tableRow.Notes);
        }

        [Fact]
        public void FunctionInvocationPage_ToggleOutputButton()
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(FailedInvocation.Id);
            JobOutputSection section = page.DetailsSection.OutputSection;

            NgButton toggleOutput = section.ToggleOutputButton;
            Assert.True(toggleOutput.IsUserAccesible);

            Assert.Equal("Toggle Output", toggleOutput.Caption);
            Assert.Equal("toggleConsole()", toggleOutput.ClickAction);

            Link downloadLink = section.DownloadLogLink;
            TextArea output = section.Output;

            Assert.False(output.IsUserAccesible);
            Assert.False(downloadLink.IsUserAccesible);

            toggleOutput.Click();
            Assert.True(output.IsUserAccesible);
            Assert.True(downloadLink.IsUserAccesible);
            Assert.Equal("download", downloadLink.Text);

            toggleOutput.Click();
            Assert.False(output.IsUserAccesible);
            Assert.False(downloadLink.IsUserAccesible);
        }

        [Fact]
        public void FunctionInvocationPage_EmptyOutput()
        {
            ValidateInvocationOutput(
                SuccessfulInvocationWithoutLog.Id, 
                output => Assert.True(string.IsNullOrWhiteSpace(output)));
        }

        [Fact]
        public void FunctionInvocationPage_SuccessfulFunction_NonEmptyOutput()
        {
            ValidateInvocationOutput(
                SuccessfulInvocationWithLog.Id, 
                output => Assert.Equal("logOnSuccess is enabled", output));
        }

        [Fact]
        public void FunctionInvocationPage_FailedFunction_NonEmptyOutput()
        {
            ValidateInvocationOutput(FailedInvocation.Id, 
                output => Assert.True(output.StartsWith("This function will always fail in this case")));
        }

        private void ValidateInvocationOutput(string invocationId, Action<string> outputTextValidator)
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(invocationId);
            JobOutputSection section = page.DetailsSection.OutputSection;

            NgButton toggleOutput = section.ToggleOutputButton;
            section.ToggleOutputButton.Click();

            TextArea output = section.Output;
            output.WaitForDataToLoad();

            outputTextValidator(output.Text.Trim());

            Link downloadLink = section.DownloadLogLink;
            Assert.False(string.IsNullOrWhiteSpace(downloadLink.Href));

            string fileOutput = Dashboard.Api.DownloadTextFrom(downloadLink.Href);
            outputTextValidator(fileOutput.Trim());
        }

        private void ValidateInvocationsTableRows(Func<InvocationsTable> tableResolver)
        {
            tableResolver().WaitForDataToLoad();

            IEnumerable<TableRow> invocationsRows = tableResolver().BodyRows;
            Assert.Equal(3, invocationsRows.Count());

            InvocationsTableRow row = invocationsRows.ElementAt(0) as InvocationsTableRow;
            Assert.Equal("SingleFunction.Function (True, False, )", row.InvocationDisplayText);
            Assert.Equal(JobStatus.Failed, row.Status);
            Assert.False(string.IsNullOrWhiteSpace(row.CompletionTime));
            Assert.False(string.IsNullOrWhiteSpace(row.RunningTime));

            row = invocationsRows.ElementAt(1) as InvocationsTableRow;
            Assert.Equal("SingleFunction.Function (False, False, )", row.InvocationDisplayText);
            Assert.Equal(JobStatus.Success, row.Status);
            Assert.False(string.IsNullOrWhiteSpace(row.CompletionTime));
            Assert.False(string.IsNullOrWhiteSpace(row.RunningTime));

            row = invocationsRows.ElementAt(2) as InvocationsTableRow;
            Assert.Equal("SingleFunction.Function (False, True, )", row.InvocationDisplayText);
            Assert.Equal(JobStatus.Success, row.Status);
            Assert.False(string.IsNullOrWhiteSpace(row.CompletionTime));
            Assert.False(string.IsNullOrWhiteSpace(row.RunningTime));
        }
    }
}
