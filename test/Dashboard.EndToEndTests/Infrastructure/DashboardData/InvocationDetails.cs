// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
namespace Dashboard.EndToEndTests.Infrastructure.DashboardData
{
    public class InvocationDetails
    {
        public string Id { get; set; }

        public IDictionary<string, Argument> Arguments { get; set; }

        public bool Succeeded { get; set; }
    }
}
