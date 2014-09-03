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
        private readonly BrowserType _browserType;

        private readonly string _baseAddress;

        private IWebDriver _driver;

        private bool _isDisposed;

        public WebJobsDashboard(string baseAddress, BrowserType browserType)
        {
            
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException("baseAddress");
            }

            _baseAddress = baseAddress;

            _browserManager = new BrowserManager();
            _browserType = browserType;
        }

        public JobsPage JobsPage
        {
            get
            {
                GuardNotDisposed();

                return new JobsPage(Driver);
            }
        }

        public FunctionsPage FunctionsPage
        {
            get
            {
                GuardNotDisposed();

                return new FunctionsPage(Driver);
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Quit();
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

        public void Quit()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }

        public void GoTo(Type page)
        {
            GuardNotDisposed();

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

        public string BuildFullUrl(string relativeUrl)
        {
            return _baseAddress + "/" + relativeUrl;
        }

        private IWebDriver Driver
        {
            get
            {
                if (_driver == null)
                {
                    _driver = _browserManager.CreateBrowser(_browserType);
                    _driver.Manage().Window.Maximize();
                }

                return _driver;
            }
        }

        private void GoTo(string address)
        {
            // Go to blank instead because some browser can be flaky upon refresh
            // and the page loaded event might fire before the refresh is complete
            Driver.Navigate().GoToUrl("about:blank");
            Driver.WaitForPageLoaded();

            Driver.Navigate().GoToUrl(address);
            Driver.WaitForPageLoaded();
        }

        private void GuardNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(typeof(WebJobsDashboard).Name);
            }
        }
    }
}
