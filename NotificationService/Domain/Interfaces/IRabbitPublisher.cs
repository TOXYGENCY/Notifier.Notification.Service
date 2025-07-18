﻿namespace NotifierNotificationService.NotificationService.Domain.Interfaces
{
    public interface IRabbitPublisher
    {
        Task PublishAsync<T>(T message, string queue);
        Task PublishAsync<T>(T message);
    }
}
