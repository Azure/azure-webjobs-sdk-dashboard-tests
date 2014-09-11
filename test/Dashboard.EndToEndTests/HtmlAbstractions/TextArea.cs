// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class TextArea : PageElement
    {
        internal TextArea(IWebElement element)
            : base(element)
        {
            if (element.TagName != Tags.TextArea)
            {
                throw new ArgumentException("The element must be a " + Tags.TextArea);
            }
        }

        public string Text
        {
            get
            {
                return RawElement.Text;
            }
        }

        public int Rows
        {
            get
            {
                return int.Parse(RawElement.GetAttribute("rows"));
            }
        }
    }
}
