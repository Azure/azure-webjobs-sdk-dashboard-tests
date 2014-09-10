// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionArgumentsTableRow : TableRow
    {
        internal FunctionArgumentsTableRow(IWebElement rowElement)
            : base(rowElement, isHeaderRow: false)
        {
            if (!string.Equals(rowElement.GetAttribute("ng-repeat"), "param in model.parameters") ||
                rowElement.FindElements(By.TagName(Tags.Td)).Count != 3)
            {
                throw new ArgumentException("The element is not a function arguments table row", "rowElement");
            }
        }

        public string Name
        {
            get
            {
                return this[0].RawElement
                    .FindElement(By.TagName(Tags.B))
                    .Text;
            }
        }

        public string Value
        {
            get
            {
                return this[1].RawElement
                    .FindElement(By.TagName(Tags.Span))
                    .Text;
            }
        }

        public string Notes
        {
            get
            {
                return this[2].RawElement
                    .Text;
            }
        }
    }
}
