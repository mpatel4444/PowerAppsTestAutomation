using OpenQA.Selenium;
using System.Diagnostics;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.PowerApps.TestAutomation.Browser
{
    public static class SeleniumExtensionsBase
    {

        [DebuggerNonUserCode()]
        public static JArray GetJsonArray(this IWebDriver driver, string @object)
        {
            @object = SanitizeReturnStatement(@object);

            var results = ExecuteScript(driver, $"return JSON.stringify({@object});").ToString();

            return JArray.Parse(results);
        }
    }
}
