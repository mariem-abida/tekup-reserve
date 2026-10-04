using System.Collections.Generic;

namespace TEKUP_RESERVE.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int BookingsToday { get; set; }

        public List<Booking> UpcomingBookings { get; set; } = new();
        public List<TopRoomItem> TopRooms { get; set; } = new();

        public class TopRoomItem
        {
            public string RoomName { get; set; } = string.Empty;
            public int ReservationsCount { get; set; }
        }
    }
}
