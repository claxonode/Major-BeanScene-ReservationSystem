
//using Major_BeanScene_ReservationSystem;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Major_BeanScene_ReservationSystem_Test
{
    public class BasicIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public BasicIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }


        //[Fact]
        //public async void TestSomething()
        //{   var client = _factory.CreateClient();

        //    var response = await client.GetAsync("/Home/Index");
        //    response.EnsureSuccessStatusCode();
        //    Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        //    //response.EnsureSuccessStatusCode();

        //    IWebDriver driver = new ChromeDriver();
        //    driver.Navigate().GoToUrl("http://localhost:5240");
        //    //Assert.Equal("text/html;charset=utf-8", response.Content.Headers.ContentType.ToString());
        //    //Major_BeanScene_ReservationSystem.Program.Main()
            

        //}
        //[Fact]
        //public void TestManager()
        //{
        //    IWebDriver driver = new ChromeDriver();
        //    driver.Navigate().GoToUrl("https://localhost:7200/");
        //    //IWebElement clickable = driver.FindElement(By.LinkText("Make a Reservation"));
        //    //clickable.Click();
        //    driver.Quit();
        //}
        //[Fact]
        //public void Test()
        //{
        //    IWebDriver driver = new ChromeDriver();
        //    driver.Navigate().GoToUrl("http://localhost:5240/");
        //    //IWebElement clickable = driver.FindElement(By.CssSelector("href=[Identity/Account/Login]"));
        //    //clickable.Click();
        //    driver.Quit();
        //}
    }
}