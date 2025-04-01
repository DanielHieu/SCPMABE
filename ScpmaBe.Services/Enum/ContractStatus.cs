namespace ScpmaBe.Services.Enums
{
    public enum ContractStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        PendingActivation = 4, // Contract is active but not yet effective
        Canceled = 5
    }
}
