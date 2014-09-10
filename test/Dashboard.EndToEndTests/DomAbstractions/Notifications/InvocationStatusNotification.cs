// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class InvocationStatusNotification : Notification
    {
        internal InvocationStatusNotification(IWebElement notification)
            : base(notification)
        {
            if (!notification.GetAttribute("class").Contains("alert") ||
                !notification.GetAttribute("ng-class").Contains("model.invocation.getAlertClass()"))
            {
                throw new ArgumentException("The element is not an invocation status notification");
            }
        }

        public JobStatus Status
        {
            get
            {
                const string selector = Tags.B;
                IWebElement statusBadge = RawElement.FindElement(By.TagName(selector));
                string statusText = statusBadge.Text.Replace(" ", "");

                JobStatus status;
                if (!Enum.TryParse<JobStatus>(statusText, true, out status))
                {
                    throw new InvalidOperationException("The status is not recognized: " + statusText);
                }

                return status;
            }
        }

        public string CompletionTime
        {
            get
            {
                return RawElement
                    .FindElement(By.TagName(Tags.Span))
                    .Text;
            }
        }

        public string RunningTime
        {
            get
            {
                return RawElement
                    .FindElement(By.TagName(Tags.Small))
                    .Text;
            }
        }
    }
}
