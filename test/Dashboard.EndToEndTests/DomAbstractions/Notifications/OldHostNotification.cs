// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class OldHostNotification : Notification
    {
        internal OldHostNotification(IWebElement notification)
            : base(notification)
        {
            if (!notification.GetAttribute("class").Contains("alert") ||
                !notification.GetAttribute("class").Contains("alert-warning") ||
                !notification.GetAttribute("ng-if").Equals("isOldHost"))
            {
                throw new ArgumentException("The element is not an old host notification");
            }
        }

        public Link HelpUrl
        {
            get
            {
                IWebElement linkElement = RawElement.FindElement(By.TagName(Tags.A));
                return new Link(linkElement);
            }
        }
    }
}
