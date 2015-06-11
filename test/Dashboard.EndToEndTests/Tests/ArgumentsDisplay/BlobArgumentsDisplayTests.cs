// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Dashboard.EndToEndTests.Infrastructure;
using Dashboard.EndToEndTests.Infrastructure.DashboardData;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class BlobArgumentsDisplayTests : DashboardTestClass<BlobArgumentsDisplayFixture>
    {
        private const string DataSizeKey = "datasize";
        private const string DataUnitKey = "dataunit";
        private const string PercentKey = "percent";
        private const string TimeKey = "time";
        private const string TimeUnitKey = "timeunit";

        private const string DataMatcherPattern = "(?<" + DataSizeKey + ">\\d+) (?<" + DataUnitKey + ">\\w+)";
        private const string PercentMatcherPattern = "[(](?<" + PercentKey + ">\\d+[.]\\d+)% of total[)]";
        private const string TimeMatcherPattern = "About (?<" + TimeKey + ">\\d+) (?<" + TimeUnitKey + ">\\w+) spent on I/O.";

        private static readonly string _blobReadNotesPattern = string.Format(
            "(?n)^Read {0} {1}[.]( {2})?$",
            DataMatcherPattern,
            PercentMatcherPattern,
            TimeMatcherPattern);

        private static readonly string _blobWroteNotesPattern = string.Format(
            "(?n)^Wrote {0}[.]$",
            DataMatcherPattern);

        private static readonly Regex _blobReadNotesRegex = new Regex(_blobReadNotesPattern, RegexOptions.Compiled);
        private static readonly Regex _blobWroteNotesRegex = new Regex(_blobWroteNotesPattern, RegexOptions.Compiled);

        private WebJobsStorageAccount _storageAccount;

        public BlobArgumentsDisplayTests(BlobArgumentsDisplayFixture fixture) : base(fixture)
        {
            Waiters.WaitForAction(() => Dashboard.Api.IndexingQueueLength(limit: 1) == 0);

            _storageAccount = fixture.StorageAccount;
        }

        [Fact]
        public void Blob_CloudBlockBlobArguments()
        {
            FunctionWithBlobArgumentsTest(
                BlobArgumentsDisplayFunctions.CloudBlockBlobMethodInfo,
                rows =>
                {
                    Assert.Equal(string.Empty, rows[0].Notes);
                    Assert.Equal(string.Empty, rows[1].Notes);
                    Assert.Equal(string.Empty, rows[2].Notes);
                });
        }

        [Fact]
        public void Blob_POCOArguments()
        {
            FunctionWithBlobArgumentsTest(
                BlobArgumentsDisplayFunctions.POCOMethodInfo,
                rows =>
                {
                    AssertReadDataDisplay(rows[0].Notes, expectedDataSize: "35");
                    AssertReadDataDisplay(rows[1].Notes, expectedDataSize: "35");
                    AssertWroteDataDisplay(rows[2].Notes, expectedDataSize: "38");
                });
        }

        [Fact]
        public void Blob_StreamArguments()
        {
            FunctionWithBlobArgumentsTest(
                BlobArgumentsDisplayFunctions.StreamMethodInfo,
                rows =>
                {
                    AssertReadDataDisplay(rows[0].Notes, expectedDataSize: "16");
                    AssertReadDataDisplay(rows[1].Notes, expectedDataSize: "13");
                    AssertWroteDataDisplay(rows[2].Notes, expectedDataSize: "29");
                });
        }

        [Fact]
        public void Blob_StringArguments()
        {
            FunctionWithBlobArgumentsTest(
                BlobArgumentsDisplayFunctions.StringMethodInfo,
                rows =>
                {
                    AssertReadDataDisplay(rows[0].Notes, expectedDataSize: "16");
                    AssertReadDataDisplay(rows[1].Notes, expectedDataSize: "13");
                    AssertWroteDataDisplay(rows[2].Notes, expectedDataSize: "32");
                });
        }

        [Fact]
        public void Blob_TextReaderWriterArguments()
        {
            FunctionWithBlobArgumentsTest(
                BlobArgumentsDisplayFunctions.TextReaderWriterMethodInfo,
                rows =>
                {
                    AssertReadDataDisplay(rows[0].Notes, expectedDataSize: "16");
                    AssertReadDataDisplay(rows[1].Notes, expectedDataSize: "13");
                    AssertWroteDataDisplay(rows[2].Notes, expectedDataSize: "32");
                });
        }

        private void AssertReadDataDisplay(
            string text,
            string expectedDataSize,
            string expectedUnit = "bytes",
            string expectedPercent = "100.00",
            bool hasTime = true,
            int minimumExpectedIOTime = -1)
        {
            BlobDataNotes notes;
            Assert.True(BlobDataNotes.TryParseAsReadData(text, out notes));

            Assert.Equal(expectedDataSize, notes.DataSize);
            Assert.Equal(expectedUnit, notes.DataUnit);

            Assert.Equal(expectedPercent, notes.Percent);

            if (hasTime)
            {
                Assert.False(string.IsNullOrWhiteSpace(notes.Time));
                int numericTime = int.Parse(notes.Time);
                Assert.True(numericTime > minimumExpectedIOTime);
                Assert.False(string.IsNullOrWhiteSpace(notes.TimeUnit));
            }
            else
            {
                Assert.Equal(string.Empty, notes.Time);
                Assert.Equal(string.Empty, notes.TimeUnit);
            }
        }

        private void AssertWroteDataDisplay(
            string text,
            string expectedDataSize,
            string expectedUnit = "bytes")
        {
            BlobDataNotes notes;
            Assert.True(BlobDataNotes.TryParseAsWroteData(text, out notes));

            Assert.Equal(expectedDataSize, notes.DataSize);
            Assert.Equal(expectedUnit, notes.DataUnit);

            Assert.Equal(string.Empty, notes.Percent);
            Assert.Equal(string.Empty, notes.Time);
            Assert.Equal(string.Empty, notes.TimeUnit);
        }

        private void FunctionWithBlobArgumentsTest(MethodInfo function, Action<FunctionArgumentsTableRow[]> customDisplayValidator)
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

            string blobPartialName = BlobArgumentsDisplayFunctions.ContainerName + "/" + function.Name.ToLowerInvariant();

            string fullBlobName = blobPartialName + "-trigger";
            FunctionArgumentsTableRow argumentRow = rows[0];
            Link blobLink = argumentRow.Value.BlobLink;
            Assert.Equal("trigger", argumentRow.Name);
            Assert.True(blobLink.IsUserAccesible);
            Assert.Equal(fullBlobName, blobLink.Text);
            Assert.Equal(
                Dashboard.Api.ConstructDownloadBlobUrl(fullBlobName),
                blobLink.Href,
                StringComparer.OrdinalIgnoreCase);

            fullBlobName = blobPartialName + "-in";
            argumentRow = rows.ElementAt(1);
            blobLink = argumentRow.Value.BlobLink;
            Assert.Equal("input", argumentRow.Name);
            Assert.True(blobLink.IsUserAccesible);
            Assert.Equal(fullBlobName, blobLink.Text);
            Assert.Equal(
                Dashboard.Api.ConstructDownloadBlobUrl(fullBlobName),
                blobLink.Href,
                StringComparer.OrdinalIgnoreCase);

            fullBlobName = blobPartialName + "-out";
            argumentRow = rows.ElementAt(2);
            blobLink = argumentRow.Value.BlobLink;
            Assert.Equal("output", argumentRow.Name);
            Assert.True(blobLink.IsUserAccesible);
            Assert.Equal(fullBlobName, blobLink.Text);
            Assert.Equal(
                Dashboard.Api.ConstructDownloadBlobUrl(fullBlobName),
                blobLink.Href,
                StringComparer.OrdinalIgnoreCase);

            customDisplayValidator(rows);
        }

        private class BlobDataNotes
        {
            private BlobDataNotes()
            {
            }

            public string DataSize { get; private set; }
            public string DataUnit { get; private set; }
            public string Percent { get; private set; }
            public string Time { get; private set; }
            public string TimeUnit { get; private set; }

            public static bool TryParseAsReadData(string input, out BlobDataNotes result)
            {
                result = TryParse(_blobReadNotesRegex, input);
                return result != null;
            }

            public static bool TryParseAsWroteData(string input, out BlobDataNotes result)
            {
                result = TryParse(_blobWroteNotesRegex, input);
                return result != null;
            }

            private static BlobDataNotes TryParse(Regex regex, string input)
            {
                Match match = regex.Match(input);
                if (!match.Success)
                {
                    return null;
                }

                return new BlobDataNotes()
                {
                    DataSize = match.Groups[DataSizeKey].Value,
                    DataUnit = match.Groups[DataUnitKey].Value,
                    Percent = match.Groups[PercentKey].Value,
                    Time = match.Groups[TimeKey].Value,
                    TimeUnit = match.Groups[TimeUnitKey].Value,
                };
            }
        }
    }
}
