﻿using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System;
using Xunit;

namespace Intranet.SeleniumTests
{
  public class CakeTest : IDisposable
  {
    private IWebDriver driver;

    public CakeTest()
    {
      //Setup
    }

    public void Dispose()
    {
      //Teardown
      driver.Quit();
    }


    [Theory]
    [InlineData("http://www.certaincy.com/", "Certaincy")]
    public void CertaincyTest(string url, string keyword)
    {
      driver = new PhantomJSDriver(@"C:\Users\Victor Dalqvist\Documents\Intranet\src\Intranet.SeleniumTests\bin\Debug\netcoreapp1.1\");
      driver.Navigate().GoToUrl(url);
      Assert.True(driver.Title.Contains(keyword));
    }
  }
}