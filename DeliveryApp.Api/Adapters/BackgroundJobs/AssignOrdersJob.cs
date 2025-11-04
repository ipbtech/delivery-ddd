using DeliveryApp.Core.Application.Commands.AssignOrder;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignOrdersJob(IMediator mediator) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var assignOrdersCommand = new AssignOrderCommand();
        return mediator.Send(assignOrdersCommand);
    }
}