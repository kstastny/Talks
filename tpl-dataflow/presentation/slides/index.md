- title : TPL Dataflow
- description : 
- author : Karel Šťastný
- theme : night 
- transition : none

***

# TPL Dataflow

## Karel Šťastný

***

## TPL Dataflow

* dataflow and pipelining tasks
* actor-based
* asynchronous
* explicit control

***

## TPL Dataflow

* increase robustness of concurrency-enabled applications
* enables effective techniques for running embarrassingly parallel problems
* built-in support for throttling and asynchrony
* explicit control over how data is buffered and moves around the system
* obviates the need for synchronization locks 


' explain embarrassingly parallel
' https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow 
' https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library


***

## TPL Dataflow Pipeline

  
![](images/tpl-workflow.png)


<p class="reference"><a href="https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow">https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow</a></p>  


' TODO my own image of pipeline (just multiple boxes connected by lines) ?
' Blocks compose pipelines and networks
' Composable - big strength, blocks can be reordered, removed, added, even during runtime

***

## Dataflow Blocks

* Structures that buffer and process data

![](images/tpl-dataflow-block-anatomy.png)

' consume, produce, propagate/transform data
' data is buffered


***

## Block Types

* Source
* Target
* Propagator

***

## Linking Blocks

* Blocks are connected by "linking" them together

`source.LinkTo(target)`

' PropagateCompletion, Append, MaxMessages

***

### **DEMO** Simple Pipeline

> BasicPipeline

***

## Block Completion

* blocks should not expect any more messages
* allows a block to finish processing messages

***

## Error Handling

* Faulted Blocks
* Error Propagation

' see https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow

***

### **DEMO** Completion and Error Handling

> ParallelBatchProcessing

***

## Block examples

* `TransformBlock`
* `TransformManyBlock`
* `BroadcastBlock`
* `BatchBlock`
* `JoinBlock`

TODO prepare explanation and examples

' TODO https://www.blinkingcaret.com/2019/06/05/tpl-dataflow-in-net-core-in-depth-part-2/
' https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow + docs

***

## Message propagation

* multiple targets - pub/sub, many blocks processing one message
* multiple sources - different sources, message aggregation
* target - consume, postpone or decline message

TODO example - pub/sub
TODO example - aggregation 
TODO example - postpone or decline message - maybe remove this?

***

## Back-Pressure and Load Shedding

* `BroadcastBlock` vs `TransformManyBlock`

' https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow

***

### **DEMO** Back-Pressure and Load Shedding

> BackPressureLoadShedding


***

## "Case Study" 

TODO maybe different slide name

* https://github.com/Vanlightly/StreamProcessingSeries

TODO example from Riccardo Terrell, if I use it
    - probably a nice example of pipeline, read https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow and study the code
TODO alternative - Jack Vanlighty

***

## Sources

* You can find this talk on my github https://github.com/kstastny/Talks
* https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow
* https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow

***

## Q&A
