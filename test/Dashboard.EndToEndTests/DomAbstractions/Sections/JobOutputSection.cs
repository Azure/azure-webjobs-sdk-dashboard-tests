// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using Dashboard.EndToEndTests.HtmlAbstractions.Angular;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class JobOutputSection:PageElement
    {
        internal JobOutputSection(IWebElement sectionElement)
            : base(sectionElement)
        {
            if (sectionElement.TagName != Tags.ConsoleOutput)
            {
                throw new ArgumentException("The element must be of type " + Tags.ConsoleOutput);
            }
        }

        public NgButton ToggleOutputButton
        {
            get
            {
                const string selector = Tags.P + ">" + Tags.Button + ".btn.btn-primary.btn-sm";
                IWebElement button = Waiters.WaitForElementToAppear(() => RawElement.FindElement(By.CssSelector(selector)));

                return new NgButton(button);
            }
        }

        public Link DownloadLogLink
        {
            get
            {
                const string selector = Tags.Span + ".ng-scope >" + Tags.A;
                IWebElement link = Waiters.WaitForElementToAppear(() => RawElement.FindElement(By.CssSelector(selector)));

                return new Link(link);
            }
        }

        public TextArea Output
        {
            get
            {
                const string selector = Tags.TextArea;
                IWebElement textArea = Waiters.WaitForElementToAppear(() => RawElement.FindElement(By.TagName(selector)));

                return new TextArea(textArea);
            }
        }
    }
}
