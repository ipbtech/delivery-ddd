using DeliveryApp.Core.Application.Commands.MoveCouriers;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class MoveCouriersJob(IMediator mediator) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var moveCourierToOrderCommand = new MoveCouriersCommand();
        return mediator.Send(moveCourierToOrderCommand);
    }
}