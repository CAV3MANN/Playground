<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Logging.Abstractions</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#nullable enable

void Main()
{
	// ILogDetailProcessor implementations will take log details, put them in a standardized message, and place them on
	// some queue for further processing.
	ILogDetailProcessor logDetailProcessor = GetLogDetailProcessor();
	SpecificLogDetailBuilder specificLogDetailBuilder = new SpecificLogDetailBuilder();

	specificLogDetailBuilder.AddSpecificDetail("Detail");
	specificLogDetailBuilder.SetAnotherSpecificDetail("Another Detail");

	var completedLogDetail = specificLogDetailBuilder.Complete();		
	
	logDetailProcessor.Process(completedLogDetail);	
}

ILogDetailProcessor GetLogDetailProcessor()
{
	var applicationDetailRetriever = new ApplicationDetailRetriever();
	var deviceDetailRetriever = new DeviceDetailRetriever();
	var processDetailRetriever = new ProcessDetailRetriever();
	IStandardizedLogMessageBuilder logMessageBuilder = new StandardizedLogBuilder(applicationDetailRetriever, deviceDetailRetriever, processDetailRetriever);

	ILogMessageQueuer logMessageQueuer = new LogMessageQueueController();
	LogMessageQueueProcess logMessageQueueProcess = new LogMessageQueueProcess(logMessageQueuer);
	// In case there are other locations log messages should be sent to and/or processed by
	ILogMessageProcess logMessageProcessComposite = new LogMessageProcessComposite(new[] { logMessageQueueProcess });
	return new StandardLogDetailProcessor(logMessageBuilder, logMessageProcessComposite);
}

// Define other methods, classes and namespaces here
public class ApplicationDetail
{
}

public interface IApplicationDetailRetriever<T> where T : ApplicationDetail
{
	T Detail { get; }
}

public sealed class ApplicationDetailRetriever : IApplicationDetailRetriever<ApplicationDetail>
{
	public ApplicationDetail Detail { get;} = new ApplicationDetail();
}

public class DeviceDetail
{
}

public interface IDeviceDetailRetriever<T> where T : DeviceDetail
{
	T Detail { get; }
}

public sealed class DeviceDetailRetriever : IDeviceDetailRetriever<DeviceDetail>
{
	public DeviceDetail Detail { get;} = new DeviceDetail();
}

public class ProcessDetail
{
}

public interface IProcessDetailRetriever<T> where T : ProcessDetail
{
	T Retrieve();
}

public sealed class ProcessDetailRetriever : IProcessDetailRetriever<ProcessDetail>
{
	public ProcessDetail Retrieve()
	{
		return new ProcessDetail();
	}
}

public class StandardizedLogMessage
{
	public ApplicationDetail ApplicationDetail { get; }
	public DeviceDetail DeviceDetail { get; }
	public ProcessDetail ProcessDetail { get; }
	public object Detail { get; }
	public string DetailType { get; }

	public StandardizedLogMessage(ApplicationDetail applicationDetail,
		DeviceDetail deviceDetail,
		ProcessDetail processDetail,
		object detail,
		string detailType)
	{
		this.ApplicationDetail = applicationDetail;
		this.DeviceDetail = deviceDetail;
		this.ProcessDetail = processDetail;
		this.Detail = detail;
		this.DetailType = detailType;
	}
}

public sealed class StandardizedLogMessage<T> : StandardizedLogMessage where T : ILogDetail
{
	public new ApplicationDetail ApplicationDetail { get; }
	public new DeviceDetail DeviceDetail { get; }
	public new ProcessDetail ProcessDetail { get; }
	public new T Detail { get; }
	public new string DetailType { get; }

	public StandardizedLogMessage(ApplicationDetail applicationDetail,
		DeviceDetail deviceDetail,
		ProcessDetail processDetail,
		T detail) : base(applicationDetail, deviceDetail, processDetail, detail, detail.LogDetailType)
	{
		this.ApplicationDetail = applicationDetail;
		this.DeviceDetail = deviceDetail;
		this.ProcessDetail = processDetail;
		this.Detail = detail;
		this.DetailType = detail.LogDetailType;
	}
}

public interface ILogDetailBuilder<T>
{
	T Complete();
}

public interface ILogDetail
{
	string LogDetailType { get; }
}

public class SpecificLogDetail : ILogDetail
{
	public string LogDetailType { get; } = "SpecificLogDetail";
}

public sealed class SpecificLogDetailBuilder : ILogDetailBuilder<SpecificLogDetail>
{
	public SpecificLogDetail Complete()
	{
		return new SpecificLogDetail();
	}

	internal void AddSpecificDetail(string v)
	{
		
	}

	internal void SetAnotherSpecificDetail(string v)
	{
		
	}
}

public interface IStandardLogBuilder<TApplication, TDevice, TProcess>
	where TApplication : ApplicationDetail
	where TDevice : DeviceDetail
	where TProcess : ProcessDetail
{
	StandardizedLogMessage GenerateStandardizedLogMessage<T>(T logDetail) where T : ILogDetail;
}

