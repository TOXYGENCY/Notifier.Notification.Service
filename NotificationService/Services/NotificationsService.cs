﻿using Microsoft.AspNetCore.Identity;
using NotifierNotificationService.NotificationService.Domain.Interfaces.Repositories;
using NotifierNotificationService.NotificationService.Domain.Entities;
using NotifierNotificationService.NotificationService.Domain.Interfaces.Services;
using NotifierNotificationService.NotificationService.Domain.Entities.Dto;
using System.Text.Json;

namespace NotifierNotificationService.NotificationService.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository notificationsRepository;

        public NotificationsService(INotificationsRepository notificationsRepository)
        {
            this.notificationsRepository = notificationsRepository;
        }

        public async Task UpdateNotificationAsync(Guid notificationId, NotificationDto updatedNotificationDto)
        {
            if (updatedNotificationDto == null || updatedNotificationDto.Id == null) throw new ArgumentNullException("Не все аргументы переданы.");
            if (updatedNotificationDto.Id != notificationId) throw new ArgumentException("ID не совпадают.");

            var notification = await notificationsRepository.GetByIdAsync(notificationId);
            if (notification == null) throw new InvalidOperationException($"Уведомление с Id = {notificationId} не найдено.");

            // TODO: исчезают виртуальные EF-свойства - понять проблема ли это
            var updatedNotification = FromDto(updatedNotificationDto);
            await notificationsRepository.UpdateAsync(updatedNotification);
        }

        public async Task AddNotificationAsync(NotificationDto newNotificationDto)
        {
            if (newNotificationDto is null) throw new ArgumentNullException(nameof(newNotificationDto));
            if (newNotificationDto.Id != null)
            {
                var existingNotification = await notificationsRepository.GetByIdAsync(newNotificationDto.Id.Value);
                if (existingNotification != null) throw new ArgumentException(newNotificationDto.Id.Value.ToString());
            }

            var newNotification = await FromDtoAsync(newNotificationDto);

            await notificationsRepository.AddAsync(newNotification);
        }

        public async Task<Notification> FromDtoAsync(NotificationDto notificationDto)
        {
            if (notificationDto is null) throw new ArgumentNullException(nameof(notificationDto));
            Notification? notification = null;

            if (notificationDto.Id != null)
                notification = await notificationsRepository.GetByIdAsync(notificationDto.Id.Value);

            // TODO: что с Guid? он станет пустой после сериализации (если он null) или нет?
            notification ??= FromDto(notificationDto);
            return notification;
        }
        public Notification FromDto(NotificationDto notificationDto)
        {
            if (notificationDto is null)
                throw new ArgumentNullException(nameof(notificationDto));

            if (notificationDto.Id == null)
                // TODO: разобраться подробнее как быть с Guid
                notificationDto.Id = Guid.Empty;

            if (notificationDto.CreatedAt == null) 
                notificationDto.CreatedAt = DateTime.MinValue;

            var notification = JsonSerializationConvert<NotificationDto, Notification>(notificationDto);
            return notification;
        }


        public NotificationDto ToDto(Notification notification)
        {
            if (notification is null) throw new ArgumentNullException(nameof(notification));

            return JsonSerializationConvert<Notification, NotificationDto>(notification);
        }

        public IEnumerable<NotificationDto> ToDtos(IEnumerable<Notification> notifications)
        {
            if (notifications is null) throw new ArgumentNullException(nameof(notifications));
            var notificationDtos = new List<NotificationDto>();

            foreach (var notification in notifications)
                notificationDtos.Add(ToDto(notification));

            return notificationDtos;
        }

        /// <summary>
        /// Конвертирование из объекта src типа SRC в объект типа DEST через сериализацию и десереализацию в JSON-объект (встроенный авто-маппинг).
        /// </summary>
        /// <typeparam name="SRC"></typeparam>
        /// <typeparam name="DEST"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        private DEST JsonSerializationConvert<SRC, DEST>(SRC src)
        {
            return JsonSerializer.Deserialize<DEST>(JsonSerializer.Serialize(src));
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(Guid notificationId)
        {
            var notification = await notificationsRepository.GetByIdAsync(notificationId);
            // TODO: решить вот с этим
            if (notification is null) return null;
            return ToDto(notification);
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var notifications = await notificationsRepository.GetAllAsync();
            return ToDtos(notifications);
        }
    }
}