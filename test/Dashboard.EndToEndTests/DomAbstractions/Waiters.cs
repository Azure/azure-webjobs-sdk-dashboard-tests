// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dashboard.EndToEndTests.HtmlAbstractions;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public static class Waiters
    {
        public static T WaitForElementToAppear<T>(Func<T> actionToWait)
        {
            const int DefaultTimeoutInSeconds = 10;
            return WaitForElementToAppear(actionToWait, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));
        }

        public static T WaitForElementToAppear<T>(Func<T> actionToWait, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime) <= timeout)
            {
                try
                {
                    return actionToWait();
                }
                catch (NoSuchElementException)
                {
                }

                Thread.Sleep(500);
            }

            throw new TimeoutException("Operation failed");
        }

        public static void WaitForDataToLoad(this Table table)
        {
            const int DefaultTimeoutInSeconds = 15;
            WaitForDataToLoad(table, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));
        }

        public static void WaitForDataToLoad(this Table table, TimeSpan timeout)
        {
            try
            {
                DateTime startTime = DateTime.Now;

                while ((DateTime.Now - startTime) <= timeout)
                {
                    IEnumerable<TableRow> rows = table.BodyRows;
                    if (rows.Count() != 1)
                    {
                        return;
                    }

                    TableRow singleRow = rows.Single();
                    IEnumerable<TableCell> cells = singleRow.Cells;
                    if (cells.Count() != 1)
                    {
                        return;
                    }

                    TableCell singleCell = cells.Single();
                    if (!string.Equals("Loading...", singleCell.RawElement.Text))
                    {
                        return;
                    }

                    Thread.Sleep(500);
                }

                throw new TimeoutException("Operation failed");
            }
            catch (StaleElementReferenceException)
            {
                // The table is gone so we are done waiting
            }
        }
    }
}
