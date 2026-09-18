using FinanceManagementSystem.Models;
using System.Data;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface IAdminActivityRepository
{
    Task CreateAsync(
        AdminActivity activity,
        IDbTransaction dbTransaction);

    Task<IEnumerable<AdminActivity>> GetAllAsync();
}
