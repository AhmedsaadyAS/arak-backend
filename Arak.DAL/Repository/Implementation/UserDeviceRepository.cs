using Arak.DAL.Database;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Implementation
{
    public class UserDeviceRepository : GenericRepository<UserDevice>, IUserDeviceRepository
    {
        public UserDeviceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task UpsertAsync(string userId, string fcmToken, string? deviceName)
        {
            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.FcmToken == fcmToken);

            if (device != null)
            {
                // Update existing
                device.UserId = userId; // in case token migrated to another logged-in user
                device.LastSeenAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(deviceName))
                {
                    device.DeviceName = deviceName;
                }
                _context.UserDevices.Update(device);
            }
            else
            {
                // Insert new
                device = new UserDevice
                {
                    UserId = userId,
                    FcmToken = fcmToken,
                    DeviceName = deviceName,
                    CreatedAt = DateTime.UtcNow,
                    LastSeenAt = DateTime.UtcNow
                };
                await _context.UserDevices.AddAsync(device);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteByTokenAsync(string userId, string fcmToken)
        {
            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.FcmToken == fcmToken);

            if (device != null)
            {
                _context.UserDevices.Remove(device);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<string>> GetTokensForUserAsync(string userId)
        {
            return await _context.UserDevices
                .Where(d => d.UserId == userId)
                .Select(d => d.FcmToken)
                .ToListAsync();
        }
    }
}
