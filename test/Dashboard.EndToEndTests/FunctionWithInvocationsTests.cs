﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
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
using Microsoft.Azure.WebJobs.Host.TestCommon;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class FunctionWithInvocationsTestsFixture : DashboardTestFixture
    {
        public static readonly MethodInfo FunctionInfo = typeof(SingleFunction).GetMethod("Function");

        public FunctionWithInvocationsTestsFixture()
            : base(cleanStorageAccount: true)
        {
            JobHostConfiguration hostConfiguration = new JobHostConfiguration(StorageAccount.ConnectionString)
            {
                TypeLocator = new SimpleTypeLocator(typeof(SingleFunction))
            };

            JobHost host = new JobHost(hostConfiguration);
            host.Call(FunctionInfo, new { fail = false, logOnSuccess = true });
            host.Call(FunctionInfo, new { fail = false, logOnSuccess = false });
            try
            {
                host.Call(FunctionInfo, new { fail = true, logOnSuccess = false });
            }
            catch (InvalidOperationException)
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
       
        public override void SetFixture(FunctionWithInvocationsTestsFixture data)
        {
            base.SetFixture(data);

            if (data.FunctionId == null)
            {
                Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

                data.FunctionId = data.StorageAccount.MethodInfoToFunctionDefinitionId(FunctionWithInvocationsTestsFixture.FunctionInfo);
                data.Invocations = data.StorageAccount.MethodInfoToInvocations(FunctionWithInvocationsTestsFixture.FunctionInfo);
            }

            _functionDefinitionId = data.FunctionId;
            _storageAccount = data.StorageAccount;
            _invocations = data.Invocations;
        }

        private InvocationDetails FailedInvocation
        {
            get
            {
                return _storageAccount
                    .MethodInfoToInvocations(FunctionWithInvocationsTestsFixture.FunctionInfo)
                    .Single(invocation => !invocation.Succeeded);
            }
        }

        private InvocationDetails SuccessfulInvocationWithLog
        {
            get
            {
                return _storageAccount
                    .MethodInfoToInvocations(FunctionWithInvocationsTestsFixture.FunctionInfo)
                    .Single(invocation => invocation.Succeeded && invocation.Arguments["logOnSuccess"].Value == "True");
            }
        }

        private InvocationDetails SuccessfulInvocationWithoutLog
        {
            get
            {
                return _storageAccount
                    .MethodInfoToInvocations(FunctionWithInvocationsTestsFixture.FunctionInfo)
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
            Assert.True(exceptionMessage.StartsWith("System.InvalidOperationException: System.InvalidOperationException: Operation is not valid due to the current state of the object"));
            Assert.Equal("This was function was programmatically called via the host APIs.", section.InvokeReason);
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
            Assert.Equal("True", tableRow.Value);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(1) as FunctionArgumentsTableRow;
            Assert.Equal("logOnSuccess", tableRow.Name);
            Assert.Equal("False", tableRow.Value);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(2) as FunctionArgumentsTableRow;
            Assert.Equal("log", tableRow.Name);
            Assert.Equal("", tableRow.Value);
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
            Assert.Equal("False", tableRow.Value);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(1) as FunctionArgumentsTableRow;
            Assert.Equal("logOnSuccess", tableRow.Name);
            Assert.Equal("True", tableRow.Value);
            Assert.Equal("", tableRow.Notes);

            tableRow = table.BodyRows.ElementAt(2) as FunctionArgumentsTableRow;
            Assert.Equal("log", tableRow.Name);
            Assert.Equal("", tableRow.Value);
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
            ValidateInvocationOutput(SuccessfulInvocationWithoutLog.Id, string.Empty);
        }

        [Fact]
        public void FunctionInvocationPage_SuccessfulFunction_NonEmptyOutput()
        {
            ValidateInvocationOutput(SuccessfulInvocationWithLog.Id, "logOnSuccess is enabled");
        }

        [Fact]
        public void FunctionInvocationPage_FailedFunction_NonEmptyOutput()
        {
            ValidateInvocationOutput(FailedInvocation.Id, "This function will always fail in this case");
        }

        private void ValidateInvocationOutput(string invocationId, string expectedOutput)
        {
            FunctionInvocationPage page = Dashboard.GoToFunctionInvocationPage(invocationId);
            JobOutputSection section = page.DetailsSection.OutputSection;

            NgButton toggleOutput = section.ToggleOutputButton;
            section.ToggleOutputButton.Click();

            TextArea output = section.Output;
            output.WaitForDataToLoad();

            string outputText = output.Text;
            if (outputText != null && string.IsNullOrWhiteSpace(outputText))
            {
                outputText = string.Empty;
            }
            Assert.Equal(expectedOutput, outputText);

            Link downloadLink = section.DownloadLogLink;
            Assert.False(string.IsNullOrWhiteSpace(downloadLink.Href));

            string fileOutput = Dashboard.Api.DownloadTextFrom(downloadLink.Href);
            fileOutput = fileOutput.TrimEnd('\r', '\n');
            Assert.Equal(expectedOutput, fileOutput);
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