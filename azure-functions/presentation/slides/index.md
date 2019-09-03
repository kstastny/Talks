- title : Azure Functions
- description : Overview of Azure Functions for the noninitiated
- author : Karel Šťastný
- theme : night 
- transition : none

***

# Azure Functions



***

## Outline

* Serverless
* What are Azure Functions
* Implementing Azure Functions
    * Function App
    * Triggers
    * Bindings
* Deployment
* Monitoring    


* TODO PREPARE WHAT TO SAY EXACTLY


***

## Serverless

* Cloud provider fully manages server infrastructure
* You only worry about application code and business logic

' https://bravenewgeek.com/serverless-on-gcp/
' obligatory definition of serverless. who doesn't know what is serverless? who knows? skip or not

***

### Serverless Flavors

* Compute
* Databases
* Storage
* Message Queues


' Compute - Functions, Lambda; Databases - SQL, CosmosDb; Storage - BlobStorage, S3;

***

### Serverless benefits

* Focus on providing value to your customer
* "outsource" operations related tasks
* automatic scaling, fault tolerance

***

## Azure Functions

* Serverless computing service on Azure
* Function as a Service 
* Stateless*
* Event-Driven
 
 <p class="reference">*except Durable Functions</p>

' FaaS - run code without managing own server/container or own long lived application
' no worry about infrastructure - just write function, define how to run it and run. No need for complex deployment process
' stateless - Durable Functions are stateful but I will not talk about them


***

### Benefits

* no need to maintain infrastructure
* eliminated boilerplate
* rapid and simple development
* lower cost
* automated scaling
* all the power of Azure Web Apps

' pricing - you pay only when the function runs (pay as you go). optionally also dedicated plan. Premium plan in preview, see https://docs.microsoft.com/en-us/azure/azure-functions/functions-scale
' Azure Web Apps - Functions based on WebJobs. Kudu, CI etc. available
' autoscaling - you cannot over or under provision
    TODO prepare a few notes to autoscaling - there are still limits per Function App, no silver bullet

***

### Drawbacks

* vendor lock-in
* performance - extra latency when spinned down (TODO TEST IF ITS TRUE)
* monitoring and debugging resource consumption ?
* not for everyone
* sometimes shows us what was DLL Hell
    * TODO UČESAT SLIDE
    * various DLL incompatibilities and breaking changes, e.g. //needs Microsoft.WindowsAzure.Storage.dll 9.3.1, workaround here https://github.com/Azure/azure-functions-host/issues/3784.
* no in-server state

' TODO see also https://martinfowler.com/articles/serverless.html#drawbacks for stuff to talk about
' performance -
' TODO other cons? https://en.wikipedia.org/wiki/Serverless_computing

***

## Use Cases
 
* System Glue
* Full backend application
* Automating processes
* Scheduled jobs
* Dynamic workloads


' glue - simple to develop, has to be small and flexible. glue - integration of services, apps apod.
' Automating processes - e.g. webhook for automatic restart on alarm
' Dynamic workloads - because of autoscaling
' replaces webjobs

***

## Examples

* Automatic log cleaning
* Automatic restart of App Service on alarm (webhook)

TODO other examples (ask Roman, Landy) - I am working on the app below, then ask

' log cleaning - DB and files from app service


***

### **DEMO** Create Function (F#)

> HTTP Hello world

' possible in portal but good just for demo purposes, therefore we won't be doing that
' TODO PREPARE during the talk - how to do it, in Demo.FunctionApp (or new solution). call the function
' TODO also show from CLI? If there's time for it, prepare. just Hello world, rest in Rider

***

### Azure Functions prerequisities

