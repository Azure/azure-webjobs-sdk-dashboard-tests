// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class Header : PageElement
    {
        private static readonly Regex headerRegex = new Regex(@"h\d", RegexOptions.Compiled);

        public Header(IWebElement element)
            : base(element)
        {
            if (!headerRegex.IsMatch(element.TagName))
            {
                throw new ArgumentException("The element must be a header", "element");
            }
        }

        public string Text
        {
            get
            {
                return RawElement.Text;
            }
        }
    }
}
