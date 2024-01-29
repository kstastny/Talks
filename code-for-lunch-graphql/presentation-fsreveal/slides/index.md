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
- Examples

***

## GraphQL

- Query Language for API
- Strongly typed

' TODO mention advantages - predictable, FE asks for exactly what it wants, BE only loads needed data and does not need to return hard to reach things. flexibility

***

## GraphQL Service

- Schema
    - Types
    - Operations on said types
- Endpoints implementing said Schema    

***

## Schema Example

```
schema {
  query: RootQuery
  mutation: RootMutation
  subscription: RootSubscription
}

type RootQuery {
  vehicles: [VehicleOutput!]!
}

type VehicleOutput {
  rootDriverNaive: DriverOutput
  rootDriverDataLoader: DriverOutput
  id: UUID!
  registrationPlate: String
  label: String
  rootDriverId: UUID
}
```

***

## Schema Definition in HotChocolate

- Code-first
- Schema-first 
- Annotation Based

' will show Annotation based examples later. Annotation is easier, but slightly less powerful. 
' approaches can be combined

***

## How to talk with API

- Query
- Mutation
- Subscription

***

## Examples in HotChocolate

<img src="./images/vehicle-tracking.png" />

***

### Queries

<img src="./images/query2.png"width="540" height="540" />

' how to query data
' DataLoaders
' DataLoaders- show on drivers, two approaches

***

### Mutations

<img src="./images/mutation2.png"width="540" height="540" />

' likely skip, just mention what it is and that the samples are there

***

### Subscriptions

<img src="./images/subscription2.png"width="540" height="540" />

' TODO different approaches to subscription definition

***

## Advanced topics

- Authentication
- Middleware
- Distributed Schemas

' - Error Handling //TODO this may be actually presented in the samples? if theres time
' TODO study and prepare what to say

***

### The Good

<img src="./images/The-Good.png"width="540" height="540" />

' The Good
' GQL Implicit validation, strong types, certainty about what do you get, Schema

***

### The Bad

<img src="./images/The-Bad.png"width="540" height="540" />

' The Bad
' slightly harder than REST (i.e. needs libraries), TODO others for HotChocolate

***

### The Ugly

<img src="./images/The-Ugly2.png" width="540" height="540" />

' The Ugly 

' sometimes naming, breaking changes, more or less one man show?, TODO for HotChocolate

***

## Sources

Slides at https://github.com/kstastny/Talks

* https://graphql.org/learn/
* https://chillicream.com/docs/hotchocolate/