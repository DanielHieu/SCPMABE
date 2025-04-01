namespace ScpmaBe.Services.Enums
{
    public enum ParkingSpaceStatus
    {
        Available = 1,  // Space is empty and can be used
        Occupied = 2,   // Space is currently in use
        Reserved = 3,   // Space is reserved for future use
        Disabled = 4,  // Space is not available for use (maintenance, etc.)
        Pending = 5  // Space is in a transitional state (e.g., car entering/exiting)
    }
}
