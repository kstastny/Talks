- title : Azure Functions
- description : Overview of Azure Functions for the noninitiated
- author : Karel Šťastný
- theme : night 
- transition : none

***

# Azure Functions



***

## Outline

TODO create slides for stuff below

* # Azure Functions - what are they
* -- Serverless definition, pros and cons - incuded above
* # when to use functions (glue or main app)
* TODO Functions - how to create (demo, then show prerequisities - links included)
* TODO Function App
* TODO Functions 
    - # function runtime versions, supported languages
    - TODO Triggers, Input and Output Bindings
            TODO demo - function triggered from queue, saves to blob or something like that 
            use https://hub.docker.com/r/microsoft/azure-storage-emulator/ or Azurite (Emulator je kompletnější a na SQL Express i běží zdá se)
            or BlobTrigger - if there's new file, process it - parse and extract contents
              NOTE: BlobTrigger procesuje i bloby, které jsou uložené, ale function host je neviděl!

            TODO demo https://simonholman.blog/azure-functions-with-imperative-bindings/
    - TODO settings    
    - TODO authentication
    - TODO deployment
    - TODO monitoring
* TODO DEMO - whole application - neme-import, how it can be structured, how does it work (tailored for demo)    
        TODO show recommended application structure (later, with whole demo) - functions are just thin layer that will call business logic, same as if we used Web API or Giraffe or whatever
* TODO PREPARE WHAT TO SAY EXACTLY

-- what is serverless, ideas behind it, pros, cons
TODO supported languages, function runtimes
TODO why should we use them, pros, cons of functions
TODO main usecases - glue or application itself. will talk in context of glue. replaces WebJobs, much easier
    TODO examples (from my experience)
            - automatic log cleaning (DB and files from app service) 
            - automatic restart of app service on alarm
    TODO other examples - talk to Landy, Roman        
TODO triggers, inputs, outputs
TODO versions v1 .NET Framework, v2 .NET Core
TODO Azure Portal - good for exploration, also show

TODO Azure Functions cons
    * various DLL incompatibilities and breaking changes, e.g. //needs Microsoft.WindowsAzure.Storage.dll 9.3.1, workaround here https://github.com/Azure/azure-functions-host/issues/3784.

***

# Azure Functions

* Serverless computing service on Azure
* Function as a Service 
* Stateless
* Event-Driven
 

' FaaS - run code without managing own server/container or own long lived application
' no worry about infrastructure - just write function, define how to run it and run. No need for complex deployment process
' stateless - Durable Functions are stateful but I will not talk about them


***

## Benefits

* no need to maintain infrastructure
* eliminated boilerplate
* rapid and simple development
* lower cost
* automated scaling
* all the power of Azure Web Apps

' pricing - you pay only when the function runs (pay as you go). optionally also dedicated plan. Premium plan in preview, see https://docs.microsoft.com/en-us/azure/azure-functions/functions-scale
' Azure Web Apps - Functions based on WebJobs. Kudu, CI etc. available
' autoscaling - you cannot over or under provision

## Disadvantages

* vendor lock-in
* performance - extra latency when spinned down (TODO TEST IF ITS TRUE)
* monitoring and debugging resource consumption ?
* not for everyone

' performance -
' TODO other cons? https://en.wikipedia.org/wiki/Serverless_computing

***
TODO remove these notes
    * Azure Functions - Functions as a Service, each has Trigger, Input and Output Bindings (Blob Storage, CosmosDb, SendGrid)
            * one function can have multiple bindings, `in` or `out` parameters
            TODO think of example, https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings
            e.g. Mark Heath - function is HTTP Triggered but using binding reads data from Table Storage

* Durable Functions? do not include

***

## Use Cases
 
* Glue
* Full backend
* Automating processes
* Scheduled jobs
* Dynamic workloads


' glue - simple to develop, has to be small and flexible
' Automating processes - e.g. webhook for automatic restart on alarm
' Dynamic workloads - because of autoscaling


***

### **DEMO** Create Function (F#)

TODO explain - possible in portal but good just for demo purposes, therefore we won't be doing that
TODO just simple hello world

TODO only F#. On Smart, just show Rider for F#, show CLI for C#?
on FSharping show alternatives and their problems (VS Code, CLI)

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


***

### Function Bindings

* Allow connecting function to another resource
* optional
* function may have multiple bindings
* `input`, `output` or both
* abstracts access to other services

***

### Function Bindings

* Blob storage
* Cosmos DB
* SignalR
* Table Storage

***

### Function App

* holds related Azure Functions


' you can think of it as your "app service" or "web" that has the functionality
' show in Azure Portal

***


### **DEMO** Function Settings

* Configurable the same way as Azure App Service
    * e.g. `local.settings.json`
    * Azure portal

TODO `local.settings.json` how to copy during build? and not include
  include, commit sample and delete on publish

' TODO show in nemestats-import application or some other demo?
TODO explain how to configure locally, how to configure when deployed, how to access configuration

***

### Authentication



TODO show authentication modes and options (header, `x-functions-key`)
    TODO discuss - mostly suitable for development, should implement better auth (where was this written? meaning OAuth and such?)

TODO test that it works as documented (might not, see https://markheath.net/post/managing-azure-function-keys), let them know
    * `system` requires master key


***

### Deployment

TODO discuss deployment options
    * from Azure side - can take from github etc.
    * Azure DevOps - deploy as normal AppService
    * from CLI - demo, not feasible for real projects
TODO pricing modes - under existing app service, or consumption based    

*** 
### Monitoring

TODO how to monitor, where are the logs, where are the launches, Function console
TODO what if they are not visible?

***

## Q&A

***

## Sources

* You can find this talk on my github https://github.com/kstastny/Talks

* TODO - Gojko, MS documentation etc.