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
    public class CleanStorageAccountTests : IUseFixture<DashboardTestFixture>
    {
        private WebJobsDashboard _dashboard;
        private WebJobsStorageAccount _storageAccount;

        public void SetFixture(DashboardTestFixture data)
        {
            _storageAccount = data.StorageAccount;
            _dashboard = data.CreateDashboard();
        }

        [Fact]
        public void NavbarHasAllButtons()
        {
            _dashboard.GoTo(typeof(JobsPage));
            JobsPage page = _dashboard.JobsPage;

            Navbar navbar = page.Navbar;

            // Verify title
            Link title = navbar.TitleLink;
            Assert.True(title.IsUserAccesible);
            Assert.Equal("Microsoft Azure WebJobs", title.Text);
            Assert.Equal(_dashboard.BuildFullUrl(JobsPage.RelativePath), title.Href);

            // Verify navbar
            IEnumerable<Link> links = navbar.NavLinks;
            Assert.Equal(3, links.Count());

            Link functionLink = links.ElementAt(0);
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("Functions", functionLink.Text);
            Assert.Equal(_dashboard.BuildFullUrl(FunctionsPage.RelativePath), functionLink.Href);

            Link searchLink = links.ElementAt(1);
            Assert.True(searchLink.IsUserAccesible);
            Assert.Equal("Search Blobs", searchLink.Text);
            Assert.Equal(_dashboard.BuildFullUrl(SearchBlobPage.RelativePath), searchLink.Href);

            Link aboutLink = links.ElementAt(2);
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("About", aboutLink.Text);
            Assert.Equal(_dashboard.BuildFullUrl(AboutPage.RelativePath), aboutLink.Href);
        }

        [Fact]
        public void FunctionsPageHasNoFunctions()
        {
            _dashboard.GoTo(typeof(FunctionsPage));
            FunctionsPage page = _dashboard.FunctionsPage;

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
        public void FunctionsPageHasNoInvocations()
        {
            _dashboard.GoTo(typeof(FunctionsPage));
            FunctionsPage page = _dashboard.FunctionsPage;

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
        public void FunctionsPageOldHost()
        {
            try
            {
                _storageAccount.CreateOldHostContainer();

                _dashboard.GoTo(typeof(FunctionsPage));
                FunctionsPage page = _dashboard.FunctionsPage;

                OldHostNotification notification = page.ErrorsNotificationSection.OldHostNotification;
                Assert.True(notification.IsUserAccesible);
                Assert.Equal(
                    "Jobs from an earlier version of the Azure WebJobs SDK have been detected. Please upgrade the jobs to the latest version in order to see their status in the dashboard. Please visit this article for more information about the Azure WebJobs SDK.",
                    notification.Text);
                Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=401520", notification.HelpUrl.Href);
            }
            finally
            {
                _dashboard.Quit();
                _storageAccount.RemoveOldHostContainer();
            }
        }
    }
}
