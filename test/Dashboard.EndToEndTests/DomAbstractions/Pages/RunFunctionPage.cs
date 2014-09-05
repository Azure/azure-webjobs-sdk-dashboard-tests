// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class RunFunctionPage : DashboardPage
    {
        public const string RelativePath = "function/run?functionId=";

        public RunFunctionPage(IWebDriver driver)
            : base(driver)
        {
        }

        public static string ConstructRelativePath(string functionDefinitonId)
        {
            if (string.IsNullOrWhiteSpace(functionDefinitonId))
            {
                throw new ArgumentNullException("functionDefinitonId");
            }

            return RelativePath + functionDefinitonId;
        }
    }
}
