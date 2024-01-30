- title : Git Secret
- description : 
- author : Karel Šťastný
- theme : night 
- transition : none

***


# Lunchtime Sweets with Hot Chocolate

***

<img src="./images/ai-powered.png" />

' ![](images/ai-powered.png)

***

## Agenda

<ul class="too-much-text">

<li> <strong>Introduction to GraphQL</strong>: Explain what GraphQL is, its benefits over traditional REST APIs, such as efficient data retrieval and flexibility for clients. </li>
<li> <strong>Basic Concepts</strong>: Discuss the fundamental concepts of GraphQL like queries, mutations, and subscriptions. Explain how these concepts work in GraphQL. </li>
<li> <strong>Introduction to HotChocolate</strong>: Introduce HotChocolate as a .NET platform for building GraphQL servers. Highlight its compatibility and ease of use within the .NET ecosystem. </li>
<li> <strong>Setting Up HotChocolate</strong>: Briefly explain how to set up a GraphQL server using HotChocolate in a .NET environment. Include basics like adding NuGet packages and configuring the startup class. </li>
<li> <strong>Defining Schemas in HotChocolate</strong>: Discuss how to define GraphQL schemas in HotChocolate. This might include how to create types, queries, and mutations. </li>
<li> <strong>Data Fetching and Resolvers</strong>: Explain how data fetching works in HotChocolate and how to write resolvers, which are key for fetching the data for each field in the schema. </li>
<li> <strong>Middleware and Customization</strong>: Touch on the middleware architecture in HotChocolate and how it can be used to customize request handling, error handling, and other server-side operations. </li>
<li> <strong>Advanced Features</strong>: If time allows, delve into advanced features of HotChocolate like subscription support, filtering, sorting, and integration with databases. </li>
<li> <strong>Real-world Examples</strong>: Provide a simple yet real-world example of implementing a GraphQL API using HotChocolate. This can help the audience understand practical applications. </li>
<li> <strong>Comparisons and Use Cases</strong>: Discuss where GraphQL and specifically HotChocolate shines in comparison to other technologies. Provide insights into ideal use cases and scenarios. </li>
<li> <strong>Best Practices</strong>: Share some best practices for designing and implementing GraphQL APIs with HotChocolate, such as schema design, versioning, and security considerations. </li>
<li> <strong>Resources and Community</strong>: Provide resources for further learning and information about the HotChocolate community for support and contributions. </li>

</ul>

***

## Let's start again...

- GraphQL
    - Schemas
    - Modes of API Communication 
- Examples (HotChocolate)

***

## GraphQL

- Query Language for API
- Strongly typed

' history, why it was created - ask for what you need

***

## GraphQL Service

- Schema (SDL)
    - Types
    - Operations on said types
- Endpoints implementing said Schema    

***

## Schema Example

```
schema {
  query: RootQuery
  mutation: RootMutation
}

type VehicleOutput {
  rootDriver: DriverOutput
  id: UUID!
  registrationPlate: String
  label: String
  rootDriverId: UUID
}

type RootQuery {
  vehicles: [VehicleOutput!]!
}

type RootMutation {
  updateLabel(vehicleInput: VehicleInput): VehicleOutput
}

```

***

## How to talk with API

- Query
- Mutation
- Subscription

***

## Schema Definition in HotChocolate

- Code-first
- Schema-first 
- Annotation Based

' will show Annotation based examples later. Annotation is easier, but slightly less powerful. 
' approaches can be combined

***

## Examples in HotChocolate

<img src="./images/vehicle-tracking.png" />

' show how it works from outside - real schema of our app, how to test and do a simple call

***

### Queries

<img src="./images/query2.png"width="540" height="540" />

' how to query data, how to go deeper - first just list of vehicles, compare with and without position. Then drivers and multiload problem
' advantages - predictable, FE asks for exactly what it wants, BE only loads needed data and does not need to return hard to reach things. flexibility
' DataLoaders - show on drivers, two approaches to get data
' sample - how how to return driver? imo there won't be time for that

***

### Mutations

<img src="./images/mutation2.png"width="540" height="540" />

' skip, just mention what it is and that the samples are there

***

### Subscriptions

<img src="./images/subscription2.png"width="540" height="540" />

***

## Advanced topics

- Distributed Schemas
- Middleware


' GQL pipeline constructed of many middlewares

***

### Distributed Schemas

* Schema Stitching
* Schema Federation

' stitching - merge multiple schemas into one 
' federation - combine multiple GQL services into one. More modular and maintainable, suitable for microservices architecture

***

### Middleware

```
builder
    .UseRequest<RequestLoggingMiddleware>()
    .UseRequest<ValidationErrorReportingMiddleware>()
    .UseInstrumentations()
    .UseExceptions()
    .UseTimeout()
    .UseDocumentCache()
    .UseDocumentParser()
    .UseDocumentValidation()
    .UseOperationCache()
    .UseOperationComplexityAnalyzer()
    .UseOperationResolver()
    .UseOperationVariableCoercion()
    .UseOperationExecution()
```

***


### The Good

<img src="./images/The-Good.png"width="540" height="540" />

' The Good
' GQL Implicit validation, strong types, certainty about what do you get, Schema, efficiency
' efficient for BE and FE

***

### The Bad

<img src="./images/The-Bad.png"width="540" height="540" />

' The Bad
' slightly harder than REST (i.e. needs libraries)
' almost one man show

***

### The Ugly

<img src="./images/The-Ugly2.png" width="540" height="540" />

' The Ugly 

' documentation is mostly videos
' breaking changes - authentication endpoint
' sometimes naming clashes with C#

***

## Q&A

***

## Sources

Slides at https://github.com/kstastny/Talks/tree/master/code-for-lunch-graphql/code-for-lunch-graphql

* https://graphql.org/learn/
* https://chillicream.com/docs/hotchocolate/