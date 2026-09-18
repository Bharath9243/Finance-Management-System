using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface IAdminActivityService
{
    Task CreateAsync(
        AdminActivity activity,
        System.Data.IDbTransaction dbTransaction);

    Task<IEnumerable<AdminActivity>> GetAllAsync();
}