// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionsTableRow : TableRow
    {
        internal FunctionsTableRow(IWebElement rowElement)
            : base(rowElement, isHeaderRow: false)
        {
            if (!string.Equals(rowElement.GetAttribute("ng-repeat"), "entry in functionDefinitions.page") ||
                rowElement.FindElements(By.TagName(Tags.Td)).Count != 2)
            {
                throw new ArgumentException("The element is not a functions table data row", "rowElement");
            }
        }

        public bool IsHostRunning
        {
            get
            {
                const string selector = Tags.Span + ".glyphicon.glyphicon-exclamation-sign";

                try
                {
                    IWebElement element = this[0].RawElement.FindElement(By.CssSelector(selector));
                    if (element != null)
                    {
                        return false;
                    }
                }
                catch(NoSuchElementException)
                {
                }

                return true;
            }
        }

        public string FunctionName
        {
            get
            {
                return this[0].RawElement.Text;
            }
        }

        public Link FunctionLink
        {
            get
            {
                IWebElement element = this[0].RawElement.FindElement(By.TagName(Tags.A));
                return new Link(element);
            }
        }

        public int SuccessCount
        {
            get
            {
                const string selector =Tags.Span + ".label.label-success";

                IWebElement successLabel = this[1].RawElement.FindElement(By.CssSelector(selector));
                return int.Parse(successLabel.Text);
            }
        }

        public int FailCount
        {
            get
            {
                const string selector = Tags.Span + ".label.label-danger";

                IWebElement failLabel = this[1].RawElement.FindElement(By.CssSelector(selector));
                return int.Parse(failLabel.Text);
            }
        }
    }
}
