using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetMyCategoriesAsync(
        int userId)
    {
        return await _categoryRepository.GetByUserIdAsync(userId);
    }

    public async Task<int> CreateCategoryAsync(
        int userId,
        string name,
        string type)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Category name is required.");
        }

        name = name.Trim();

        if (type != "Income" && type != "Expense")
        {
            throw new ArgumentException(
                "Category type must be Income or Expense.");
        }

        var existingCategories =
            await _categoryRepository.GetByUserIdAsync(userId);

        if (existingCategories.Any(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            c.Type == type))
        {
            throw new ArgumentException(
                "A category with this name already exists.");
        }

        var category = new Category
        {
            UserId = userId,
            Name = name,
            Type = type
        };

        return await _categoryRepository.CreateAsync(category);
    }

    public async Task<Category?> GetCategoryAsync(
        int userId,
        int categoryId)
    {
        var category =
            await _categoryRepository.GetByIdAsync(categoryId);

        if (category == null)
        {
            return null;
        }

        if (category.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access this category.");
        }

        return category;
    }
}