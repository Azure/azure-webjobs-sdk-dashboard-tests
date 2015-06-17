// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public abstract class InvokeFunctionPage : DashboardPage
    {
        public InvokeFunctionPage(IWebDriver driver)
            : base(driver)
        {
        }

        public virtual Collection<FormElement> GetParameters()
        {
            IWebElement runForm = Driver.FindElement(By.XPath("/html/body/div/form"));
            IWebElement[] parameterRows = runForm.FindElements(By.XPath("fieldset/div")).ToArray();

            Collection<FormElement> formElements = new Collection<FormElement>();
            foreach (IWebElement row in parameterRows)
            {
                if (row.Text == "Run" ||
                    row.Text == "Replay")
                {
                    // skip the button
                    continue;
                }

                var inputBox = row.FindElement(By.TagName("input"));
                FormElement formElement = new FormElement
                {
                    Label = row.Text,
                    InputValue = inputBox.GetAttribute("value")
                };
                formElements.Add(formElement);
            }

            return formElements;
        }
    }
}
