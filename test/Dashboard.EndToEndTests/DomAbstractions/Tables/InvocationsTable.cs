// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class InvocationsTable : Table
    {
        internal InvocationsTable(IWebElement element)
            : base(element)
        {
            TableRow headerRow = HeaderRows.Single();
            if (!headerRow[0].RawElement.Text.StartsWith("FUNCTION") ||
                !string.Equals("STATUS", headerRow[1].RawElement.Text) ||
                !string.Equals("STATUS DETAIL", headerRow[2].RawElement.Text))
            {
                throw new ArgumentException("Table is not an invocations table", "element");
            }
        }

        protected override TableRow MapRow(IWebElement rowElement)
        {
            try
            {
                return new InvocationsTableRow(rowElement);
            }
            catch(ArgumentException)
            {
                return base.MapRow(rowElement);
            }
        }
    }
}
