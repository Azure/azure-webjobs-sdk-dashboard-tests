// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FunctionArgumentsTable : Table
    {
        internal FunctionArgumentsTable(IWebElement element)
            : base(element)
        {
            TableRow headerRow = HeaderRows.Single();
            if (!string.Equals("PARAMETER", headerRow[0].RawElement.Text) ||
                !string.Equals("VALUE", headerRow[1].RawElement.Text) ||
                !string.Equals("NOTES", headerRow[2].RawElement.Text))
            {
                throw new ArgumentException("Table is not a function arguments table", "element");
            }
        }

        protected override TableRow MapRow(IWebElement rowElement)
        {
            try
            {
                return new FunctionArgumentsTableRow(rowElement);
            }
            catch(ArgumentException)
            {
                return base.MapRow(rowElement);
            }
        }
    }
}
