// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;
using TestEasy.WebBrowser;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class WebJobsDashboard : IDisposable
    {
        private readonly BrowserManager _browserManager;
        private readonly IWebDriver _driver;

        private readonly string _baseAddress;

        private Lazy<JobsPage> _jobsPage;
        private Lazy<FunctionsPage> _functionsPage;

        private bool _isDisposed;

        public WebJobsDashboard(string baseAddress, BrowserType browserType)
        {
            
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException("baseAddress");
            }

            _baseAddress = baseAddress;

            _browserManager = new BrowserManager();
            _driver = _browserManager.CreateBrowser(browserType);
            _driver.Manage().Window.Maximize();

            _jobsPage = new Lazy<JobsPage>(() => new JobsPage(_driver));
            _functionsPage = new Lazy<FunctionsPage>(() => new FunctionsPage(_driver));
        }

        public JobsPage JobsPage
        {
            get
            {
                return _jobsPage.Value;
            }
        }

        public FunctionsPage FunctionsPage
        {
            get
            {
                return _functionsPage.Value;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _browserManager.Dispose();
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        public void GoTo(Type page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            if (page == typeof(JobsPage))
            {
                GoTo(BuildFullUrl(JobsPage.RelativePath));
            }
            else if (page == typeof(FunctionsPage))
            {
                GoTo(BuildFullUrl(FunctionsPage.RelativePath));
            }
            else
            {
                throw new ArgumentException("Unknown page type: " + page);
            }
        }

        public DashboardPage GetCurrentPage()
        {
            DashboardPage page = null;

            if (_driver.PageSource.StartsWith(BuildFullUrl(JobsPage.RelativePath)))
            {
                page = JobsPage;
            }
            else if (_driver.PageSource.StartsWith(BuildFullUrl(FunctionsPage.RelativePath)))
            {
                page = FunctionsPage;
            }

            return page;
        }

        public string BuildFullUrl(string relativeUrl)
        {
            return _baseAddress + "/" + relativeUrl;
        }

        private void GoTo(string address)
        {
            // Go to blank instead because some browser can be flaky upon refresh
            // and the page loaded event might fire before the refresh is complete
            _driver.Navigate().GoToUrl("about:blank");
            _driver.WaitForPageLoaded();
            
            _driver.Navigate().GoToUrl(address);
            _driver.WaitForPageLoaded();
        }
    }
}
