// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class ReplayFunctionPage : InvokeFunctionPage
    {
        public const string RelativePath = "function/replay?parentId=";

        public ReplayFunctionPage(IWebDriver driver)
            : base(driver)
        {
        }

        public static string ConstructRelativePath(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                throw new ArgumentNullException("parentId");
            }

            return RelativePath + parentId;
        }
    }
}
