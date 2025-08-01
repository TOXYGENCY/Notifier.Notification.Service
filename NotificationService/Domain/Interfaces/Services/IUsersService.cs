﻿using NotifierNotificationService.NotificationService.API.Dto;
using NotifierNotificationService.NotificationService.Domain.Entities;

namespace NotifierNotificationService.NotificationService.Domain.Interfaces.Services
{
    public interface IUsersService : IDtoConverter<User, UserDto, Guid>
    {
        string HashPassword(string password);
        Task AddUserAsync(UserDto user, string password);
        Task UpdateUserAsync(Guid id, UserDto updatedUser, string? newPassword = null);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
