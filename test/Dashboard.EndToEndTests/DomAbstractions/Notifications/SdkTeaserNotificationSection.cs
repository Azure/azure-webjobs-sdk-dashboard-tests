// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class SdkTeaserNotificationSection : NotificationSection
    {
        public SdkTeaserNotificationSection(IWebElement section)
            : base(section)
        {
            if (section.GetAttribute("src") != "'app/views/shared/SdkTeaser.html'")
            {
                throw new ArgumentException("The area is not for sdk teaser notifications");
            }
        }

        public SdkTeaserNotification SdkTeaserNotification
        {
            get
            {
                IWebElement notificationElement = Content.FindElement(By.TagName(Tags.P));
                return new SdkTeaserNotification(notificationElement);
            }
        }
    }
}
