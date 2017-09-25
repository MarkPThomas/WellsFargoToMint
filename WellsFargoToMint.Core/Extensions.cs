// NuGet: "Selenium.WebDriver", "Selenium.Support", and "Selenium.WebDriver.ChromeDriver"

using OpenQA.Selenium;

namespace MPT.WellsFargoToMint.Core
{
    // REFERENCE Taken from <a href="https://www.codeproject.com/Articles/1183561/Importing-transactions-into-Mint-com-using-Csharp">https://www.codeproject.com/Articles/1183561/Importing-transactions-into-Mint-com-using-Csharp</a>
    public static class Extensions
    {
        // REFERENCE <a href="http://stackoverflow.com/questions/6229769/execute-javascript-using-selenium-webdriver-in-c-sharp">http://stackoverflow.com/questions/6229769/execute-javascript-using-selenium-webdriver-in-c-sharp</a>
        public static IJavaScriptExecutor Scripts(this IWebDriver driver)
        {
            return (IJavaScriptExecutor) driver;
        }
    }
}

    
