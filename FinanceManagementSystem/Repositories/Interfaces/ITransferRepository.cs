using FinanceManagementSystem.Models;
using System.Data;
using System.Security.Cryptography.Xml;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface ITransferRepository
{
    Task<int> CreateAsync(
        Transfer transfer,
        IDbTransaction dbTransaction);
}