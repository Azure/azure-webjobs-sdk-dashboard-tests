// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class SearchBlobPage : DashboardPage
    {
        public const string RelativePath = "function/SearchBlob";

        public SearchBlobPage(IWebDriver driver)
            : base(driver)
        {
        }
    }
}
