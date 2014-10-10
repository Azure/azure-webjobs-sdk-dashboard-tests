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
        public void FunctionsPage_InaccessibleError()
        {
            FunctionsPage page = Dashboard.GoToFunctionsPage();

            BadConnectionStringNotification notification = page.ErrorsNotificationSection.BadConnectionStringNotification;
            Assert.True(notification.IsUserAccesible);
            Assert.Equal("The configuration is not properly set for the Microsoft Azure WebJobs Dashboard. " +
                "Failed to connect with the xxxxxxxxxx storage account using credentials provided in the connection " + 
                "string.\r\nIn your Microsoft Azure Website configuration you must set a connection string named " + 
                "AzureWebJobsDashboard by using the following format " + 
                "DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY pointing to the Microsoft Azure " + 
                "Storage account where the Microsoft Azure WebJobs Runtime logs are stored.\r\n\r\nPlease visit the " + 
                "article about configuring connection strings for more information on how you can configure " + 
                "connection strings in your Microsoft Azure Website.", 
                notification.ErrorMessage);
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
