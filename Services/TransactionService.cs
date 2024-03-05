using BankAPI.Data;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class TransactionService
{
    private readonly BankContext _context;

    public TransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<bool> Withdraw(int accountId, decimal amount, int? externalAccountId = null)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account != null && account.Balance >= amount)
        {
            account.Balance -= amount;
            var transaction = new BankTransaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionType = externalAccountId.HasValue ? 2 : 1, 
                ExternalAccount = externalAccountId,
                RegData = DateTime.UtcNow
            };
            _context.BankTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

   public async Task<bool> Deposit(int accountId, decimal amount)
{
    var account = await _context.Accounts.FindAsync(accountId);
    if (account == null)
    {
        return false; 
    }

    account.Balance += amount; 

    
    var transaction = new BankTransaction
    {
        AccountId = accountId,
        Amount = amount,
        TransactionType = 3, 
        RegData = DateTime.UtcNow
    };
    _context.BankTransactions.Add(transaction);

    await _context.SaveChangesAsync(); 
    return true;
}

}
