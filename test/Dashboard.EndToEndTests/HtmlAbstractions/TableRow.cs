// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class TableRow : PageElement
    {
        private readonly bool _isHeaderRow;

        public TableRow(IWebElement element, bool isHeaderRow)
            : base(element)
        {
            if (element.TagName != Tags.Tr)
            {
                throw new ArgumentException("Element must be a table row", "element");
            }

            _isHeaderRow = isHeaderRow;
        }

        public IEnumerable<TableCell> Cells
        {
            get
            {
                return RawElement
                    .FindElements(By.TagName(_isHeaderRow ? Tags.Th : Tags.Td))
                    .Select(td => new TableCell(td));
            }
        }

        public TableCell this[int index]
        {
            get
            {
                return Cells.ElementAt(index);
            }
        }
    }
}
