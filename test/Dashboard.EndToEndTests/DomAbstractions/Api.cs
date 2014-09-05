// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class Api
    {
        private const string DiagnosticsApi = "/api/diagnostics";
        private const string IndexingQueueLengthApi = DiagnosticsApi + "/indexingQueueLength?limit=";

        private readonly string _baseAddress;

        internal Api(string baseAddress)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException("baseAddress");
            }

            _baseAddress = baseAddress;
        }

        public int IndexingQueueLength(int limit)
        {
            return IndexingQueueLengthAsync(limit).GetAwaiter().GetResult();
        }

        public async Task<int> IndexingQueueLengthAsync(int limit)
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(_baseAddress + IndexingQueueLengthApi + limit.ToString());
            return int.Parse(response);
        }
    }
}
