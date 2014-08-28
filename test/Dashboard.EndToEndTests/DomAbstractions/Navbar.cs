// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using OpenQA.Selenium;
using System.Linq;
using Dashboard.EndToEndTests.HtmlAbstractions;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class Navbar : PageElement
    {
        public Navbar(IWebElement navBar)
            : base(navBar)
        {
        }

        public Link TitleLink
        {
            get
            {
                const string selector = Tags.A + ".navbar-brand";
                IWebElement titleLink = RawElement.FindElement(By.CssSelector(selector));
                return new Link(titleLink);
            }
        }

        public IEnumerable<Link> NavLinks
        {
            get
            {
                const string selector = Tags.Div + " > " + Tags.Ul + ".nav > " + Tags.Li + " > " + Tags.A;
                IEnumerable<IWebElement> linkElements = RawElement.FindElements(By.CssSelector(selector));
                return linkElements.Select(a => new Link(a)).ToList();
            }
        }
    }
}
