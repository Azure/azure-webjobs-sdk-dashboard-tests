// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dashboard.EndToEndTests.DomAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions
{
    public class Table : PageElement
    {
        internal Table(IWebElement tableElement)
            : base(tableElement)
        {
            if (tableElement.TagName != Tags.Table)
            {
                throw new ArgumentException("The element must be a table", "tableElement");
            }
        }

        public IEnumerable<TableRow> HeaderRows
        {
            get
            {
                const string selector = ".//" + Tags.THead + "/" + Tags.Tr;

                return RawElement.FindElements(By.XPath(selector)).Select(tr => new TableRow(tr, isHeaderRow: true));
            }
        }

        public IEnumerable<TableRow> BodyRows
        {
            get
            {
                const string selector = ".//" + Tags.TBody + "/" + Tags.Tr;

                return  Waiters.WaitForElementToAppear(() => RawElement.FindElements(By.XPath(selector)))
                    .Select(tr => MapRow(tr));
            }
        }

        protected virtual TableRow MapRow(IWebElement rowElement)
        {
            return new TableRow(rowElement, isHeaderRow: false);
        }
    }
}
