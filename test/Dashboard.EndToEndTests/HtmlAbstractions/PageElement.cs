// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public abstract class PageElement
    {
        private readonly IWebElement _rawElement;

        protected PageElement(IWebElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            _rawElement = element;
        }

        public IWebElement RawElement
        {
            get
            {
                return _rawElement;
            }
        }

        public bool IsUserAccesible
        {
            get
            {
                return 
                    _rawElement.Enabled && 
                    _rawElement.Displayed && 
                    _rawElement.Size.Height > 0 && _rawElement.Size.Width > 0;
            }
        }
    }
}
