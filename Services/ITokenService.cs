namespace HouseholdExpenseTrackerAPI.Services
{
    public interface ITokenService
    {
        // Decoupled from the EF entity on purpose — takes plain values so it
        // can be unit tested without a DbContext.
         (string Token, DateTime ExpiresAt) GenerateToken(int userId, string name, string email, string roleName);
             
    }
}
