// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class TableCell : PageElement
    {
        public TableCell(IWebElement cellElement)
            : base(cellElement)
        {
            if (cellElement.TagName != Tags.Td && cellElement.TagName != Tags.Th)
            {
                throw new ArgumentException("Element must be a table cell");
            }
        }

        public int ColSpan
        {
            get
            {
                string attributeValue = RawElement.GetAttribute("colspan");
                return !string.IsNullOrWhiteSpace(attributeValue) ? int.Parse(attributeValue) : 1;
            }
        }
    }
}
