// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Dashboard.EndToEndTests
{
    public class InvalidConnectionStringDashboardTestFixture : DashboardTestFixture
    {
        public InvalidConnectionStringDashboardTestFixture()
            : base(cleanStorageAccount: false)
        {
            Server.SetStorageConnectionString("DefaultEndpointsProtocol=https;AccountName=xxxxxxxxxx;AccountKey=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX==");
        }
    }
}
