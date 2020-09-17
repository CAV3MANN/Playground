<Query Kind="Program">
  <NuGetReference>Azure.Messaging.EventHubs</NuGetReference>
  <Namespace>Azure.Messaging.EventHubs.Producer</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Azure.Messaging.EventHubs</Namespace>
</Query>

async Task Main()
{
	// Requires usage of C# 8, can work with .Net Core SDK 2.1.X as long as this is specified though...
	// https://docs.microsoft.com/en-us/dotnet/api/overview/azure/messaging.eventhubs-readme?view=azure-dotnet
	var eventHubProducerClient = new EventHubProducerClient("Endpoint=sb://<FQDN>/;SharedAccessKeyName=<KeyName>;SharedAccessKey=<KeyValue>");

	var peekingCollection = new PekingCollection<string>(1_000_000);

	var initialDelayInMilliseconds = 5000;
	var minBackoff = 0;
	var maxBackoff = 7;
	var currentBackoff = minBackoff;
	while (true)
	{					
		var currentCountOfItemsInCollection = peekingCollection.Count;

		if (currentCountOfItemsInCollection > 0)
		{
			if (currentBackoff < maxBackoff)
			{
				currentBackoff++;
			}

			await Task.Delay(CalculateDelay(initialDelayInMilliseconds, currentBackoff));

			// Exit loop early because we can't do anything anyways.
			continue;
		}
		else
		{
			currentBackoff = minBackoff;
		}
		
		using var eventBatch = await eventHubProducerClient.CreateBatchAsync(new CreateBatchOptions
		{		
			MaximumSizeInBytes = 20_000_000 // 20mb
		});		

		for (int i = 0; i < currentCountOfItemsInCollection; i++)
		{
			// Take until count reaches the currentItemsInCollection OR until
			// the eventBatch size is reached. This mitigates the issue of waiting forever
			// for the blocking collection to empty as items could be added to it as we are
			// taking from it.
			
			// We need some way to peek and then take :(
			peekingCollection.TryPeek(out var peekedItem);
			
			// If peekedItem is null, we have other issues...
			var encodedItem = Encoding.UTF8.GetBytes(peekedItem!);
			if(!eventBatch.TryAdd(new EventData(encodedItem)))
			{
				// Exit early if batch be full
				break;
			}
			
			// If this statement is reached, then the item was successfully added to the batch 
			// and can be popped off the blocking collection
			peekingCollection.TryPop();
		}
		
		// No cancelaltion token because we always want to complete
		await eventHubProducerClient.SendAsync(eventBatch);
	}
}

// Define other methods, classes and namespaces here
private static int CalculateDelay(int initialDelayInMilliseconds, int currentBackoff)
{	
	return (int)(initialDelayInMilliseconds * Math.Pow(2, currentBackoff) - 4000);
}

public sealed class PekingCollection<T> where T : class // (Not to be confused with Peking Chicken, should be read as peeking collection)
{
	// lol and here I thought I may have been clever, what with trying to copy App Insights and all
	// https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff963548(v=pandp.10)?redirectedfrom=MSDN
	// I'm going to be slightly annoyed if somehow it ends up being best to use a list
	private readonly ConcurrentQueue<T> forPeekingOnly;
	private readonly BlockingCollection<T> blockingCollection;

	public PekingCollection(int capacity)
	{
		forPeekingOnly = new ConcurrentQueue<T>();
		blockingCollection = new BlockingCollection<T>(forPeekingOnly, capacity);
	}
	
	public bool TryPeek(out T? item)
	{
		return forPeekingOnly.TryPeek(out item);			
	}
	
	public bool TryPop()
	{
		return blockingCollection.TryTake(out _);
	}
	
	public int Count => blockingCollection.Count;
}
