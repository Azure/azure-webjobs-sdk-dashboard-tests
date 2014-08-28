// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionsTable : Table
    {
        internal FunctionsTable(IWebElement element)
            : base(element)
        {
            TableRow headerRow = HeaderRows.Single();
            if (!string.Equals("FUNCTION NAME", headerRow[0].RawElement.Text) ||
                !string.Equals("STATISTICS", headerRow[1].RawElement.Text))
            {
                throw new ArgumentException("Table is not a functions table", "element");
            }
        }

        protected override TableRow MapRow(IWebElement rowElement)
        {
            try
            {
                return new FunctionsTableRow(rowElement);
            }
            catch(ArgumentException)
            {
                return base.MapRow(rowElement);
            }
        }
    }
}
