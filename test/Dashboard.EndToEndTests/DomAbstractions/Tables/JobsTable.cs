// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class JobsTable : Table
    {
        internal JobsTable(IWebElement element)
            : base(element)
        {
            TableRow headerRow = HeaderRows.Single();
            if (!string.Equals("NAME", headerRow[0].RawElement.Text) ||
                !string.Equals("STATUS", headerRow[1].RawElement.Text) ||
                !string.Equals("LAST RUN TIME", headerRow[2].RawElement.Text))
            {
                throw new ArgumentException("Table is not a jobs table", "element");
            }
        }
    }
}
