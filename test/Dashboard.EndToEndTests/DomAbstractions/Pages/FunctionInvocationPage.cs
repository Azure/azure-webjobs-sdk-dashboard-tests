// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionInvocationPage : NgPage
    {
         public const string RelativePath = "#/functions/invocations/";

         internal FunctionInvocationPage(IWebDriver driver)
            : base(driver)
        {
        }

         public static string ConstructRelativePath(string functionInvocationId)
         {
             if (string.IsNullOrWhiteSpace(functionInvocationId))
             {
                 throw new ArgumentNullException("functionInvocationId");
             }

             return RelativePath + functionInvocationId;
         }

         public InvocationDetailsSection DetailsSection
         {
             get
             {
                 const string selector = Tags.Div;
                 IWebElement section = NgScope.FindElement(By.TagName(selector));
                 return new InvocationDetailsSection(section);
             }
         }
    }
}
