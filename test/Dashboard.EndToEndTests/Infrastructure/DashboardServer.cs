// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Web.Administration;
using TestEasy.Core.Configuration;
using TestEasy.WebServer;

namespace Dashboard.EndToEndTests.Infrastructure
{
    public class DashboardServer : IDisposable
    {
        private const string ApplicationName = "WebJobsDashboard";
        private const string ConnectionStringFormat = ".*<add name=\"AzureWebJobsDashboard\" connectionString=\"(?<cs>[^\"]*)\" />.*";

        private static readonly Regex _connectionStringRegex = new Regex(ConnectionStringFormat, RegexOptions.Compiled); 

        private readonly WebServer _server;
        private readonly Application _application;
        private readonly string _virtualPath;
        private readonly string _webConfigFilePath;

        private bool _disposed;

        public DashboardServer(string deployPath)
        {
            if (string.IsNullOrWhiteSpace(deployPath))
            {
                throw new ArgumentNullException(deployPath);
            }
            
            _server = WebServer.Create(WebServerType.IISExpress);
            _application = _server.CreateWebApplication(ApplicationName);
            _application.Deploy(deployPath);

            _webConfigFilePath = Path.Combine(_server.RootPhysicalPath, ApplicationName, "web.config");
            _virtualPath = _server.GetApplicationVirtualPath(_application, TestEasyConfig.Instance.Client.Remote);
        }

        public string VirtualPath
        {
            get
            {
                GuardNotDisposed();
                return _virtualPath;
            }
        }

        public void SetStorageConnectionString(string connectionString)
        {
            const string connectionStringGroupName = "cs";

            GuardNotDisposed();

            if (connectionString == null)
            {
                connectionString = string.Empty;
            }

            string webConfigFileContent = File.ReadAllText(_webConfigFilePath);

            webConfigFileContent = _connectionStringRegex.Replace(webConfigFileContent, m =>
            {
                string capture = m.Value;
                capture = capture.Remove(m.Groups[connectionStringGroupName].Index - m.Index, m.Groups[connectionStringGroupName].Length);
                capture = capture.Insert(m.Groups[connectionStringGroupName].Index - m.Index, connectionString);
                return capture;
            });

            File.WriteAllText(_webConfigFilePath, webConfigFileContent);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _server.Dispose();
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
