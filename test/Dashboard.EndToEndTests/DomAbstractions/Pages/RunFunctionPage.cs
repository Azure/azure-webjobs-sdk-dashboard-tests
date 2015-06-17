// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Dashboard.EndToEndTests.DomAbstractions
{
    public class FormElement
    {
        public string Label { get; set; }
        public string InputValue { get; set; }
    }

    public class RunFunctionPage : DashboardPage
    {
        public const string RelativePath = "function/run?functionId=";

        public RunFunctionPage(IWebDriver driver)
            : base(driver)
        {
        }

        public static string ConstructRelativePath(string functionDefinitonId)
        {
            if (string.IsNullOrWhiteSpace(functionDefinitonId))
            {
                throw new ArgumentNullException("functionDefinitonId");
            }

            return RelativePath + functionDefinitonId;
        }

        public Collection<FormElement> GetParameters()
        {
            IWebElement runForm = Driver.FindElement(By.XPath("/html/body/div/form"));
            IWebElement[] parameterRows = runForm.FindElements(By.XPath("fieldset/div")).ToArray();

            Collection<FormElement> formElements = new Collection<FormElement>();
            foreach (IWebElement row in parameterRows)
            {
                if (row.Text == "Run")
                {
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
