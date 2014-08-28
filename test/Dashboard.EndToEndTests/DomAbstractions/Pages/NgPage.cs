// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public abstract class NgPage : DashboardPage
    {
        protected NgPage(IWebDriver driver)
            : base(driver)
        {
        }

        public IWebElement NgScope
        {
            get
            {
                const string selector = Tags.Div + ".container" + " > " + Tags.Div + ".ng-scope";
                return Waiters.WaitForElementToAppear(() => Driver.FindElement(By.CssSelector(selector)));
            }
        }

        public ErrorsNotificationSection ErrorsNotificationSection
        {
            get
            {
                const string selector = ".//" + Tags.NgInclude + "[@src=\"'app/views/shared/Errors.html'\"]";
                IWebElement notificationElement = Waiters.WaitForElementToAppear(() => NgScope.FindElement(By.XPath(selector)));
                return new ErrorsNotificationSection(notificationElement);
            }
        }

        public SdkTeaserNotificationSection SdkTeaserNotificationSection
        {
            get
            {
                const string selector = ".//" + Tags.NgInclude + "[@src=\"'app/views/shared/SdkTeaser.html'\"]";
                IWebElement notificationElement = Waiters.WaitForElementToAppear(() => NgScope.FindElement(By.XPath(selector)));
                return new SdkTeaserNotificationSection(notificationElement);
            }
        }
    }
}
