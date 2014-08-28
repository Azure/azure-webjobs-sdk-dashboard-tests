// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public abstract class NotificationSection : PageElement
    {
        public NotificationSection(IWebElement areaElement)
            :base(areaElement)
        {
            if (areaElement.TagName != Tags.NgInclude)
            {
                throw new ArgumentException("The element must be a ng-include");
            }
        }

        protected IWebElement Content
        {
            get
            {
                return RawElement.FindElement(By.TagName(Tags.Section));
            }
        }
    }
}
