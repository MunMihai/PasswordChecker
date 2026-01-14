using PasswordChecker.Data.Models;

namespace PasswordChecker.Data.Repositories.Interfaces;

public interface IPasswordCheckRepository
{
    Task<PasswordCheck> AddAsync(PasswordCheck passwordCheck);
    Task<int> GetTodayCountAsync(Guid subscriptionId);
}
