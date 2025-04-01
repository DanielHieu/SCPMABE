public class ContractFutureExpiredResponse
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string ParkingLotName { get; set; }
    public DateOnly ExpiredDate { get; set; }
    public int RemainingDays { get; set; }
}

