- title : Decorator Pattern
- description : Decorator Pattern Description and usage
- author : Karel Šťastný
- theme : moon
- transition : default


***

### Decorator Pattern

Karel Šťastný <br />
Radium s.r.o.

***

### Example

* What is the problem we're trying to solve?

' - show example, then we'll talk theory. 
' - Data Access with auditing, permission checks, data modification
' - extend class functionality without modifying it - essentially inheritance
' - show incorrect solution, where it leads (class explosion or multiple responsibilities for class) and how to fix it
' - I chose this because it's more related to real life

***

### Decorator

* Dynamically adds responsibility by wrapping original code. We do not modify original code
* real usage: I/O Streams

' We don't want to modify the code because it might not be accessible or the behavior is needed only sometimes
' Most useful when we need to combine multiple different responsibilities
' Wrapper - alternative name, sometimes used

***

### Decorator - Implementation

![](images/decorator_pattern_wiki.png)

' image shows how to correctly implement it (read top "Component" as interface)
' the base class is necessary if we need multiple decorators

***

### How did this help us?

* Prevent "class explosion"
* Enables Single Responsibility Principle
* Usage is much more flexible

' Class explosion - if we were to use inheritance, we would need to create classes for every combination of functionality or keep everything in base and switch by flags (bleh)
' AuditingDataAccess, AuditingPermissionCheckingDataAccess, AuditingPermissionCheckingCachingDataAccess...
' Single Responsibility Principle - every module or class should have responsibility over a single part of functionality 
' Flexible - we can combine instances in different ways

***

### Where to go next?

* What if I have 100s of classes where I need to add Logging, caching?
* AOP

' logging, caching - cross cutting concerns, all very similar boilerplate

***

### Q&A

***

### Sources

* https://en.wikipedia.org/wiki/Decorator_pattern
* http://www.dictionary.com/browse/decorate



***