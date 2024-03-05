using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;


public class AccountService
{

    private readonly BankContext _context;
    public AccountService(BankContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<AccountDtoOut>> GetAll()
    {
        return await _context.Accounts.Select(a => new AccountDtoOut{
            Id=a.Id,
            AccountName=a.AccountTypeNavigation.Name,
            ClientName=a.Client !=null ? a.Client.Name : "",
            Balance=a.Balance,
            RegData=a.RegData
        }).ToListAsync();
    }


     public async Task<AccountDtoOut?> GetDtoById(int id)
    {
        return await _context.Accounts
        .Where(a=>a.Id==id)
        .Select(a => new AccountDtoOut{
            Id=a.Id,
            AccountName=a.AccountTypeNavigation.Name,
            ClientName=a.Client !=null ? a.Client.Name : "",
            Balance=a.Balance,
            RegData=a.RegData
        }).SingleOrDefaultAsync();
    }



    public async Task<Account?> GetById(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }



    public async Task<Account> Create(AccountDtoIn newAccountDTO)
    {

        var newAccount=new Account();

        newAccount.AccountType = newAccountDTO.AccountType;
        newAccount.ClientId = newAccountDTO.ClientId;   
        newAccount.Balance = newAccountDTO.Balance;

        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        return newAccount;
    }



    public async Task Update(AccountDtoIn account)
    {
        var existingAccount = await GetById(account.Id);

        if (existingAccount is not null)
        {
            existingAccount.AccountType = account.AccountType;
            existingAccount.ClientId = account.ClientId;
            existingAccount.Balance = account.Balance;
            await _context.SaveChangesAsync();
        }

    }




    public async Task Delete(int id)
    {
        var accountToDelete = await GetById(id);

        if (accountToDelete is not null)
        {
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }
    }


   

public async Task<IEnumerable<AccountDtoOut>> GetAccountsByClientId(int clientId)
{
    return await _context.Accounts
        .Where(a => a.ClientId == clientId)
        .Select(a => new AccountDtoOut
        {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegData = a.RegData
        }).ToListAsync();
}

public async Task<bool> DeleteAccountIfEmpty(int clientId, int accountId)
{
    var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.ClientId == clientId);
    if (account != null && account.Balance == 0)
    {
       
        var transactions = _context.BankTransactions.Where(t => t.AccountId == accountId);
        _context.BankTransactions.RemoveRange(transactions);

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }
    return false;
}



}