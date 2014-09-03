// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class InvalidConnectionStringTests : IUseFixture<InvalidConnectionStringDashboardTestFixture>
    {
        private WebJobsDashboard _dashboard;

        public void SetFixture(InvalidConnectionStringDashboardTestFixture data)
        {
            _dashboard = data.CreateDashboard();
        }

        [Fact]
        public void FunctionsPage_BadCredentialsConnectionString()
        {
            _dashboard.GoTo(typeof(FunctionsPage));
            FunctionsPage page = _dashboard.FunctionsPage;

            BadConnectionStringNotification notification = page.ErrorsNotificationSection.BadConnectionStringNotification;
            Assert.True(notification.IsUserAccesible);
            Assert.Equal("AzureWebJobsDashboard", notification.ConnectionStringName);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=320957", notification.HelpUrl.Href);
        }

        [Fact]
        public void NavbarHasOnlyJobsButtons()
        {
            _dashboard.GoTo(typeof(JobsPage));
            JobsPage page = _dashboard.JobsPage;
            
            // Verify navbar
            IEnumerable<Link> links = page.Navbar.NavLinks;
            Assert.Equal(1, links.Count());

            Link functionLink = links.First();
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("Functions", functionLink.Text);
            Assert.Equal(_dashboard.BuildFullUrl(FunctionsPage.RelativePath), functionLink.Href);
        }
    }
}
