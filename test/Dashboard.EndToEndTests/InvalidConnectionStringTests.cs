// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Xunit;

namespace Dashboard.EndToEndTests
{
    public class InvalidConnectionStringTestsFixture : DashboardTestFixture
    {
        public InvalidConnectionStringTestsFixture()
            : base(cleanStorageAccount: false)
        {
            Server.SetStorageConnectionString("DefaultEndpointsProtocol=https;AccountName=xxxxxxxxxx;AccountKey=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX==");
        }
    }

    public class InvalidConnectionStringTests : DashboardTestClass<InvalidConnectionStringTestsFixture>
    {
        [Fact]
        public void FunctionsPage_Warning()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            BadConnectionStringNotification notification = page.ErrorsNotificationSection.BadConnectionStringNotification;
            Assert.True(notification.IsUserAccesible);
            Assert.Equal("AzureWebJobsDashboard", notification.ConnectionStringName);
            Assert.Equal("http://go.microsoft.com/fwlink/?LinkID=320957", notification.HelpUrl.Href);
        }

        [Fact]
        public void Navbar_Buttons()
        {
            JobsPage page = Dashboard.GoToJobsPage();
            
            // Verify navbar
            IEnumerable<Link> links = page.Navbar.NavLinks;
            Assert.Equal(1, links.Count());

            Link functionLink = links.First();
            Assert.True(functionLink.IsUserAccesible);
            Assert.Equal("Functions", functionLink.Text);
            Assert.Equal(Dashboard.BuildFullUrl(FunctionsPage.RelativePath), functionLink.Href);
        }
    }
}
