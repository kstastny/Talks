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

# Agenda

- **Introduction to GraphQL**: Explain what GraphQL is, its benefits over traditional REST APIs, such as efficient data retrieval and flexibility for clients.
- **Basic Concepts**: Discuss the fundamental concepts of GraphQL like queries, mutations, and subscriptions. Explain how these concepts work in GraphQL.
- **Introduction to HotChocolate**: Introduce HotChocolate as a .NET platform for building GraphQL servers. Highlight its compatibility and ease of use within the .NET ecosystem.

---

- **Setting Up HotChocolate**: Briefly explain how to set up a GraphQL server using HotChocolate in a .NET environment. Include basics like adding NuGet packages and configuring the startup class.
- **Defining Schemas in HotChocolate**: Discuss how to define GraphQL schemas in HotChocolate. This might include how to create types, queries, and mutations.
- **Data Fetching and Resolvers**: Explain how data fetching works in HotChocolate and how to write resolvers, which are key for fetching the data for each field in the schema.

---

- **Middleware and Customization**: Touch on the middleware architecture in HotChocolate and how it can be used to customize request handling, error handling, and other server-side operations.
- **Advanced Features**: If time allows, delve into advanced features of HotChocolate like subscription support, filtering, sorting, and integration with databases.
- **Real-world Examples**: Provide a simple yet real-world example of implementing a GraphQL API using HotChocolate. This can help the audience understand practical applications.

---

- **Comparisons and Use Cases**: Discuss where GraphQL and specifically HotChocolate shines in comparison to other technologies. Provide insights into ideal use cases and scenarios.
- **Best Practices**: Share some best practices for designing and implementing GraphQL APIs with HotChocolate, such as schema design, versioning, and security considerations.
- **Resources and Community**: Provide resources for further learning and information about the HotChocolate community for support and contributions.

***

### Let's start again...

- GraphQL
    - Schemas
    - Modes of API Communication 
- Examples

***

## GraphQL

- Query Language for API
- strongly typed

' TODO mention advantages - predictable, FE asks for exactly what it wants, BE only loads needed data and does not need to return hard to reach things. flexibility

***

### GraphQL Service

- schema
    - types
    - operations on said types

***

### Schema Example

TODO sample from my app - Vehicles

***

###

TODO alternatives for defining schema (Code first vs schema first vs Annotations)

***

### How to talk with API

- Query
- Mutation
- Subscription

***

### Examples in .NET

TODO query, mutation, subscription

***

TODO dataloaders - solve the issue of multiple loads
  - show on drivers, two approaches

***

TODO maaaabe skip mutations to save time and show subscriptions, that is more interesting. Only have a sample mutation for changing vehicle label


***

TODO ADVANCED (if time, probably not)
 - Authentication, Middlewares, error handling

***

## The Good



***

## The Bad



***

## The Ugly



***

## Sources

Slides at https://github.com/kstastny/Talks

* https://graphql.org/learn/
* https://chillicream.com/docs/hotchocolate/