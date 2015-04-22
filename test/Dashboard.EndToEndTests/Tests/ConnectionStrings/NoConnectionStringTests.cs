// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class NoConnectionStringTestsFixture : DashboardTestFixture
    {
        public NoConnectionStringTestsFixture()
            : base(cleanStorageAccount: false)
        {
            Server.SetStorageConnectionString(null);
        }
    }

    public class NoConnectionStringTests : DashboardTestClass<NoConnectionStringTestsFixture>
    {
        [Fact]
        public void NavbarTitle()
        {
            JobsPage page = Dashboard.GoToJobsPage();
            
            // Verify title
            Link title = page.Navbar.TitleLink;
            Assert.True(title.IsUserAccesible);
            Assert.Equal("Microsoft Azure WebJobs", title.Text);
            Assert.Equal(Dashboard.BuildFullUrl(JobsPage.RelativePath), title.Href);
        }

        [Fact]
        public void Navbar_Buttons()
        {
            JobsPage page = Dashboard.GoToJobsPage();
            
            // Verify navbar buttons
            IEnumerable<Link> links = page.Navbar.NavLinks;
            Assert.Equal(1, links.Count());

            Link functionLink = links.Single();
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("Functions", functionLink.Text);
            Assert.Equal(Dashboard.BuildFullUrl(FunctionsPage.RelativePath), functionLink.Href);
        }

        [Fact]
        public void JobsPage_NoJobs()
        {
            JobsPage page = Dashboard.GoToJobsPage();
            
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
        public void FunctionsPage_SdkTeaser()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            SdkTeaserNotification sdkTeaserNotification = page.SdkTeaserNotificationSection.SdkTeaserNotification;
            Assert.True(sdkTeaserNotification.IsUserAccesible);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=320954", sdkTeaserNotification.MoreInfoUrl.Href);
        }
    }
}
