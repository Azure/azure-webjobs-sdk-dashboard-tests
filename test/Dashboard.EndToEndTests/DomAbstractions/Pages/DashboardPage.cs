// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;
using TestEasy.WebBrowser;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public abstract class DashboardPage
    {
        private readonly IWebDriver _driver;


        protected DashboardPage(IWebDriver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }

            _driver = driver;
        }

        public IWebDriver Driver
        {
            get
            {
                return _driver;
            }
        }

        public Navbar Navbar
        {
            get
            {
                const string selector = "div.navbar";
                IWebElement navBarElement = _driver.FindElement(By.CssSelector(selector));
                return navBarElement != null ? new Navbar(navBarElement) : null;
            }
        }
    }
}
