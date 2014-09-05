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
        public static void WaitForAction(Func<bool> action)
        {
            WaitForAction(action, TimeSpan.FromSeconds(15));
        }

        public static void WaitForAction(Func<bool> action, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime) <= timeout)
            {
                if (action())
                {
                    return;
                }

                Thread.Sleep(500);
            }

            throw new TimeoutException("Operation failed");
        }

        public static T WaitForElementToAppear<T>(Func<T> actionToWait)
        {
            return WaitForElementToAppear(actionToWait, TimeSpan.FromSeconds(10));
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
            WaitForDataToLoad(table, TimeSpan.FromSeconds(15));
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
