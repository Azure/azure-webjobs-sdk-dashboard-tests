// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using Dashboard.EndToEndTests.Infrastructure;
using TestEasy.WebBrowser;

namespace Dashboard.EndToEndTests
{
    public class DashboardTestFixture : IDisposable
    {
        private readonly DashboardServer _server;
        private readonly WebJobsStorageAccount _storage;

        private bool _isDisposed;

        private WebJobsDashboard _dashboard;

        public DashboardTestFixture()
            : this(cleanStorageAccount: true)
        {
        }

        public DashboardTestFixture(bool cleanStorageAccount)
        {
            // "DashboardSiteExtensionLocation" should point to a root site extension directory (unzipped),
            // which is the directory where the "extension.xml" file lives.
            string dashboardLocation = GetFromConfigOrEnvironmentOrDefault("DashboardSiteExtensionLocation");
            dashboardLocation = Directory.GetDirectories(dashboardLocation, "?.*.*").SingleOrDefault();
            if (string.IsNullOrEmpty(dashboardLocation))
            {
                throw new Exception("Unable to find Dashboard site extension. Make sure you've configured 'DashboardSiteExtensionLocation' correctly.");
            }
            string dashboardVersion = Path.GetFileName(dashboardLocation);
            Console.WriteLine("Testing with Dashboard version '{0}'", dashboardVersion);

            _server = new DashboardServer(dashboardLocation);
            _storage = new WebJobsStorageAccount(GetFromConfigOrEnvironmentOrDefault("StorageAccount"));
            _server.SetStorageConnectionString(_storage.ConnectionString);

            if (cleanStorageAccount)
            {
                _storage.Empty();
            }
        }

        public DashboardServer Server
        {
            get
            {
                GuardNotDisposed();

                return _server;
            }
        }

        public WebJobsStorageAccount StorageAccount
        {
            get
            {
                GuardNotDisposed();

                return _storage;
            }
        }

        public WebJobsDashboard CreateDashboard()
        {
            GuardNotDisposed();

            if (_dashboard == null || _dashboard.IsDisposed)
            {
                string browserType = GetFromConfigOrEnvironmentOrDefault("TestBrowser");

                BrowserType browser;
                if (!Enum.TryParse<BrowserType>(browserType, true, out browser))
                {
                    throw new ArgumentException("Unknown browser type: " + browserType);
                }

                _dashboard = new WebJobsDashboard(_server.VirtualPath, browser);
            }

            return _dashboard;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (_server != null)
                {
                    _server.Dispose();
                }
                
                if (_dashboard != null)
                {
                    _dashboard.Dispose();
                    _dashboard = null;
                }
            }
        }

        private void GuardNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(typeof(DashboardServer).Name);
            }
        }

        private string GetFromConfigOrEnvironmentOrDefault(string keyName)
        {
            string value = ConfigurationManager.AppSettings[keyName];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            value = Environment.GetEnvironmentVariable(keyName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            throw new InvalidOperationException("App setting is required: " + keyName);
        }
    }
}
