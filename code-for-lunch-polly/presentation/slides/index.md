- title : Git Secret
- description : 
- author : Karel Šťastný
- theme : night 
- transition : none

***


# Polly wants a cracker

***

TODO image of parrot
<img src="./images/ai-powered.png" />

' ![](images/ai-powered.png)

***


## Resiliency

- Ability to recover from failures and continue to function
- Avoid downtime and data loss

' https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/

***

## Resiliency II

- Context: communicating with "outside world"
- Problems
  - Transient
  - Nontransient

' outside world - 3rd party applications, APIs, but also database, message bus etc.

***

## TODO fallacies

https://en.wikipedia.org/wiki/Fallacies_of_distributed_computing


***

## Polly library

- set of tools and strategies for handling transient problems
- we can "just" use it

' tool in a general sense
' TODO describe what it is, no need to develop (hard to do correctly), we can choose correct behavior

***

## Strategies

- Reactive
  - Retry
  - Circuit-breaker
  - Fallback*
  - Hedging*
- Proactive
  - Timeout
  - Rate-limiter

' I did not use the starred ones in production

***

### Retry

TODO drawing, image

TODO probably when not and when to use should be slides
' when *not* to use it - if we are not the end system, let the final user decide, otherwise calls may be too slow. Demonstrate with image. Also use sparingly when facing user (we do not want people to keep waiting)
  TODO test combination - Retry plus timeout. fail after 5 seconds, second goes through - can we still timeout in time? (should work)
' when do we use it - background processes, we want the call to succeed. Still, it may be useful to use different mechanism than just in-memory Polly, if the errors are more persistent

***

### Circuit-breaker

TODO drawing, image (electrical circuit)

' my favourite :) 
' if something is failing, let it rest and only occasionaly probe. Better to fail fast then let thousands of calls timeout after 20-30 seconds

***

### Fallback

TODO short explanation

***

### Hedging

TODO short explanation

***

### Timeout

TODO short explanation

' this is pretty clear imo, just mention when it could be used and that this is useful to have common place to handle slow response, irrelevant of the underlying protocol

***

### Rate-limiter

TODO drawing, explanation

' what does it do
' describe the pitfalls, sample - partitioned rate limiter

***

## Advanced Usages

- Chaos Engineering
- Telemetry
- Resilience Context

***

TODO this time probably skip this format
 - TODO just what to be aware of - watch out for multiple instances of app, also Polly is only in memory, do not be so sure everything will go through (e.g. our app may also fail)


****

### The Good


***

### The Bad


***

### The Ugly



***

## Q&A

***

## Sources

Slides at https://github.com/kstastny/Talks/tree/master/code-for-lunch-polly/code-for-lunch-polly

* https://github.com/App-vNext/Polly
* https://en.wikipedia.org/wiki/Fallacies_of_distributed_computing