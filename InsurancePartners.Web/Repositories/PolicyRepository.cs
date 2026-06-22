using Dapper;
using InsurancePartners.Web.Models;
using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Web.Repositories;

public sealed class PolicyRepository(IDbConnectionFactory connectionFactory) : IPolicyRepository
{
    public async Task<IReadOnlyList<PolicyListItemViewModel>> GetByPartnerIdAsync(int partnerId)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                PartnerId,
                PolicyNumber,
                PolicyAmount,
                CreatedAtUtc
            FROM dbo.Policies
            WHERE PartnerId = @PartnerId
              AND IsDeleted = 0
            ORDER BY CreatedAtUtc DESC, Id DESC;
            """;

        var policies = await connection.QueryAsync<PolicyListItemViewModel>(
            sql,
            new { PartnerId = partnerId });

        return policies.ToList();
    }

    public async Task<Policy?> GetByIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                PartnerId,
                PolicyNumber,
                PolicyAmount,
                CreatedAtUtc,
                UpdatedAtUtc,
                IsDeleted,
                DeletedAtUtc,
                RowVersion
            FROM dbo.Policies
            WHERE Id = @Id
              AND IsDeleted = 0;
            """;

        return await connection.QuerySingleOrDefaultAsync<Policy>(
            sql,
            new { Id = id });
    }

    public async Task<int> CreateAsync(Policy policy)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO dbo.Policies
            (
                PartnerId,
                PolicyNumber,
                PolicyAmount
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @PartnerId,
                @PolicyNumber,
                @PolicyAmount
            );
            """;

        return await connection.ExecuteScalarAsync<int>(sql, policy);
    }

    public async Task<bool> UpdateAsync(Policy policy)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            UPDATE dbo.Policies
            SET
                PolicyNumber = @PolicyNumber,
                PolicyAmount = @PolicyAmount,
                UpdatedAtUtc = SYSUTCDATETIME()
            WHERE Id = @Id
              AND IsDeleted = 0
              AND RowVersion = @RowVersion;
            """;

        var affectedRows = await connection.ExecuteAsync(sql, policy);

        return affectedRows == 1;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            UPDATE dbo.Policies
            SET
                IsDeleted = 1,
                DeletedAtUtc = SYSUTCDATETIME()
            WHERE Id = @Id
              AND IsDeleted = 0;
            """;

        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

        return affectedRows == 1;
    }

    public async Task<int> SoftDeleteByPartnerIdAsync(int partnerId)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            UPDATE dbo.Policies
            SET
                IsDeleted = 1,
                DeletedAtUtc = SYSUTCDATETIME()
            WHERE PartnerId = @PartnerId
              AND IsDeleted = 0;
            """;

        return await connection.ExecuteAsync(sql, new { PartnerId = partnerId });
    }

    public async Task<bool> ExistsByPolicyNumberAsync(
        string policyNumber,
        int? excludedPolicyId = null)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT CAST(
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM dbo.Policies
                        WHERE PolicyNumber = @PolicyNumber
                          AND (@ExcludedPolicyId IS NULL OR Id <> @ExcludedPolicyId)
                    )
                    THEN 1
                    ELSE 0
                END
                AS bit
            );
            """;

        return await connection.ExecuteScalarAsync<bool>(
            sql,
            new
            {
                PolicyNumber = policyNumber,
                ExcludedPolicyId = excludedPolicyId
            });
    }

    public async Task<PartnerPolicySummaryViewModel> GetPartnerPolicySummaryAsync(int partnerId)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                @PartnerId AS PartnerId,
                COUNT(Id) AS PolicyCount,
                COALESCE(SUM(PolicyAmount), 0) AS TotalPolicyAmount,
                CAST(
                    CASE
                        WHEN COUNT(Id) >= 5
                             OR COALESCE(SUM(PolicyAmount), 0) > 5000
                        THEN 1
                        ELSE 0
                    END
                    AS bit
                ) AS IsMarked
            FROM dbo.Policies
            WHERE PartnerId = @PartnerId
              AND IsDeleted = 0;
            """;

        return await connection.QuerySingleAsync<PartnerPolicySummaryViewModel>(
            sql,
            new { PartnerId = partnerId });
    }
}