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

***

### Advantages

* robust concurrent applications
* easy parallelization
* no need for locks
* explicit control
* throttling



***

## TPL Dataflow Pipeline

  
![](images/tpl-workflow.png)


<p class="reference"><a href="https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow">https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow</a></p>  


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

![](images/block-link.png)

`source.LinkTo(target)`

' PropagateCompletion, Append, MaxMessages

***

### **DEMO** Simple Pipeline

> BasicPipeline

![](images/tpl-dataflow-basic-pipeline.png)

***

## Block Completion

* pipeline does not need to be eternal
* `Complete` method
* `Completion` Task

***

## Error Handling

* Faulted Blocks
* Error Propagation

' see https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow

***

### Error Propagation

![](images/tpl-workflow.png)

<p class="reference"><a href="https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow">https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow</a></p>  

***

### **DEMO** Completion and Error Handling

> ParallelBatchProcessing

***

### TransformBlock

![](images/block-transformblock.png)

' https://www.blinkingcaret.com/2019/06/05/tpl-dataflow-in-net-core-in-depth-part-2/
' https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow + docs

***

### TransformManyBlock

![](images/block-transformmanyblock.png)

***

### BroadcastBlock

![](images/block-broadcastblock.png)

***


### JoinBlock

![](images/block-joinblock.png)



***

### **DEMO** Block Types

> BlockSamples

![](images/tpl-dataflow-block-samples-demo.png)

***

### Back-Pressure and Load Shedding

' `BroadcastBlock` vs `TransformManyBlock`
' https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow

***

### **DEMO** Back-Pressure and Load Shedding

> BackPressureLoadShedding


***

### Example - Processing Pipelines

![](images/jack-vanlighty.png)

* https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow ([Source Code](https://github.com/Vanlightly/StreamProcessingSeries/tree/master/src/net-apps/InProcStreamProcessing.TplDataflow))


***


### Example - Processing a Large Stream of data

![](images/ricardo-terrell.png)

* https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow ([Source Code](https://github.com/dotnetcurry/TPL-Dataflow))


***

## Sources

* You can find this talk on my github https://github.com/kstastny/Talks
* [Ricardo Terrell](https://www.dotnetcurry.com/patterns-practices/1483/parallel-workflow-dotnet-tpl-dataflow)
* [Jack Vanlighty: Processing Pipelines Series](https://jack-vanlightly.com/blog/2018/4/18/processing-pipelines-series-tpl-dataflow)
* [Dataflow documentation](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library)

***

## Q&A
