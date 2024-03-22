# BDD Testing Project
## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Setup](#setup)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
  * [Run settings](#run-settings)
    * [Env variables and params](#environment-variables-and-test-run-parameters)
* [Running tests](#running-tests)
  * [Running from Visual Studio](#running-from-visual-studio)
  * [Running from terminal](#running-from-the-terminal)
* [Generating a SpecFlow LivingDoc report](#generating-a-specflow-livingdoc-report)
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

### Run settings

Create a file called `local.runsettings` inside the folder `.\uk.co.nfocus.EcommerceBDDProject\uk.co.nfocus.EcommerceBDDProject` with the text below. Update the environment variables and test run parameters, particularly the username and password of the account previously created that the tests will login to.
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

#### Environment variables and Test run parameters

Here you can see what each variable and parameter is for and what possible values you can set for them

| Field name       | Description                                                                   | Values                |
| ---------------- | ----------------------------------------------------------------------------- | --------------------- |
| BROWSER          | Specifies the browser environment that the test driver will execute the tests | edge, chrome, firefox |
| SCREENSHOTTOGGLE | Toggles screenshot capture on and off                                         | All, None             |
| WebAppUrl        | The url of the website to test                                                |                       |
| Username         | The username of the account to log into                                       |                       |
| Password         | The password of the account to log into                                       |                       |

## Running tests

These tests can be run from both the terminal and Visual Studio and the following instructions lay out both methods

### Running from Visual Studio

1. First open the project solution file `uk.co.nfocus.EcommerceBDDProject.csproj` in Visual Studio
2. Configure the project to use your run settings file. Go to Test>Configure Run Settings>Select Solution Wide runsettings File and select the `local.runsettings` file created previously
3. Build the project with `ctrl+b`

Visual Studio is an easier method to run and view the results of these tests. All tests in this project can be viewed and run in the Test Explorer, which can be opened under the view tab: View>Test Explorer

![Screenshot of Test Explorer showing the tests under this project][TestExplorer]

You can run individual test by selecting the test and clicking the single green play symbol ![Single play symbol][SinglePlaySymbol] or run all test by pressing the double green play symbol ![Double play symbol][DoublePlaySymbol]

Tests will return as passing or failing as a green tick or red cross respectively. To view the output of an individual test, select a test and scroll through the log on the bottom half of the test explorer called the test detail summary

![Screenshot of the test detail summary][TestDetailSummary]

### Running from the terminal

Using the terminal is a lightweight method to run these tests and the following steps will demonstrate this

1. Open a new terminal and navigate to the project folder `.\uk.co.nfocus.EcommerceBDDProject\uk.co.nfocus.EcommerceBDDProject`. The same one that contains the runsettings file

2. Run the command
   ```powershell
   dotnet test --settings "local.runsettings"
   ```

This will run the tests and the terminal will display the output and results which should look something like the following:
```powershell
PS \> dotnet test --settings "local.runsettings"
  Determining projects to restore...
  All projects are up-to-date for restore.
  SpecFlowFeatureFiles: Features\CheckoutSystem.feature
  -> Using default config
  SpecFlowGeneratedFiles: Features\CheckoutSystem.feature.cs
  uk.co.nfocus.EcommerceBDDProject -> .\uk.co.nfocus.EcommerceBDDProject\uk.co.nfocus
  .EcommerceBDDProject\bin\Debug\net6.0\uk.co.nfocus.EcommerceBDDProject.dll
Test run for .\uk.co.nfocus.EcommerceBDDProject\uk.co.nfocus.EcommerceBDDProject\bin\Debug\net6.0\uk.co.nfocus.EcommerceBDDProject.dll (.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.9.0 (x64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
-> Username and Password have been set
-> Browser is set to: firefox
#... Test output ...

#Results
Passed!  - Failed:     0, Passed:     5, Skipped:     0, Total:     5, Duration: 1 m 49 s - uk.co.nfocus.EcommerceBDDProject.dll (net6.0)
PS \>
```

## Generating a SpecFlow LivingDoc report

This project also supports generating a LivingDoc report, the outputs and test attachments are included specifically for this method of reporting.

![Screenshot of a generated LivingDoc report where all the tests have passed][LivingDocReport]

To generate a report you can use the following commands:

1. First navigate into the folder storing the test results from inside the project folder
   ```powershell
   cd "bin\Debug\net6.0"
   ```

2. Then use this command to generate a new livingdoc report
   ```powershell
   livingdoc test-assembly uk.co.nfocus.EcommerceBDDProject.dll -t TestExecution.json
   ```

3. Access the report by opening the newly created file `LivingDoc.html` in your prefered browser

## Troubleshooting


[AccountPage]: ./README-Assets/AccountPage.png
[TestExplorer]: ./README-Assets/TestExplorer.png
[SinglePlaySymbol]: ./README-Assets/RunIndividualTests.png
[DoublePlaySymbol]: ./README-Assets/RunAllTests.png
[TestDetailSummary]: ./README-Assets/TestDetailSummary.png
[LivingDocReport]: ./README-Assets/LivingDocReport.png
