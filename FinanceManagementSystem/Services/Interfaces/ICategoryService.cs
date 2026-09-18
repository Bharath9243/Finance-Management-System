using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetMyCategoriesAsync(int userId);

    Task<int> CreateCategoryAsync(
        int userId,
        string name,
        string type);

    Task<Category?> GetCategoryAsync(
        int userId,
        int categoryId);
}