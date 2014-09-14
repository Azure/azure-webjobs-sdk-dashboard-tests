// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionArgumentsTableRowValueCell : TableCell
    {
        internal FunctionArgumentsTableRowValueCell(IWebElement cell)
            :base(cell)
        {
        }

        public Link BlobLink
        {
            get
            {
                By selector=By.TagName(Tags.A);
                IWebElement linkElement = RawElement.FindElement(selector);
                return new Link(linkElement);
            }
        }

        public string TextValue
        {
            get
            {
                By selector = By.TagName(Tags.Span);

                return RawElement
                    .FindElement(selector)
                    .Text;
            }
        }
    }
}
