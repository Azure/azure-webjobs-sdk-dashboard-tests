// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionDefinitionPage : NgPage
    {
        public const string RelativePath = "#/functions/definitions/";

        internal FunctionDefinitionPage(IWebDriver driver)
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

        public DefinitionSection DefinitionSection
        {
            get
            {
                const string selector = ".//" + Tags.Div;
                IWebElement definitionSection = Waiters.WaitForElementToAppear(() => NgScope.FindElement(By.XPath(selector)));
                return new DefinitionSection(definitionSection);
            }
        }
    }
}
