# BaseWorker Documentation

## Overview

`BaseWorker` is an abstract class that provides a foundational structure for implementing background workers in a .NET
application. It extends the `BackgroundService` class, offering additional features for managing worker execution,
timeouts, and error handling.

## Main Features

1. **Configurable Activity**: Workers can be activated or deactivated dynamically through the `IsActive` property.
2. **Iteration Timeout**: Optional timeout for each worker iteration, configurable via `IterationTimeout`.
3. **Error Handling**: Built-in error catching and logging.
4. **Configurable Delays**: Separate delay times for normal operation and after errors.
5. **Cancellation Support**: Proper handling of cancellation requests.
6. **Time Unit Flexibility**: Delays and timeouts can be specified in various time units (seconds, minutes, hours, days)
   through configuration. Simply use the appropriate suffix (
   e.g., `DelaySeconds`, `ErrorDelayMinutes`, `IterationTimeoutHours`) in your configuration, and adjust
   your `GetDelayTime` and `GetErrorDelayTime` implementations accordingly.

## Installation

### Build and Use the Library

Clone and build the repository:

```bash
git clone https://github.com/nora-alshareef/JobUtils
cd JobUtils
dotnet build
dotnet pack
```

Add Reference to the JobUtils DLL
To add a reference to the JobUtils DLL in your project, follow these steps:

1. Right-click on your project in the Solution Explorer.
2. Select "Add" > "Reference".
3. In the Reference Manager dialog, click on "Browse".
4. Navigate to the location of the Job Utils DLL file.
5. Select the DLL file (e.g., "JobUtils.dll") and click "Add".
6. Click "OK" in the Reference Manager dialog to confirm.

Alternatively, if you're using the command line or prefer editing the .csproj file directly, you can add the following
line within an <ItemGroup> in your project file:

```xml
<Reference Include="JobUtils">
    <HintPath>path\to\JobUtils.dll</HintPath>
</Reference>
```

## How to Use

### 1. Create a Worker Class

Inherit from `BaseWorker` and implement the required abstract methods:

```csharp
public class MyWorker( 
    ILogger<MyWorker> logger,
    MyHandler handler, // if needed ! 
    IOptions<WorkersConfigurations> config) : BaseWorker
{
    private readonly WorkerConfiguration _config = config.Value.Workers["MyWorker"];
        
        protected override bool IsActive()
        {
            return _config.IsActive;
        }

        protected override TimeSpan? GetIterationTimeout()
        {
            return _config.IterationTimeoutSeconds.HasValue
                ? TimeSpan.FromSeconds(_config.IterationTimeoutSeconds.Value)
                : null;
        }

    public MyWorker(ILogger<MyWorker> logger, IOptions<WorkersConfigurations> config) 
        : base(logger)
    {
        _config = config.Value.Workers["MyWorker"];
        IsActive = _config.IsActive;
        IterationTimeout = _config.IterationTimeoutSeconds.HasValue 
            ? TimeSpan.FromSeconds(_config.IterationTimeoutSeconds.Value) 
            : null;
    }

    protected override async Task ProcessAsync()
    {
        // Implement your worker logic here
        Logger.LogInformation("MyWorker is processing...");
        await Task.Delay(1000);
    }

    protected override TimeSpan GetDelayTime() => TimeSpan.FromSeconds(_config.DelaySeconds);

    protected override TimeSpan GetErrorDelayTime() => TimeSpan.FromSeconds(_config.ErrorDelaySeconds);
}
```

### 2. Configure Your Worker

Add configuration to your appsettings.json:

```json
{
    "WorkersConfigurations": {
      "Workers": {
        "MyWorker": {
          "DelaySeconds": 30,
          "ErrorDelaySeconds": 60,
          "IsActive": true,
          "IterationTimeoutSeconds": 120
        }
      }
    }
}
```

### 3. Register Your Worker

In your Program.cs:

```csharp
services.Configure<WorkersConfigurations>(Configuration.GetSection("WorkersConfigurations"));
services.AddHostedService<MyWorker>();
```

## Key Concepts

### ProcessAsync

- Core logic implementation of your worker
- Called repeatedly while the worker is active and not cancelled
- Override this method to define your worker's specific functionality

### IsActive

- Controls worker execution
- When `true`: Worker executes `ProcessAsync`
- When `false`: Worker runs but skips `ProcessAsync`
- Allows dynamic activation/deactivation without stopping the service

### IterationTimeout

- Optional maximum duration for each `ProcessAsync` execution
- If set and reached, the current iteration is cancelled
- Helps prevent long-running or stuck iterations

### GetDelayTime and GetErrorDelayTime

- Define intervals between worker iterations
- `GetDelayTime`: Normal operation delay
- `GetErrorDelayTime`: Delay after an error occurs
- Allows for different backoff strategies in error scenarios