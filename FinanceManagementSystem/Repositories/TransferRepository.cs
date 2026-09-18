using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using System.Data;
using System.Security.Cryptography.Xml;

namespace FinanceManagementSystem.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public TransferRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
        Transfer transfer,
        IDbTransaction dbTransaction)
    {
        return await dbTransaction.Connection!
            .ExecuteScalarAsync<int>(
                """
                INSERT INTO Transfers
                (
                    FromAccountId,
                    ToAccountId,
                    Amount,
                    TransferDate
                )
                OUTPUT INSERTED.TransferId
                VALUES
                (
                    @FromAccountId,
                    @ToAccountId,
                    @Amount,
                    SYSUTCDATETIME()
                )
                """,
                transfer,
                dbTransaction);
    }
}