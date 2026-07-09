using Rased.Core.DTO.UserMode;
using Rased.Core.Entities;
using Rased_Project;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Rased.Infrustracture.Services
{
    public class RideMatchingService
    {
        private readonly ApplicationDbContext _context;

        public RideMatchingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateUserLocationAsync(Guid userId, UpdateLocationDto dto)
        {
            var existingLocation = await _context.UserLiveLocations
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (existingLocation != null)
            {
                existingLocation.Lat = dto.Lat;
                existingLocation.Lng = dto.Lng;
                existingLocation.IsSharingLive = dto.IsSharingLive;
                existingLocation.UserType = dto.UserType;
                existingLocation.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newLocation = new UserLiveLocation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Lat = dto.Lat,
                    Lng = dto.Lng,
                    IsSharingLive = dto.IsSharingLive,
                    UserType = dto.UserType,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.UserLiveLocations.AddAsync(newLocation);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<LiveUserDto>> GetActivePassengersAsync(double driverLat, double driverLng, double radiusInKm = 5.0)
        {
            var activeTimeThreshold = DateTime.UtcNow.AddMinutes(-5);

            var activePassengers = await _context.UserLiveLocations
                .Include(l => l.User)
                .ThenInclude(u => u.UserProfile) 
                .Where(l => l.UserType == "Passenger" && l.IsSharingLive && l.UpdatedAt >= activeTimeThreshold)
                .ToListAsync();

            var nearbyPassengers = new List<LiveUserDto>();

            foreach (var passenger in activePassengers)
            {
                var distance = CalculateDistance(driverLat, driverLng, passenger.Lat, passenger.Lng);
                if (distance <= radiusInKm)
                {
                    nearbyPassengers.Add(new LiveUserDto
                    {
                        UserId = passenger.UserId,
                        FullName = passenger.User?.UserProfile?.FullName ?? "راكب منتظر",
                        Lat = passenger.Lat,
                        Lng = passenger.Lng
                    });
                }
            }

            return nearbyPassengers;
        }

        public async Task<List<HotspotDto>> GetHotspotsAsync(double radiusInMeters = 500)
        {
            var hotspots = await _context.GatheringSpots.ToListAsync();

            var activeTimeThreshold = DateTime.UtcNow.AddMinutes(-5);
            var activePassengers = await _context.UserLiveLocations
                .Where(l => l.UserType == "Passenger" && l.IsSharingLive && l.UpdatedAt >= activeTimeThreshold)
                .ToListAsync();

            var result = new List<HotspotDto>();

            foreach (var spot in hotspots)
            {
                int count = activePassengers.Count(p =>
                    CalculateDistance(spot.Lat, spot.Lng, p.Lat, p.Lng) <= (radiusInMeters / 1000.0));

                result.Add(new HotspotDto
                {
                    SpotId = spot.Id,
                    SpotName = spot.Name,
                    Lat = spot.Lat,
                    Lng = spot.Lng,
                    PassengerCount = count
                });
            }

            return result;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; 
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle) => (Math.PI / 180) * angle;
    }
}
