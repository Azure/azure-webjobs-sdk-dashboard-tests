// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class NoConnectionStringTests : IUseFixture<DashboardTestFixture>
    {
        private WebJobsDashboard _dashboard;

        public void SetFixture(DashboardTestFixture data)
        {
            data.Server.SetStorageConnectionString(null);
            _dashboard = data.CreateDashboard();
        }

        [Fact]
        public void NavbarTitle()
        {
            _dashboard.GoTo(typeof(JobsPage));
            JobsPage page = _dashboard.JobsPage;
            
            // Verify title
            Link title = page.Navbar.TitleLink;
            Assert.True(title.IsUserAccesible);
            Assert.Equal("Microsoft Azure WebJobs", title.Text);
            Assert.Equal(_dashboard.BuildFullUrl(JobsPage.RelativePath), title.Href);
        }

        [Fact]
        public void NavbarHasOnlyJobsButtons()
        {
            _dashboard.GoTo(typeof(JobsPage));
            JobsPage page = _dashboard.JobsPage;
            
            // Verify navbar buttons
            IEnumerable<Link> links = page.Navbar.NavLinks;
            Assert.Equal(1, links.Count());

            Link functionLink = links.First();
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("Functions", functionLink.Text);
            Assert.Equal(_dashboard.BuildFullUrl(FunctionsPage.RelativePath), functionLink.Href);
        }

        [Fact]
        public void NoJobs()
        {
            _dashboard.GoTo(typeof(JobsPage));
            JobsPage page = _dashboard.JobsPage;
            
            Header header = page.JobsSection.Title;
            Assert.True(header.IsUserAccesible);
            Assert.Equal("WebJobs", header.Text);

            IEnumerable<TableRow> rows = page.JobsSection.Table.BodyRows;

            Assert.Equal(1, rows.Count());
            TableRow noJobsRow = rows.First();

            IEnumerable<TableCell> cells = noJobsRow.Cells;
            Assert.Equal(1, cells.Count());
            TableCell noJobsCell = cells.First();

            Assert.Equal(3, noJobsCell.ColSpan);
            Assert.Equal("There are no WebJobs in this website. See this article about using WebJobs.", noJobsCell.RawElement.Text);
        }

        [Fact]
        public void FunctionsPage_NoConnectionString()
        {
            _dashboard.GoTo(typeof(FunctionsPage));
            FunctionsPage page = _dashboard.FunctionsPage;

            SdkTeaserNotification sdkTeaserNotification = page.SdkTeaserNotificationSection.SdkTeaserNotification;
            Assert.True(sdkTeaserNotification.IsUserAccesible);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=320954", sdkTeaserNotification.MoreInfoUrl.Href);
        }
    }
}
