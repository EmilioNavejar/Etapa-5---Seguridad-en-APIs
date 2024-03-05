using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;

using BankAPI.Data.DTOs;



namespace BankAPI.Controllers
{
    [Authorize(Policy = "IsClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransactionsController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly TransactionService _transactionService;
        private readonly IAuthenticationService _authenticationService; // Servicio hipotético para manejar la autenticación y obtener información del usuario

        public BankTransactionsController(AccountService accountService, TransactionService transactionService, IAuthenticationService authenticationService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            _authenticationService = authenticationService;
        }

        // GET: api/BankTransactions/MyAccounts
        [Authorize(Policy = "IsClient")]
        [HttpGet("MyAccounts")]
        public async Task<IActionResult> GetMyAccounts()
        {
            var clientId = _authenticationService.GetCurrentClientId(User); 
            var accounts = await _accountService.GetAccountsByClientId(clientId);
            if (!accounts.Any()) 
            {
                return NotFound("Ninguna cuenta fue encontrada para este cliente");
            }
            return Ok(accounts);
        }


        // POST: api/BankTransactions/Withdraw
        [Authorize(Policy = "IsClient")]
        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawalDto withdrawal)
        {
            var clientId = _authenticationService.GetCurrentClientId(User);
            
            var result = await _transactionService.Withdraw(withdrawal.AccountId, withdrawal.Amount, withdrawal.ExternalAccountId);
            if (result)
            {
                return Ok(new { message = "Retiro exitoso" });
            }
            else
            {
                return BadRequest(new { message = "Hubo un error al hacer el retiro" });
            }
        }


        // POST: api/BankTransactions/Deposit
        [Authorize(Policy = "IsClient")]
        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto deposit)
        {
            var clientId = _authenticationService.GetCurrentClientId(User);
            var result = await _transactionService.Deposit(clientId, deposit.Amount); 
            if (result)
            {
                return Ok(new { message = "Deposito exitoso" });
            }
            else
            {
                return BadRequest(new { message = "Hubo un fallo en el deposito" });
            }
        }

        // DELETE: api/BankTransactions/DeleteAccount/{accountId}
        [Authorize(Policy = "IsClient")]
        [HttpDelete("DeleteAccount/{accountId}")]
        public async Task<IActionResult> DeleteAccount(int accountId)
        {
            var clientId = _authenticationService.GetCurrentClientId(User);
            var result = await _accountService.DeleteAccountIfEmpty(clientId, accountId);
            if (result)
            {
                return Ok("La cuenta fue eliminada correctamente.");
            }
            else
            {
                return BadRequest("Hubo un error al eliminar la cuenta");
            }
        }
    }
}
