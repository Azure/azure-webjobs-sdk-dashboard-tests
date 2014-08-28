// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class JobsPage : NgPage
    {
        public const string RelativePath = "#";

        internal JobsPage(IWebDriver driver)
            : base(driver)
        {
        }

        public JobsSection JobsSection
        {
            get
            {
                const string selector = Tags.Div + ".row.ng-scope >" + Tags.Div;
                IWebElement sectionElement = NgScope.FindElement(By.CssSelector(selector));
                return new JobsSection(sectionElement);
            }
        }
    }
}
