<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	// https://github.com/microsoft/ApplicationInsights-dotnet/blob/develop/BASE/src/Microsoft.ApplicationInsights/Channel/TelemetryBuffer.cs
	// https://github.com/microsoft/ApplicationInsights-dotnet/blob/develop/BASE/src/Microsoft.ApplicationInsights/Channel/InMemoryChannel.cs
	// https://github.com/microsoft/ApplicationInsights-dotnet/blob/develop/BASE/src/Microsoft.ApplicationInsights/Channel/InMemoryTransmitter.cs
	// https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger
	// https://docs.microsoft.com/en-us/dotnet/standard/collections/thread-safe/when-to-use-a-thread-safe-collection
	// Why not just use a default thread safe collection?
	// It seems that the default implementation of a queue was not used so that more control could be had around 
	// how many items (MAX) can be a part of the buffer, as well as to potentially speed up the Dequeu process (as
	// Telemetry buffer provides a copy of the entire list at Dequeue and then clears itself, instead of dequeuing one by one.
	// I'm curious about the performance implications of this 🤔
	
	var queueBoy3000 = new QueuBoy3000<int>();		
	
	var totalItemsToDealWith = 1000000;
	
	var random = new Random();

	var taskList = new List<Task>();
	for (int i = 0; i < totalItemsToDealWith; i++)
	{
		taskList.Add(AddToQueue(i, queueBoy3000, random));		
	}
	
	taskList.Add(DequeueUntilCount(totalItemsToDealWith, queueBoy3000));
	
	await Task.WhenAll(taskList);
	
	queueBoy3000.Dump();
}


Task DequeueUntilCount<T>(int count, QueuBoy3000<T> queueBoy3000)
{
	var currentCount = 0;

	while (currentCount < count)
	{
		while(queueBoy3000.TryDequeue(out var item))
		{
			currentCount++;
		}
	}
	
	return Task.CompletedTask;
}

Task AddToQueue<T>(T i, QueuBoy3000<T> queueBoy3000, Random random)
{
	Task.Delay(random.Next(100, 10000));
	
	queueBoy3000.Enqueue(i);
	
	return Task.CompletedTask;
}



// Define other methods, classes and namespaces here
public sealed class QueuBoy3000<T>
{
	private ConcurrentQueue<T> SomeQueue { get; }	= new ConcurrentQueue<T>();
	
	public void Enqueue(T item)
	{
		SomeQueue.Enqueue(item);				
	}
	
	public bool TryDequeue(out T item)
	{
		return SomeQueue.TryDequeue(out item);
	}
	
	public long Count => SomeQueue.Count;
}