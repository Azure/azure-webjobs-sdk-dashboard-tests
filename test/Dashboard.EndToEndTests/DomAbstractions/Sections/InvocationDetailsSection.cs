// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class InvocationDetailsSection : PageElement
    {
        internal InvocationDetailsSection(IWebElement sectionElement)
            : base(sectionElement)
        {
        }

        public Link ReplayFunctionLink
        {
            get
            {
                const string selector = Tags.P + ">" + Tags.A + ".btn";
                IWebElement linkElement = RawElement.FindElement(By.CssSelector(selector));
                return new Link(linkElement);
            }
        }

        public Header Title
        {
            get
            {
                IWebElement displayHeader = Waiters.WaitForElementToAppear(() => RawElement.FindElement(By.TagName(Tags.H3)));
                return new Header(displayHeader);
            }
        }

        public InvocationStatusNotification StatusNotification
        {
            get
            {
                const string selector = ".//" + Tags.P + "[@ng-class='model.invocation.getAlertClass()']";
                IWebElement notification = Waiters.WaitForElementToAppear(() => RawElement.FindElement(By.XPath(selector)));

                return new InvocationStatusNotification(notification);
            }
        }

        public string ExceptionMessage
        {
            get
            {
                const string selector = ".//" + Tags.Div + "[@ng-if='model.invocation.exceptionMessage']";
                IWebElement details = RawElement.FindElement(By.XPath(selector));

                return details.Text;
            }
        }

        public string InvokeReason
        {
            get
            {
                const string glyphSelector = Tags.P + ">" + Tags.Span + ".glyphicon";
                IWebElement glyphicon = RawElement.FindElement(By.CssSelector(glyphSelector));

                const string reasonSelector = ".//following-sibling::" + Tags.Span;
                IWebElement reason = glyphicon.FindElement(By.XPath(reasonSelector));

                return reason.Text;
            }
        }

        public FunctionArgumentsTable ArgumentsTable
        {
            get
            {
                IWebElement tableElement = Waiters.WaitForElementToAppear(() => RawElement.FindElement(By.TagName(Tags.Table)));
                return new FunctionArgumentsTable(tableElement);
            }
        }
    }
}
