// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class SdkTeaserNotification : Notification
    {
        public SdkTeaserNotification(IWebElement notification)
            : base(notification)
        {            
        }

        public Link MoreInfoUrl
        {
            get
            {
                IWebElement linkElement = RawElement.FindElement(By.TagName(Tags.A));
                return new Link(linkElement);
            }
        }
    }
}