* Command line - [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
* VS Code - Core Tools + [Azure Functions Extension](https://github.com/Microsoft/vscode-azurefunctions)
* Visual Studio - [Azure Functions and WebJobs Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs)
* JetBrains Rider - [Azure Toolkit](https://plugins.jetbrains.com/plugin/11220-azure-toolkit-for-rider)

***

### Function App

* "project" for Azure Functions
* multiple functions
* shared configuration
* deployed as one unit

' you can think of it as your "app service" or "web" that has the functionality
' show in Azure Portal

***

### Azure Functions Versions

* v1 - .NET Framework 4.7
* v2 - .NET Core 2.2
    * recommended version
    * support for some languages dropped (bash, php, F# Scripts)

see https://docs.microsoft.com/en-us/azure/azure-functions/functions-versions

' I will show v2 only

***

### Function Triggers

* define how the function will start
* each function has exactly one trigger
* often has payload which is provided to the function

' https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings

***

### Function Triggers

* HTTP
* Timer
* Blob Storage
* Cosmos DB
* Azure Service Bus
* Azure Queues
* ...

' we already saw HTTP Trigger, I will present more later

***

### Function Bindings

* Allow connecting function to another resource
* optional
* function may have multiple bindings
* `input`, `output` or both
* abstracts access to other services


' one function can have multiple bindings, `in` or `out` parameters
TODO think of example, https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings
e.g. Mark Heath - function is HTTP Triggered but using binding reads data from Table Storage

***

### Function Bindings

* Blob storage
* Cosmos DB
* SignalR
* Table Storage

***

### **DEMO** Function Triggers and Bindings

> Getting Statistics from [BGG](https://boardgamegeek.com/geekplay.php?userid=199696&redirect=1&startdate=&dateinput=&dateinput=&enddate=&action=bydate&subtype=boardgame)

' problem - get data about games from BoardGameGeek and somehow process them

***

TODO DEMO APPLICATION - IMAGE how it should look in the end, draw.io with Azure icons

 * locally - needs Azure Storage Emulator (show). or develop against azure
        * https://hub.docker.com/r/microsoft/azure-storage-emulator/
        * or Azurite (https://github.com/Azure/Azurite)
 * implement HTTP Trigger with [<Out>] queue binding
 * implement Queue Trigger with Blob binding
        * basic + then show imperative bindings. TODO PREPARE THE CODE
            TODO https://simonholman.blog/azure-functions-with-imperative-bindings/
 * rest just paste and describe - show attribute on "return" + alternative Table Storage
 * show recommended application structure
        - functions are just thin layer that will call business logic, same as if we used Web API or Giraffe or whatever

' HTTP Launcher - will queue the task. Then queue will trigger download, then blob will be parsed and result goes to TableStorage

' NOTE: BlobTrigger processes blobs that were already present but the trigger did not see them before

***


### Deployment

* Manual
    * CLI Tooling
    * IDE Tooling (Visual Studio, VS Code, Rider)
* Automated
    * Azure DevOps
    * Deployment Center

' plenty of options, I will show manual ones
' CLI, IDE - nice for testing before CI is set
' CI - show Deployment Center or Azure DevOps (same as deploying App Service)        
' Deployment Center - needs special structure of app. https://docs.microsoft.com/en-us/azure/azure-functions/functions-continuous-deployment 

***

### **DEMO** Deployment

* JetBrains Rider
* CLI

'CLI `func azure functionapp publish functionsdemo82` (needs `az` or `Az.Accounts` PowerShell module. login with `az login`)

***

### Hosting plans

* Consumption
* App Service Plan
* Premium (preview)

' pricing differes
' App Service - you have to set the scaling correctly but you pay nothing extra ("only" for the app service plan)
' Premium - consumption with extra features (no cold start, unlimited execution duration, ...)
' https://docs.microsoft.com/en-us/azure/azure-functions/functions-scale
' Consumption plan - can be limited by daily usage quota. https://azure.microsoft.com/is-is/pricing/details/functions/
' minimum execution time and memory for a single function execution is 100 ms and 128 mb respectively (i.e. one run is minimum of 1/80 GB-sec)


***

### **DEMO** Function Settings

* Configurable the same way as Azure App Service
    * e.g. `local.settings.json`
    * Azure portal


' TODO explain how to configure locally, how to configure when deployed, how to access configuration
' TODO prepare example - see SwitchMonitoring getConfigurationRoot (ConfigurationBuilder)

***

### Auth

* Function Keys
    * Host Keys
    * Function Keys
* App Service Authentication / Authorization ( Authentication Providers)
* Azure API Management (allows IP restrictions)
* Azure App Service environment
* custom Auth
* TODO application gateway?

' Function Keys are meant for development
' https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook#authorization-keys
' Host - Master key - Admin Access
' Query string or `x-functions-key` header
' Authentication Providers - same as App Services (Azure AD, MS Account, Google, Facebook, Twitter, custom)
' custom https://www.ben-morris.com/custom-token-authentication-in-azure-functions-using-bindings/

TODO test that it works as documented (might not, see https://markheath.net/post/managing-azure-function-keys), let them know
    * `system` requires master key



*** 
### Monitoring

TODO how to monitor, where are the logs, where are the launches, Function console
    * some logs are in Table storage - show
    * or Log Streaming
    * normally logs go to AppInsights if configured, otherwise some basic info is in Portal itself
        * appInsights | Logs (Analytics) | traces
    TODO show both to demo difference
TODO what if they are not visible?

TODO show logs in Azure Portal, logs in Table Storage or where they are
TODO AppInsights, see https://docs.microsoft.com/en-us/azure/azure-functions/functions-monitoring

TODO logging https://stackoverflow.com/questions/53405020/configure-loglevel-of-azure-function-using-environment-variables
+ https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=aspnetcore-2.2

***

## Q&A

***

## Sources

* You can find this talk on my github https://github.com/kstastny/Talks

* Develop in F# using VS Code
    * [Precompiled F#](https://discardchanges.com/post/building-azure-functions-with-precompiled-fsharp/)
    * [F# Scripts](https://discardchanges.com/post/building-azure-functions-with-fsharp-and-vscode/1-setup/)

* TODO - Gojko, MS documentation etc.