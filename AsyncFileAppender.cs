using log4net;
using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
///   <logger name="position">
///     <level value="DEBUG" />
///     <appender-ref ref="TESTLogger" />
///   </logger>
///   <appender name="TESTLogger" type="namespace.AsyncFileAppender">
///     <lockingModel type="log4net.Appender.FileAppender+MinimalLock"/>
///     <file value="Log\position.txt"/>
///     <appendToFile value="true"/>
///     <rollingStyle value="Date"/>
///     <datePattern value="yyyyMMdd"/>
///     <maxSizeRollBackups value="10"/>
///     <layout type="log4net.Layout.PatternLayout">
///       <conversionPattern value="%d{MM.dd.yyyy HH:mm:ss.fff} [%-5level] %logger // %message%newline"/>
///     </layout>
///     <filter type="log4net.Filter.LevelRangeFilter">
///       <levelMin value="ALL"/>
///       <acceptOnMatch value="true"/>
///     </filter>
///     <filter type="log4net.Filter.DenyAllFilter"/>
///   </appender>
/// </summary>
public class AsyncFileAppender : RollingFileAppender
{
	volatile static bool running = false;
	static ConcurrentBag<AsyncFileAppender> afAppenders = new ConcurrentBag<AsyncFileAppender>();
	static System.Threading.AutoResetEvent mAutoResetEvent = new System.Threading.AutoResetEvent(true);

	static int waitTime = 5 * 1000;

	object lockObject = new object();
	List<LoggingEvent> loggingEvents = new List<LoggingEvent>();

	/// <summary>
	/// Thread wait time (ms)
	/// </summary>
	public int WaitTime
	{
		get { return waitTime; }
		set
		{
			if (value > 0)
			{
				waitTime = value;
			}
		}
	}
	public AsyncFileAppender()
		: base()
	{
		if (running == false) { processQueue(); }

		afAppenders.Add(this);
	}

	protected override void Append(log4net.Core.LoggingEvent loggingEvent)
	{
		if (running == false) { return; }
		lock (this.lockObject)
		{
			this.loggingEvents.Add(loggingEvent);
		}
	}

	static void processQueue()
	{
		if (running) { return; } running = true; // prevent double

		new System.Threading.Thread(() =>
		{
			try
			{
				System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
				System.Threading.Thread.Sleep(300); // wait for first run delay
				while (running)
				{
					writeLogs();
					mAutoResetEvent.WaitOne(waitTime);
				}
			}
			catch (System.Threading.ThreadInterruptedException ex)
			{
			}
			catch (System.Threading.ThreadAbortException ex)
			{
			}
		}).Start();

	}
	static void writeLogs()
	{
		foreach (var appKV in afAppenders)
		{
			if (appKV.loggingEvents.Count > 0)
			{
				lock (appKV.lockObject)
				{
					appKV.AddToBaseAppender(appKV.loggingEvents.ToArray());
					appKV.loggingEvents.Clear();
				}
			}
		}
	}

	void AddToBaseAppender(LoggingEvent[] e)
	{
		base.Append(e);
	}

	protected override void OnClose()
	{
		mAutoResetEvent.Set();
		running = false;
		base.OnClose();
	}
}
