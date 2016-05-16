// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TestEasy.WebServer;

namespace Dashboard.EndToEndTests.Infrastructure
{
    public class DashboardServer : IDisposable
    {
        private const string ApplicationName = "WebJobsDashboard";
        private const string ConnectionStringFormat = "<add name=\"AzureWebJobsDashboard\" connectionString=\"(?<cs>[^\"]*)\" />"; 

        private WebServer _webServer;
        private WebApplicationInfo _webApplicationInfo;
        private string _virtualPath;
        private string _webConfigFilePath;

        private bool _disposed;

        public DashboardServer(string deployPath, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(deployPath))
            {
                throw new ArgumentNullException(deployPath);
            }

            if (connectionString == null)
            {
                throw new ArgumentNullException(connectionString);
            }

            CreateWebServer(deployPath, connectionString);
        }

        private void CreateWebServer(string deployPath, string connectionString)
        {
            const int MaxRetries = 3;
            int attempt = 0;

            while (true)
            {
                try
                {
                    _webServer = WebServer.Create("iisexpress");
                    _webApplicationInfo = _webServer.CreateWebApplication(ApplicationName);
                    _webServer.DeployWebApplication(_webApplicationInfo.Name, new List<DeploymentItem>
                        {
                            new DeploymentItem { Type = DeploymentItemType.Directory, Path = deployPath }
                        });

                    _webConfigFilePath = Path.Combine(_webServer.RootPhysicalPath, ApplicationName, "web.config");
                    SetStorageConnectionString(connectionString);

                    _virtualPath = _webApplicationInfo.VirtualPath;

                    return;
                }
                catch
                {
                    if (attempt++ < MaxRetries)
                    {
                        // Swallow any startup errors and retry after a short
                        // period
                        // We're seeing some File IO issues from TestEasy that look
                        // like timing issues
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        // We're unable to create the server even after retries
                        throw;
                    } 
                }
            }
        }

        public string VirtualPath
        {
            get
            {
                GuardNotDisposed();
                return _virtualPath;
            }
        }

        public void Start()
        {
            _webServer.Start();
        }

        private void SetStorageConnectionString(string connectionString)
        {
            const string connectionStringGroupName = "cs";

            GuardNotDisposed();

            string webConfigFileContent = File.ReadAllText(_webConfigFilePath);

            if (connectionString == null)
            {
                webConfigFileContent = Regex.Replace(webConfigFileContent, ConnectionStringFormat, String.Empty);
            }
            else
            {
                webConfigFileContent = Regex.Replace(webConfigFileContent, ConnectionStringFormat, m =>
                {
                    string capture = m.Value;
                    capture = capture.Remove(m.Groups[connectionStringGroupName].Index - m.Index, m.Groups[connectionStringGroupName].Length);
                    capture = capture.Insert(m.Groups[connectionStringGroupName].Index - m.Index, connectionString);
                    return capture;
                });
            }

            File.WriteAllText(_webConfigFilePath, webConfigFileContent);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _webServer.Stop();
                _webServer.Dispose();
            }
        }

        private void GuardNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(DashboardServer).Name);
            }
        }
    }
}
