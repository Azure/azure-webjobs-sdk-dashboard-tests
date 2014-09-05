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

        private readonly Api _api;

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

            _api = new Api(baseAddress);
        }

        public Api Api
        {
            get
            {
                return _api;
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

        public JobsPage GoToJobsPage()
        {
            GuardNotDisposed();

            GoTo(BuildFullUrl(JobsPage.RelativePath));
            return new JobsPage(Driver);
        }

        public FunctionsPage GoToFunctionsPage()
        {
            GuardNotDisposed();

            GoTo(BuildFullUrl(FunctionsPage.RelativePath));
            return new FunctionsPage(Driver);
        }

        public FunctionDefinitionPage GoToFunctionDefinitionPage(string functionDefinitonId)
        {
            GuardNotDisposed();

            GoTo(BuildFullUrl(FunctionDefinitionPage.ConstructRelativePath(functionDefinitonId)));
            return new FunctionDefinitionPage(Driver);
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
