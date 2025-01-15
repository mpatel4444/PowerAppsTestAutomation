using Microsoft.PowerApps.TestAutomation.Browser;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

internal static class SeleniumExtensionsHelpers
{

    [DebuggerNonUserCode()]
    public static JObject GetJsonObject(this IWebDriver driver, string @object)
    {
        @object = SanitizeReturnStatement(@object);

        var results = ExecuteScript(driver, $"return JSON.stringify({@object});").ToString();

        return JObject.Parse(results);
    }

        public static JObject WaitForTestResults(this IWebDriver driver, int maxWaitTimeInSeconds)
        {
            // Wait for app frame
            driver.WaitUntilVisible(By.Id("fullscreen-app-host"), TimeSpan.FromSeconds(10));

            // Switch to app frame
            driver.SwitchTo().Frame("fullscreen-app-host");

            // Define for current state of TestExecution
            int testExecutionState = 0;
            bool state = false;
            JObject jsonResultString = new JObject();

            try
            {
                //Poll every half second to see if UCI is idle
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));
                wait.Until(d =>
                {
                    try
                    {
                        // Check to see if ExecutionState is Complete(2) or Error(3)
                        jsonResultString = driver.GetJsonObject("AppMagic.TestStudio.GetTestExecutionInfo()");
                        testExecutionState = (int)jsonResultString.GetValue("ExecutionState");

                        if (testExecutionState == 0 || testExecutionState == 1)
                        {
                            state = false;
                        }
                        else if (testExecutionState == 2 || testExecutionState == 3)
                        {

                            state = true;
                        }
                    }
                    catch (TimeoutException)
                    {
                        Debug.WriteLine($"jsonResultString is {jsonResultString}.");
                        throw new Exception($"A timeout occurred while attempting to retrieve the ExecutionState of the current test. Current Execution State is: {testExecutionState}");
                    }
                    catch (NullReferenceException)
                    {

                    }

                    return state;
                });
            }
            catch (TimeoutException te)
            {
                throw new Exception($"A timeout occurred while attempting to retrieve the ExecutionState of the current test. {te}");
            }
            catch (Exception)
            {

            }

            Debug.WriteLine($"jsonResultString is {jsonResultString}.");
            return jsonResultString;
        }
}
