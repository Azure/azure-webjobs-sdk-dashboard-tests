// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class ErrorsNotificationSection : NotificationSection
    {
        internal ErrorsNotificationSection(IWebElement notificationArea)
            : base(notificationArea)
        {
            if (notificationArea.GetAttribute("src") != "'app/views/shared/Errors.html'")
            {
                throw new ArgumentException("The area is not for error notifications");
            }
        }

        public BadConnectionStringNotification BadConnectionStringNotification
        {
            get
            {
                const string selector = Tags.P + ".alert.alert-danger";
                IWebElement notificationElement = Waiters.WaitForElementToAppear(() => Content.FindElement(By.CssSelector(selector)));
                return new BadConnectionStringNotification(notificationElement);
            }
        }

        public OldHostNotification OldHostNotification
        {
            get
            {
                const string selector = Tags.P + ".alert.alert-warning";
                IWebElement notificationElement = Waiters.WaitForElementToAppear(() => Content.FindElement(By.CssSelector(selector)));
                return new OldHostNotification(notificationElement);
            }
        }
    }
}
