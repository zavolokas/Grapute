# Grapute
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?style=flat-square)]()

The library helps to define a graph of computations or a pipeline by connecting computation `Nodes` to eachother, where an output of one node becomes an input for the next `Node`.

`Node` always returns an array of results. Later these results can be either consumed one by one by another `Node` or all together by a `SinkNode`.

You can define your processing `Node` by inheriting from `Node` class and implementing process method. Another option is to provide your own process `Func` to `FuncNode`.

You can find a sample code that demonstrates how to implement a simple MapReduce algorithm using Grapute.

## Example

```csharp
var produceThreeOutputsNode = new FuncNode<int, int>(x =>
    {
        //
        return new[] { x , x + 1, x + 2 };
    });

var pipeline = produceThreeOutputsNode
    .ForEachOutput(x =>
        {
            // As an output we have a doubled input
            return new []{x, x};
        })
    .ForEachOutput(x =>
        {
            // Increase the input
            return new []{x + 1};
        })
    .CollectAllOutputsToOneArray()
    .ForArray(all =>
        {
            // Split the input array and convert to a String
            var p1 = all
                .Take(all.Length / 2)
                .Aggregate((a ,b) => $"{a}{b}");

            var p2 = all
                .Skip(all.Length / 2)
                .Aggregate((a ,b) => $"{a}{b}");

            return new []{p1, p2};
        });

produceThreeOutputsNode.SetInput(5);
var result = pipeline.Process().Output;
```
The code above represents the following graph of computations:

![graph]

[graph]: images/graph.png "Graph of computations"