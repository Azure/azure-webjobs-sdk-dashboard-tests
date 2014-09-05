// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class DefinitionSection : PageElement
    {
        internal DefinitionSection(IWebElement sectionElement)
            : base(sectionElement)
        {
        }

        public Header Title
        {
            get
            {
                IWebElement displayHeader = RawElement.FindElement(By.TagName(Tags.H3));
                return new Header(displayHeader);
            }
        }

        public Link RunFunctionLink
        {
            get
            {
                const string selector = "p>a.btn";
                IWebElement linkElement = RawElement.FindElement(By.CssSelector(selector));
                return new Link(linkElement);
            }
        }

        public InvocationsTable Table
        {
            get
            {
                IWebElement tableElement = RawElement.FindElement(By.TagName(Tags.Table));
                return new InvocationsTable(tableElement);
            }
        }
    }
}
