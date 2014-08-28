// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class AboutPage : NgPage
    {
        public const string RelativePath = "#/about";

        internal AboutPage(IWebDriver driver)
            : base(driver)
        {
        }
    }
}
