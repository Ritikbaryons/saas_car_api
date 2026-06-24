using System;

namespace Saas_Car_Management.Core.DTOs
{
    public class DriverTripDto
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public DateTime ScheduledStart { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CarDetails { get; set; } = string.Empty;
        public string AssignmentStatus { get; set; } = string.Empty;
        public int BookingVehicleId { get; set; }
    }
}
