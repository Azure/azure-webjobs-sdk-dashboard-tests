// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs;

namespace Dashboard.EndToEndTests
{
    public class SingleFunction
    {
        [NoAutomaticTrigger]
        public static void Function(bool fail, bool logOnSuccess, TextWriter log)
        {
            if (fail)
            {
                log.WriteLine("This function will always fail in this case");
                throw new InvalidOperationException();
            }
            else if (logOnSuccess)
            {
                log.WriteLine("logOnSuccess is enabled");
            }
        }
    }
}
