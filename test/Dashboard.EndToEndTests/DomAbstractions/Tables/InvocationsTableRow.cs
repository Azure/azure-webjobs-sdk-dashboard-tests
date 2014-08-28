// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class InvocationsTableRow : TableRow
    {
        internal InvocationsTableRow(IWebElement rowElement)
            : base(rowElement, isHeaderRow: false)
        {
            if (!string.Equals(rowElement.GetAttribute("ng-repeat"), "entry in invocations.page") ||
                rowElement.FindElements(By.TagName(Tags.Td)).Count != 3)
            {
                throw new ArgumentException("The element is not a jobs table row", "rowElement");
            }
        }

        public string InvocationDisplayText
        {
            get
            {
                return InvocationLink.Text;
            }
        }

        public Link InvocationLink
        {
            get
            {
                IWebElement element = this[0].RawElement.FindElement(By.TagName(Tags.A));
                return new Link(element);
            }
        }

        public JobStatus Status
        {
            get
            {
                const string selector = Tags.Span + ".label";
                IWebElement statusBadge = this[1].RawElement.FindElement(By.CssSelector(selector));
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
                return this[2].RawElement
                    .FindElement(By.TagName(Tags.Span))
                    .Text;
            }
        }

        public string RunningTime
        {
            get
            {
                return this[2].RawElement
                    .FindElement(By.TagName(Tags.Small))
                    .Text;
            }
        }
    }
}