public interface IStandardizedLogMessageBuilder : IStandardLogBuilder<ApplicationDetail, DeviceDetail, ProcessDetail>
{
}

public sealed class StandardizedLogBuilder : IStandardizedLogMessageBuilder
{
	private readonly IApplicationDetailRetriever<ApplicationDetail> applicationDetailRetriever;
	private readonly IDeviceDetailRetriever<DeviceDetail> deviceDetailRetriever;
	private readonly IProcessDetailRetriever<ProcessDetail> processDetailRetiver;

	public StandardizedLogBuilder(IApplicationDetailRetriever<ApplicationDetail> applicationDetailRetriever,
		IDeviceDetailRetriever<DeviceDetail> deviceDetailRetriever,
		IProcessDetailRetriever<ProcessDetail> processDetailRetiver)
	{
		this.applicationDetailRetriever = applicationDetailRetriever;
		this.deviceDetailRetriever = deviceDetailRetriever;
		this.processDetailRetiver = processDetailRetiver;
	}

	private ApplicationDetail? applicationDetailOverride;
	private ApplicationDetail ApplicationDetail => applicationDetailOverride ?? applicationDetailRetriever.Detail;

	private DeviceDetail? deviceDetailOverride;
	private DeviceDetail DeviceDetail => deviceDetailOverride ?? deviceDetailRetriever.Detail;

	private ProcessDetail? processDetailOverride;
	private ProcessDetail GetProcessDetail() => processDetailOverride ?? processDetailRetiver.Retrieve();


	public void OverrideDefaultApplicationDetail(ApplicationDetail detail)
	{
		this.applicationDetailOverride = detail;
	}

	public void OverrideDefaultDeviceDetail(DeviceDetail deviceDetail)
	{
		this.deviceDetailOverride = deviceDetail;
	}

	public void OverrideDefaultProcessDetail(ProcessDetail processDetail)
	{
		this.processDetailOverride = processDetail;
	}

	public StandardizedLogMessage GenerateStandardizedLogMessage<T>(T logDetail) where T : ILogDetail
	{
		return new StandardizedLogMessage<T>(applicationDetailRetriever.Detail, deviceDetailRetriever.Detail, processDetailRetiver.Retrieve(), logDetail);
	}
}

public interface ILogDetailProcessor
{
	void Process<T>(T detail) where T : ILogDetail;
}

public sealed class StandardLogDetailProcessor : ILogDetailProcessor
{
	private IStandardizedLogMessageBuilder standardizedLogMessageBuilder;
	private ILogMessageProcess logMessageProcess;

	public StandardLogDetailProcessor(IStandardizedLogMessageBuilder standardizedLogMessageBuilder,
		ILogMessageProcess logMessageProcess)
	{
		this.standardizedLogMessageBuilder = standardizedLogMessageBuilder;
		this.logMessageProcess = logMessageProcess;
	}

	public void Process<T>(T detail) where T : ILogDetail
	{
		var standardLogMessage = standardizedLogMessageBuilder.GenerateStandardizedLogMessage(detail);		
		
		// Wanted to play around with Pattern Matching 😎
		//switch(standardLogMessage)
		//{
		//	case StandardizedLogMessage<SpecificLogDetail> c: c.Dump("Type recognized"); break;
		//	case StandardizedLogMessage slm: slm.Dump("Type not recognized"); break;
		//};
		
		logMessageProcess.Process(standardLogMessage);
	}
}

public interface ILogMessageQueuer
{
	void Enqueu(StandardizedLogMessage message);
}

public interface ILogMessageDequeuer
{
	IEnumerable<StandardizedLogMessage> Dequeu(int maxDequeuCount);
}

public sealed class LogMessageQueueController : ILogMessageQueuer, ILogMessageDequeuer
{
	public IEnumerable<StandardizedLogMessage> Dequeu(int maxDequeuCount)
	{
		return Enumerable.Empty<StandardizedLogMessage>();
	}

	public void Enqueu(StandardizedLogMessage message)
	{
		
	}
}

public interface ILogMessageProcess
{
	void Process(StandardizedLogMessage message);
}

public sealed class LogMessageProcessComposite : ILogMessageProcess
{
	private readonly IEnumerable<ILogMessageProcess> logMessageProcesses;

	public LogMessageProcessComposite(IEnumerable<ILogMessageProcess> logMessageProcesses)
	{
		this.logMessageProcesses = logMessageProcesses;
	}

	public void Process(StandardizedLogMessage message)
	{
		foreach (var process in logMessageProcesses)
		{
			process.Process(message);
		}
	}
}

public sealed class LogMessageQueueProcess : ILogMessageProcess
{
	private readonly ILogMessageQueuer logMessageQueuer;

	public LogMessageQueueProcess(ILogMessageQueuer logMessageQueuer)
	{
		this.logMessageQueuer = logMessageQueuer;
	}

	public void Process(StandardizedLogMessage message)
	{
		logMessageQueuer.Enqueu(message);
	}
}