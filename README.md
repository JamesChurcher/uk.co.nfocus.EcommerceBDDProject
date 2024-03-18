# BDD Testing Project
## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Setup](#setup)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
  * [Run settings](#run-settings)
* [Running tests](#running-tests)
* [Troubleshooting](#troubleshooting)

## General Info
This is a project that aims to demonstrate the testing of an e-commerce web application using Behaviour-Driven-Development practices. I created this project as part of a larger training course to demonstrate my knowledge of BDD, Specflow, and Selenium Webdriver.

In this project I was tasked with writing two testcases which, in the Gherkin language, I chose to structure as two scenarios under one feature file. Testcase one is to apply a discount code to a cart and verify if the correct amount was deducted, testcase two is to checkout a cart and verify if the new order created is listed under the account that it was purchased with.

## Technologies
* C#
* .NET 6.0
* NUnit
* Selenium Webdriver
* SpecFlow
* Gherkin

## Setup

### Prerequisites

* Visual Studio 2022

This project requires Visual Studio which can be installed from their [website](https://visualstudio.microsoft.com/downloads/).

### Installation

1. Clone this repository
   ```powershell
   git clone "https://github.com/JamesChurcher/uk.co.nfocus.EcommerceBDDProject.git"
   ```

2. Create an account on the edgewords shop website https://www.edgewordstraining.co.uk/demo-site/ and record the username and password used

    ![Screenshot of login and register page][AccountPage]

3. Open the project solution in Visual Studio

### Run settings

Create a file called 'local.runsettings' with the text below. Update the environment variables and test run parameters, particularly the username and password of the account the tests need to login to.
   ```xml
   <?xml version="1.0" encoding="utf-8" ?>
   <RunSettings>
       <!-- config elements -->
       <RunConfiguration>
           <EnvironmentVariables>
               <!-- Env variables, accessable outside of tests, describes environment -->
               <BROWSER>firefox</BROWSER>
               <SCREENSHOTTOGGLE>All</SCREENSHOTTOGGLE> <!-- All, None -->
           </EnvironmentVariables>
       </RunConfiguration>
       <TestRunParameters>
           <!-- NUnit config params, only tests have access to, describes tests -->
           <Parameter name="WebAppUrl" value="https://www.edgewordstraining.co.uk/demo-site/"/>
           <Parameter name="Username"  value="Account email address"/>
           <Parameter name="Password"  value="Account password"/>
       </TestRunParameters>
   </RunSettings>
   ```

   Environment variables: 
   * BROWSER -> Specifies the browser environment that the test driver will execute the tests
   * SCREENSHOTTOGGLE -> Toggles screenshot capture on and off

   Test run parameters:
   * WebAppUrl -> The url of the website to test
   * Username -> The username of the account to log into
   * Password -> The password of the account to log into

## Running tests

All tests in this project can be viewed and run in the Test Explorer, which can be opened under the view tab: View>Test Explorer

![Screenshot of Test Explorer showing the tests under this project][TestExplorer]

You can run individual test by selecting the test and clicking the single green play symbol ![Single play symbol][SinglePlaySymbol] or run all test by pressing the double green play symbol ![Double play symbol][DoublePlaySymbol]

## Troubleshooting


[AccountPage]: ./README-Assets/AccountPage.png
[TestExplorer]: ./README-Assets/TestExplorer.png
[SinglePlaySymbol]: ./README-Assets/RunIndividualTests.png
[DoublePlaySymbol]: ./README-Assets/RunAllTests.png