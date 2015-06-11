// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Dashboard.EndToEndTests.Infrastructure;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class CleanStorageAccountTests : DashboardTestClass<DashboardTestFixture>
    {
        private WebJobsStorageAccount _storageAccount;

        public CleanStorageAccountTests(DashboardTestFixture fixture)
            : base(fixture)
        {
            _storageAccount = fixture.StorageAccount;
        }

        [Fact]
        public void Navbar_Buttons()
        {
            JobsPage page = Dashboard.GoToJobsPage();

            Navbar navbar = page.Navbar;

            // Verify title
            Link title = navbar.TitleLink;
            Assert.True(title.IsUserAccesible);
            Assert.Equal("Microsoft Azure WebJobs", title.Text);
            Assert.Equal(Dashboard.BuildFullUrl(JobsPage.RelativePath), title.Href);

            // Verify navbar
            IEnumerable<Link> links = navbar.NavLinks;
            Assert.Equal(3, links.Count());

            Link functionLink = links.ElementAt(0);
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("Functions", functionLink.Text);
            Assert.Equal(Dashboard.BuildFullUrl(FunctionsPage.RelativePath), functionLink.Href);

            Link searchLink = links.ElementAt(1);
            Assert.True(searchLink.IsUserAccesible);
            Assert.Equal("Search Blobs", searchLink.Text);
            Assert.Equal(Dashboard.BuildFullUrl(SearchBlobPage.RelativePath), searchLink.Href);

            Link aboutLink = links.ElementAt(2);
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("About", aboutLink.Text);
            Assert.Equal(Dashboard.BuildFullUrl(AboutPage.RelativePath), aboutLink.Href);
        }

        [Fact]
        public void FunctionsPage_NoFunctions()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            FunctionsSection section = page.FunctionsSection;
            Assert.True(section.IsUserAccesible);

            Assert.Equal("Functions", section.Title.Text);

            FunctionsTable table = section.Table;
            table.WaitForDataToLoad();

            // Find the table again because after load it gets completely replace
            table = section.Table;

            IEnumerable<TableRow> rows = table.BodyRows;
            Assert.Equal(1, rows.Count());

            TableRow singleRow = rows.Single();
            IEnumerable<TableCell> cells = singleRow.Cells;
            Assert.Equal(1, cells.Count());

            TableCell singleCell = cells.Single();
            Assert.Equal(table.HeaderRows.First().Cells.Count(), singleCell.ColSpan);
            Assert.Equal("No functions are present.", singleCell.RawElement.Text);
        }

        [Fact]
        public void FunctionsPage_NoInvocations()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            InvocationsSection section = page.InvocationsSection;
            Assert.True(section.IsUserAccesible);

            Assert.Equal("Invocation Log Recently executed functions", section.Title.Text);

            InvocationsTable table = section.Table;
            table.WaitForDataToLoad();

            table = section.Table;

            IEnumerable<TableRow> rows = table.BodyRows;
            Assert.Equal(1, rows.Count());

            TableRow singleRow = rows.Single();
            IEnumerable<TableCell> cells = singleRow.Cells;
            Assert.Equal(1, cells.Count());

            TableCell singleCell = cells.Single();
            Assert.Equal(table.HeaderRows.First().Cells.Count(), singleCell.ColSpan);
            Assert.Equal("No functions have run recently.", singleCell.RawElement.Text);
        }

        [Fact]
        public void FunctionsPage_OldHost()
        {
            try
            {
                _storageAccount.CreateOldHostContainer();

                FunctionsPage page = Dashboard.GoToFunctionsPage();

                OldHostNotification notification = page.ErrorsNotificationSection.OldHostNotification;
                Assert.True(notification.IsUserAccesible);
                Assert.Equal(
                    "Jobs from an earlier version of the Azure WebJobs SDK have been detected. Please upgrade the jobs to the latest version in order to see their status in the dashboard. Please visit this article for more information about the Azure WebJobs SDK.",
                    notification.Text);
                Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=401520", notification.HelpUrl.Href);
            }
            finally
            {
                Dashboard.Quit();
                _storageAccount.RemoveOldHostContainer();
            }
        }
    }
}
