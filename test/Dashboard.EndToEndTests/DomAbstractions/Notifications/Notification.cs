// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public abstract class Notification : PageElement
    {
        public Notification(IWebElement notification)
            :base(notification)
        {
            if (notification.TagName != Tags.P)
            {
                throw new ArgumentException("The element must be a paragraph");
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
