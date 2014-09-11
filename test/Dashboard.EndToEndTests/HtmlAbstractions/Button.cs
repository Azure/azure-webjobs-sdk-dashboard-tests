// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class Button : PageElement
    {
        internal Button(IWebElement element)
            : base(element)
        {
            if (element.TagName != Tags.Button)
            {
                throw new ArgumentException("The element must be a button");
            }
        }

        public string Caption
        {
            get
            {
                return RawElement.Text;
            }
        }

        public void Click()
        {
            RawElement.Click();
        }
    }
}
