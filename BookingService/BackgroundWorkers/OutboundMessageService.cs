using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using EventDispatcher.Generic;
using BookingService.Data.Abstract;
using System.Threading;
using BookingService.Models;
using BookingService.Extensions;
using EventDispatcher.generic;
using Microsoft.Extensions.Logging;

namespace BookingService.BackgroundWorkers
{
    public sealed class OutboundMessageService<T> : BackgroundService
    {
        private readonly IEventSender<EventMessage<T>> m_EventSender;
        private readonly IMessageRepository m_MessageRepository;
        private readonly ILogger m_Logger;
        private readonly int m_PollingIntervalInMilliseconds;

        public OutboundMessageService(IEventSender<EventMessage<T>> sender, IMessageRepository messageRepository, ILogger<OutboundMessageService<T>> logger, int pollingInterval)
        {
            m_EventSender = sender;
            m_MessageRepository = messageRepository;
            m_Logger = logger;
            m_PollingIntervalInMilliseconds = pollingInterval;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                List<Message> messagesList = m_MessageRepository.Messages.ToList();
                foreach (Message message in messagesList)
                {
                    try
                    {
                        await m_EventSender.SendAsync(new[] { message.ToEventMessage<T>() });
                        await m_MessageRepository.DeleteAsync(message);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.LogError(ex, $"OutboundMessageService failed handling message with contents: {message.ToString()}");
                    }
                }

                try
                {
                    await Task.Delay(m_PollingIntervalInMilliseconds, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    m_Logger.LogInformation("OutboundMessageService was canceled.");
                }
            }
        }


    }
}
