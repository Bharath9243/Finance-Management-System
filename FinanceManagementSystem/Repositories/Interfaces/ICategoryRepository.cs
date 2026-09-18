using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetByUserIdAsync(int userId);

    Task<int> CreateAsync(Category category);

    Task<Category?> GetByIdAsync(int categoryId);
}