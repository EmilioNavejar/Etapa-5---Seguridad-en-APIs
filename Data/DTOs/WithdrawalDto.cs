namespace BankAPI.Data.DTOs;

public class WithdrawalDto
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public int? ExternalAccountId { get; set; }
}