using System.Runtime.CompilerServices;
using Elsa.Extensions;
using Elsa.Scheduling.Bookmarks;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;

namespace Elsa.Scheduling.Activities;

/// <summary>
/// Represents a timer to periodically trigger the workflow.
/// </summary>
[Activity("Elsa", "Scheduling", "Trigger workflow execution at a specific interval using a CRON expression.")]
public class Cron : EventGenerator
{
    /// <inheritdoc />
    public Cron([CallerFilePath] string? source = null, [CallerLineNumber] int? line = null) : base(source, line)
    {
    }

    /// <inheritdoc />
    public Cron(string cronExpression, [CallerFilePath] string? source = null, [CallerLineNumber] int? line = null) : this(new Input<string>(cronExpression), source, line)
    {
    }

    /// <inheritdoc />
    public Cron(Input<string> cronExpression, [CallerFilePath] string? source = null, [CallerLineNumber] int? line = null) : this(source, line)
    {
        CronExpression = cronExpression;
    }

    /// <summary>
    /// The interval at which the timer should execute.
    /// </summary>
    [Input(Description = "The CRON expression at which the timer should execute.")]
    public Input<string> CronExpression { get; set; } = null!;

    /// <inheritdoc />
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        if(context.IsTriggerOfWorkflow())
        {
            await context.CompleteActivityAsync();
            return;
        }
        
        var cronParser = context.GetRequiredService<ICronParser>();
        var cronExpression = context.ExpressionExecutionContext.Get(CronExpression)!;
        var executeAt = cronParser.GetNextOccurrence(cronExpression);
        
        context.JournalData.Add("ExecuteAt", executeAt);
        context.CreateBookmark(new CronBookmarkPayload(executeAt, cronExpression));
    }

    /// <inheritdoc />
    protected override object GetTriggerPayload(TriggerIndexingContext context)
    {
        var cronExpression = context.ExpressionExecutionContext.Get(CronExpression)!;
        return new CronTriggerPayload(cronExpression);
    }

    /// <summary>
    /// Creates a new <see cref="Cron"/> activity set to trigger at the specified cron expression.
    /// </summary>
    public static Cron FromCronExpression(string value, [CallerFilePath] string? source = null, [CallerLineNumber] int? line = null) => new(value, source, line);
}