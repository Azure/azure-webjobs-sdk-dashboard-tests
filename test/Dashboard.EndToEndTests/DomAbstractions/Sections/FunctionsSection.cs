﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionsSection : PageElement
    {
        internal FunctionsSection(IWebElement sectionElement)
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

        public FunctionsTable Table
        {
            get
            {
                IWebElement tableElement = RawElement.FindElement(By.TagName(Tags.Table));
                return new FunctionsTable(tableElement);
            }
        }
    }
}
