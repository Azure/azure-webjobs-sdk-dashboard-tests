// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class BadConnectionStringNotification : Notification
    {
        internal BadConnectionStringNotification(IWebElement notification)
            : base(notification)
        {
            if (!notification.GetAttribute("class").Contains("alert") ||
                !notification.GetAttribute("class").Contains("alert-danger"))
            {
                throw new ArgumentException("The element must be a danger alert");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return RawElement.Text;
            }
        }

        public string ConnectionStringName
        {
            get
            {
                const string selector = ".//" + Tags.Code + "[1]";
                return RawElement.FindElement(By.XPath(selector)).Text;
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
