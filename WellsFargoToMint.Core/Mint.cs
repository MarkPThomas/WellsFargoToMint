// NuGet: "Selenium.WebDriver", "Selenium.Support", and "Selenium.WebDriver.ChromeDriver"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MPT.WellsFargoToMint.Core
{
    // REFERENCE Taken from <a href="https://www.codeproject.com/Articles/1183561/Importing-transactions-into-Mint-com-using-Csharp">https://www.codeproject.com/Articles/1183561/Importing-transactions-into-Mint-com-using-Csharp</a>
    public class Mint
    {

        #region Properties
        public static string Date = DateTime.Now.ToString("yyyyMMddHHmm");
        public static string Self = System.Reflection.Assembly.GetEntryAssembly().Location;
        public static string Root = Path.GetDirectoryName(Self);
        public static ArgumentTable Arguments = new ArgumentTable();
        public static LogFile Log = new LogFile();
        public static int ExitCode;
        #endregion

        #region Methods
        public static int Main(string[] args)
        {
            try
            {
                // start
                if (!Arguments.ContainsKey("transactionfile")) Arguments["transactionfile"] = Path.Combine(Root, "transactions.csv");       // defaults to .\transactions.csv
                if (!Arguments.ContainsKey("name")) { Console.Write("Enter email or user id: "); Arguments["name"] = Console.ReadLine(); }  // required. TODO: Change this for GUI
                if (!Arguments.ContainsKey("password")) { Console.Write("Enter password: "); Arguments["password"] = ReadPassword(); }      // required. TODO: Change this for GUI

                if (!Arguments.ContainsKey("logfile")) Arguments["logfile"] =
                        Path.Combine(Root, $"{Path.GetFileNameWithoutExtension(Self)}-{Date}.log");
                Log.Open(Arguments["logfile"]);
                Log.Message("Start");
                Log.Debug(Arguments.ToString());

                // 1. load csv data into an array of objects
                Log.Trace("Loading CSV data...");
                List<Transaction> transactions = File.ReadAllLines(Arguments["transactionfile"]).Skip(1).Select(Transaction.FromCsv).ToList();

                // 2. Upload csv data to Mint
                using (IWebDriver driver = new ChromeDriver())
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    // 2a. open <a href="http://mint.com/">mint.com</a>
                    Log.Trace("Opening website...");
                    driver.Url = "<a href=\"https://mint.intuit.com/login.event?referrer=direct&soc=&utm=\">https://mint.intuit.com/login.event?referrer=direct&soc=&utm=</a>";

                    // 3. login
                    Log.Trace("Logging in...");
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ius-userid")));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ius-password")));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ius-sign-in-submit-btn")));
                    driver.FindElement(By.Id("ius-userid")).SendKeys(Arguments["name"]);
                    driver.FindElement(By.Id("ius-password")).SendKeys(Arguments["password"]);
                    driver.FindElement(By.Id("ius-sign-in-submit-btn")).Submit();

                    // 4. navigate to transactions page
                    Log.Trace("Navigating to transaction page...");
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href*='transaction.event']")));
                    driver.FindElement(By.CssSelector("a[href*='transaction.event']")).Click();
                    System.Threading.Thread.Sleep(3000); // MAGIC, let the new page load; sometimes the first transaction fails because the form is add-cash but the fields are an existing transaction and not "Enter Description"

                    // 5. import transactions
                    Log.Trace("Importing transactions...");
                    foreach (var transaction in transactions)
                    {
                        Log.Debug("Found {0}", transaction.ToString());

                        // a. open form
                        Log.Trace("Opening form..");
                        wait.Until(ExpectedConditions.ElementExists(By.Id("txnEdit")));
                        wait.Until(ExpectedConditions.ElementExists(By.Id("txnEdit-form")));
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("controls-add")));

                        Log.Debug("#txnEdit class = {0}", driver.FindElement(By.Id("txnEdit")).GetAttribute("class"));
                        wait.Until(d => d.FindElement(By.Id("txnEdit")).GetAttribute("class") == "single regular");

                        Log.Debug("#txnEdit-form class = {0}", driver.FindElement(By.Id("txnEdit-form")).GetAttribute("class"));
                        wait.Until(d => d.FindElement(By.Id("txnEdit-form")).GetAttribute("class").Contains("hide"));
                        driver.Scripts().ExecuteScript("document.getElementById('controls-add').click()"); // driver...Click() sometimes failed

                        // b. enter values
                        Log.Trace("Entering values..");
                        Log.Debug("#txnEdit class = {0}", driver.FindElement(By.Id("txnEdit")).GetAttribute("class"));
                        wait.Until(d => d.FindElement(By.Id("txnEdit")).GetAttribute("class") == "add cash");

                        Log.Debug("#txnEdit-form class = {0}", driver.FindElement(By.Id("txnEdit-form")).GetAttribute("class"));
                        wait.Until(d => d.FindElement(By.Id("txnEdit-form")).GetAttribute("class").Contains("hide") == false);
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("txnEdit-date-input")));
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("txnEdit-merchant_input")));
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("txnEdit-category_input")));
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("txnEdit-amount_input")));

                        Log.Debug("#txnEdit-merchant_input value = {0}", (string)driver.Scripts().ExecuteScript("return document.getElementById('txnEdit-merchant_input').value"));
                        wait.Until(d => (string)d.Scripts().ExecuteScript("return document.getElementById('txnEdit-merchant_input').value") == "Enter Description");  // the most important safety check, otherwise you might override existing data

                        driver.Scripts().ExecuteScript("document.getElementById('txnEdit-date-input').value = arguments[0]", transaction.Date); // .SendKeys doesn't work for this field
                        driver.FindElement(By.Id("txnEdit-merchant_input")).SendKeys(transaction.Merchant);
                        driver.FindElement(By.Id("txnEdit-category_input")).SendKeys(transaction.Category);
                        driver.FindElement(By.Id("txnEdit-amount_input")).SendKeys(transaction.Amount);
                        if (transaction.Type == TransactionType.Expense)
                        {
                            driver.FindElement(By.Id("txnEdit-mt-expense")).Click();
                            if (driver.FindElement(By.Id("txnEdit-mt-cash-split")).Selected) driver.FindElement(By.Id("txnEdit-mt-cash-split")).Click();
                        }
                        else
                        {
                            driver.FindElement(By.Id("txnEdit-mt-income")).Click();
                        }
                        driver.FindElement(By.Id("txnEdit-note")).SendKeys("Imported transaction.");

                        // c. submit form
                        Log.Trace("Submitting form..");
                        if (!Arguments.ContainsKey("whatif")) // submit
                        {
                            driver.FindElement(By.Id("txnEdit-submit")).Click();
                        }
                        else // pretend
                        {
                            driver.FindElement(By.Id("txnEdit-cancel")).Click();
                        }
                        Log.Message("Imported {0}", transaction);
                        System.Threading.Thread.Sleep(3000); // MAGIC, safety net, let the submit cook
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex.Message);
                ExitCode = 255;
            }
            finally
            {
                // finish
                Log.Message("Finished [{0}]", ExitCode);
                Log.Close();
            }

            return ExitCode;
        }

        // TODO: Change this for GUI
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key != ConsoleKey.Backspace)
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
                else if (password.Length > 0)
                {
                    password = password.Substring(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
                keyInfo = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }
        #endregion
    } 
}
