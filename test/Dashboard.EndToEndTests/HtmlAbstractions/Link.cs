// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class Link : PageElement
    {
        private const string HrefAttribute = "href";

        internal Link(IWebElement linkElement)
            : base (linkElement)
        {
            if (linkElement.TagName != Tags.A)
            {
                throw new ArgumentException("The web element must be a link");
            }
        }

        public string Text
        {
            get
            {
                return RawElement.Text;
            }
        }

        public string Href
        {
            get
            {
                return RawElement.GetAttribute(HrefAttribute);
            }
        }
    }
}
