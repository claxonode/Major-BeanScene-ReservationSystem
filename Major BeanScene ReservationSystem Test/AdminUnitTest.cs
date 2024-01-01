
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Major_BeanScene_ReservationSystem.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Major_BeanScene_ReservationSystem_Test
{
    [Collection("Sequential")]
    public class AdminUnitTest
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;
        public AdminUnitTest()
        {
            var options = new ChromeOptions();
            
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5240");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(d => driver.FindElement(By.Id("Login")).Displayed);
            IWebElement login = driver.FindElement(By.Id("Login"));
            login.Click();

            //IWebElement email = driver.FindElement(By.Id("Input_Email"));
            //IWebElement password = driver.FindElement(By.Id("Input_Password"));
            //IWebElement loginButton = driver.FindElement(By.Id("login-submit"));
            //email.SendKeys("admin@admin.com");
            //password.SendKeys("mM123!@#");
            //loginButton.Click();
        }
        [Theory]
        [InlineData("admin@admin.com","invalid")]
        public void InvalidUserCredentials_ShouldNotLogin(string email, string password)
        {
            IWebElement emailField = driver.FindElement(By.Id("Input_Email"));
            IWebElement passwordField = driver.FindElement(By.Id("Input_Password"));
            IWebElement loginButton = driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys(email);
            passwordField.SendKeys(password);
            loginButton.Click();
            wait.Until(d => driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']")).Displayed);
            IWebElement error = driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']"));
            Assert.Equal("Invalid login attempt.", error.Text);
            driver.Quit();
        }

        [Theory]
        [InlineData("admin@admin.com", "mM123!@#", "http://localhost:5240/Manager")]
        public void ValidUserCredentials_ShouldLogin(string email, string password,string url)
        {
            IWebElement emailField = driver.FindElement(By.Id("Input_Email"));
            IWebElement passwordField = driver.FindElement(By.Id("Input_Password"));
            IWebElement loginButton = driver.FindElement(By.Id("login-submit"));

            emailField.SendKeys(email);
            passwordField.SendKeys(password);
            loginButton.Click();
            Assert.Equal(url, driver.Url);

            driver.Quit();
        }

        [Theory]
        [InlineData("http://localhost:5240/doesNotExists")]
        public void NavigatingToPageThatDoesNotExist_ShouldReturnNotFoundPage(string url)
        {
            driver.Navigate().GoToUrl(url);
            IWebElement errorHeader = driver.FindElement(By.TagName("h1"));
            Assert.Equal("(404) Page Not Found.",errorHeader.Text);
            driver.Quit();
        }

        //[Theory]
        //[InlineData(20,"Test sitting")]
        //public void CorrectValuesForSitting_ReturnTrue(int capacity, string sittingTitle)
        //{
        //    driver.Navigate().GoToUrl("http://localhost:5240/Manager/Sittings/Create");


        //    DateTime tommorrow = DateTime.Now.AddDays(1).Date;
        //    DateTime startTime = tommorrow.AddHours(7);
        //    DateTime endTime = tommorrow.AddHours(9);
        //    IWebElement sittingInput = driver.FindElement(By.Id("Name"));
        //    IWebElement start = driver.FindElement(By.Id("StartTime"));
        //    IWebElement end = driver.FindElement(By.Id("EndTime"));
        //    IWebElement submitSitting = driver.FindElement(By.CssSelector("input[value='Create']"));
        //    IWebElement capacityInput = driver.FindElement(By.Id("Capacity"));
        //    capacityInput.Clear();
        //    capacityInput.SendKeys(capacity.ToString());
        //    sittingInput.SendKeys(sittingTitle);
        //    start.SendKeys(tommorrow.ToString("d"));
        //    //start.SendKeys(Keys.ArrowRight);
        //    //start.SendKeys("70");
        //    end.SendKeys(tommorrow.ToString("d"));
        //    submitSitting.Click();


        //    //List<IWebElement> tableRows = driver.FindElements(By.TagName("tr")).ToList();
        //    IWebElement newSitting = driver.FindElement(By.CssSelector("tbody tr:nth-last-child(1)"));///Contains all values
        //    var newSittingValues = newSitting.Text;
        //    bool checkCapacity = newSittingValues.Contains(capacity.ToString());
        //    bool checkSittingTlte = newSittingValues.Contains(sittingTitle);
        //    Assert.True(checkSittingTlte);
        //    Assert.True(checkCapacity);
        //    IWebElement delete = newSitting.FindElement(By.LinkText("Delete")); //Need to wait until clickable
        //    delete.Click();

        //    IWebElement deleteConfirm = driver.FindElement(By.CssSelector("input[value='Delete']"));
        //    deleteConfirm.Click();
        //    driver.Quit();

        //}
    }
}
