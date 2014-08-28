// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionsPage : NgPage
    {
        public const string RelativePath = "#/functions";

        public FunctionsPage(IWebDriver driver)
            : base(driver)
        {
        }

        public FunctionsSection FunctionsSection
        {
            get
            {
                const string selector = ".//" + Tags.Div + "[1]";
                IWebElement invocationsSection = Waiters.WaitForElementToAppear(() => NgScope.FindElement(By.XPath(selector)));
                return new FunctionsSection(invocationsSection);
            }
        }

        public InvocationsSection InvocationsSection
        {
            get
            {
                const string selector = ".//" + Tags.Div + "[2]";
                IWebElement invocationsSection = Waiters.WaitForElementToAppear(() => NgScope.FindElement(By.XPath(selector)));
                return new InvocationsSection(invocationsSection);
            }
        }
    }
}
