﻿namespace NotifierNotificationService.NotificationService.API.Dto;

public partial class StatusDto
{
    public short? Id { get; set; }

    public string? Name { get; set; }

    public string EngName { get; set; } = null!;
}
