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
        private const string LoadingText = "Loading...";

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

        public static void WaitForDataToLoad(this TextArea textArea, TimeSpan timeout)
        {
            WaitForAction(
                () => !string.Equals(LoadingText, textArea.Text),
                timeout);

            // Wait another second because it takes a while to render the actual text
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public static void WaitForDataToLoad(this Table table, TimeSpan timeout)
        {
            try
            {
                WaitForAction(() =>
                    {
                        IEnumerable<TableRow> rows = table.BodyRows;
                        if (rows.Count() != 1)
                        {
                            return true;
                        }

                        TableRow singleRow = rows.Single();
                        IEnumerable<TableCell> cells = singleRow.Cells;
                        if (cells.Count() != 1)
                        {
                            return true;
                        }

                        TableCell singleCell = cells.Single();
                        if (!string.Equals(LoadingText, singleCell.RawElement.Text))
                        {
                            return true;
                        }

                        return false;
                    },
                    timeout);
            }
            catch (StaleElementReferenceException)
            {
                // The table is gone so we are done waiting
            }
        }

        #region Without the timeout argument

        public static void WaitForDataToLoad(this Table table)
        {
            WaitForDataToLoad(table, TimeSpan.FromSeconds(15));
        }

        public static void WaitForDataToLoad(this TextArea textArea)
        {
            WaitForDataToLoad(textArea, TimeSpan.FromSeconds(15));
        }

        public static void WaitForAction(Func<bool> action)
        {
            WaitForAction(action, TimeSpan.FromSeconds(25));
        }

        public static T WaitForElementToAppear<T>(Func<T> actionToWait)
        {
            return WaitForElementToAppear(actionToWait, TimeSpan.FromSeconds(10));
        }

        #endregion
    }
}
