// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.HtmlAbstractions.Angular
{
    public class NgButton : Button
    {
        internal NgButton(IWebElement button)
            :base(button)
        {
        }

        public string ClickAction
        {
            get
            {
                const string ngClickAttributeName = "ng-click";
                return RawElement.GetAttribute(ngClickAttributeName);
            }
        }
    }
}
