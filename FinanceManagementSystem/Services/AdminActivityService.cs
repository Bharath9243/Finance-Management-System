using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;
using System.Data;

namespace FinanceManagementSystem.Services;

public class AdminActivityService : IAdminActivityService
{
    private readonly IAdminActivityRepository _repository;

    public AdminActivityService(
        IAdminActivityRepository repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(
        AdminActivity activity,
        IDbTransaction dbTransaction)
    {
        return _repository.CreateAsync(
            activity,
            dbTransaction);
    }

    public Task<IEnumerable<AdminActivity>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }
}