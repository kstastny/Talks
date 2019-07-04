- title : Azure Functions
- description : Overview of Azure Functions for the noninitiated
- author : Karel Šťastný
- theme : night 
- transition : none

***

# Azure Functions


***

## Outline

* What are Azure Functions
    * "Serverless"

*** 

### Azure Functions

TODO what is serverless, ideas behind it, pros, cons
TODO supported languages, function runtimes
TODO why should we use them, pros, cons of functions
TODO main usecases - glue or application itself. will talk in context of glue. replaces WebJobs, much easier
    TODO examples (from my experience)
            - automatic log cleaning (DB and files from app service) 
            - automatic restart of app service on alarm
    TODO other examples - talk to Landy, Roman        
TODO triggers, inputs, outputs

***

### **DEMO** Create Function (F#)

TODO explain - possible in portal but good just for demo purposes, therefore we won't be doing that

TODO only F#. On Smart, just show Rider for F#, show CLI for C#?
on FSharping show alternatives and their problems (VS Code, CLI)
TODO show recommended application structure - functions are just thin layer that will call business logic, same as if we used Web API or Giraffe or whatever

***

### **DEMO** Function Settings

TODO explain how to configure locally, how to configure when deployed, how to access configuration

***

### Authentication

TODO show authentication modes and options (header, `x-functions-key`)
    TODO discuss - mostly suitable for development, should implement better auth (where was this written? meaning OAuth and such?)

TODO test that it works as documented (might not, see https://markheath.net/post/managing-azure-function-keys), let them know


***

### Deployment

TODO discuss deployment options
    * from Azure side - can take from github etc.
    * Azure DevOps - deploy as normal AppService
    * from CLI - demo, not feasible for real projects
TODO pricing modes - under existing app service, or separately    

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