using Dapper;
using InsurancePartners.Web.Models;
using InsurancePartners.Web.ViewModels.Partners;
using InsurancePartners.Web.ViewModels.Policies;

namespace InsurancePartners.Web.Repositories;

public sealed class PartnerRepository(IDbConnectionFactory connectionFactory) : IPartnerRepository
{
    public async Task<IReadOnlyList<PartnerListItemViewModel>> GetAllAsync()
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                p.Id,
                CONCAT(p.FirstName, N' ', p.LastName) AS FullName,
                p.PartnerNumber,
                p.CroatianPIN,
                pt.Name AS PartnerTypeName,
                p.CreatedAtUtc,
                p.IsForeign,
                p.Gender,
                COUNT(po.Id) AS PolicyCount,
                COALESCE(SUM(po.PolicyAmount), 0) AS TotalPolicyAmount,
                CAST(
                    CASE
                        WHEN COUNT(po.Id) >= 5
                             OR COALESCE(SUM(po.PolicyAmount), 0) > 5000
                        THEN 1
                        ELSE 0
                    END
                    AS bit
                ) AS IsMarked
            FROM dbo.Partners p
            INNER JOIN dbo.PartnerTypes pt ON pt.Id = p.PartnerTypeId
            LEFT JOIN dbo.Policies po
                ON po.PartnerId = p.Id
                AND po.IsDeleted = 0
            WHERE p.IsDeleted = 0
            GROUP BY
                p.Id,
                p.FirstName,
                p.LastName,
                p.PartnerNumber,
                p.CroatianPIN,
                pt.Name,
                p.CreatedAtUtc,
                p.IsForeign,
                p.Gender
            ORDER BY
                p.CreatedAtUtc DESC,
                p.Id DESC;
            """;

        var partners = await connection.QueryAsync<PartnerListItemViewModel>(sql);

        return partners.ToList();
    }

    public async Task<Partner?> GetByIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                FirstName,
                LastName,
                Address,
                PartnerNumber,
                CroatianPIN,
                PartnerTypeId,
                CreatedAtUtc,
                CreateByUser,
                IsForeign,
                ExternalCode,
                Gender,
                UpdatedAtUtc,
                IsDeleted,
                DeletedAtUtc,
                RowVersion
            FROM dbo.Partners
            WHERE Id = @Id
              AND IsDeleted = 0;
            """;

        return await connection.QuerySingleOrDefaultAsync<Partner>(sql, new { Id = id });
    }

    public async Task<PartnerDetailsViewModel?> GetDetailsAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                p.Id,
                CONCAT(p.FirstName, N' ', p.LastName) AS FullName,
                p.Address,
                p.PartnerNumber,
                p.CroatianPIN,
                pt.Name AS PartnerTypeName,
                p.CreatedAtUtc,
                p.CreateByUser,
                p.IsForeign,
                p.ExternalCode,
                p.Gender,
                COUNT(po.Id) AS PolicyCount,
                COALESCE(SUM(po.PolicyAmount), 0) AS TotalPolicyAmount,
                CAST(
                    CASE
                        WHEN COUNT(po.Id) >= 5
                             OR COALESCE(SUM(po.PolicyAmount), 0) > 5000
                        THEN 1
                        ELSE 0
                    END
                    AS bit
                ) AS IsMarked
            FROM dbo.Partners p
            INNER JOIN dbo.PartnerTypes pt ON pt.Id = p.PartnerTypeId
            LEFT JOIN dbo.Policies po
                ON po.PartnerId = p.Id
                AND po.IsDeleted = 0
            WHERE p.Id = @Id
              AND p.IsDeleted = 0
            GROUP BY
                p.Id,
                p.FirstName,
                p.LastName,
                p.Address,
                p.PartnerNumber,
                p.CroatianPIN,
                pt.Name,
                p.CreatedAtUtc,
                p.CreateByUser,
                p.IsForeign,
                p.ExternalCode,
                p.Gender;

            SELECT
                Id,
                PartnerId,
                PolicyNumber,
                PolicyAmount,
                CreatedAtUtc
            FROM dbo.Policies
            WHERE PartnerId = @Id
              AND IsDeleted = 0
            ORDER BY CreatedAtUtc DESC, Id DESC;
            """;

        using var result = await connection.QueryMultipleAsync(sql, new { Id = id });

        var partner = await result.ReadSingleOrDefaultAsync<PartnerDetailsViewModel>();

        if (partner is null)
        {
            return null;
        }

        var policies = await result.ReadAsync<PolicyListItemViewModel>();

        partner.Policies = policies.ToList();

        return partner;
    }

    public async Task<IReadOnlyList<PartnerTypeOptionViewModel>> GetPartnerTypesAsync()
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name
            FROM dbo.PartnerTypes
            ORDER BY Id;
            """;

        var partnerTypes = await connection.QueryAsync<PartnerTypeOptionViewModel>(sql);

        return partnerTypes.ToList();
    }

    public async Task<int> CreateAsync(Partner partner)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO dbo.Partners
            (
                FirstName,
                LastName,
                Address,
                PartnerNumber,
                CroatianPIN,
                PartnerTypeId,
                CreateByUser,
                IsForeign,
                ExternalCode,
                Gender
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @FirstName,
                @LastName,
                @Address,
                @PartnerNumber,
                @CroatianPIN,
                @PartnerTypeId,
                @CreateByUser,
                @IsForeign,
                @ExternalCode,
                @Gender
            );
            """;

        return await connection.ExecuteScalarAsync<int>(sql, partner);
    }

    public async Task<bool> UpdateAsync(Partner partner)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            UPDATE dbo.Partners
            SET
                FirstName = @FirstName,
                LastName = @LastName,
                Address = @Address,
                PartnerNumber = @PartnerNumber,
                CroatianPIN = @CroatianPIN,
                PartnerTypeId = @PartnerTypeId,
                CreateByUser = @CreateByUser,
                IsForeign = @IsForeign,
                ExternalCode = @ExternalCode,
                Gender = @Gender,
                UpdatedAtUtc = SYSUTCDATETIME()
            WHERE Id = @Id
              AND IsDeleted = 0
              AND RowVersion = @RowVersion;
            """;

        var affectedRows = await connection.ExecuteAsync(sql, partner);

        return affectedRows == 1;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            UPDATE dbo.Partners
            SET
                IsDeleted = 1,
                DeletedAtUtc = SYSUTCDATETIME()
            WHERE Id = @Id
              AND IsDeleted = 0;
            """;

        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

        return affectedRows == 1;
    }

    public async Task<bool> ExistsByPartnerNumberAsync(
        string partnerNumber,
        int? excludedPartnerId = null)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT CAST(
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM dbo.Partners
                        WHERE PartnerNumber = @PartnerNumber
                          AND (@ExcludedPartnerId IS NULL OR Id <> @ExcludedPartnerId)
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
                PartnerNumber = partnerNumber,
                ExcludedPartnerId = excludedPartnerId
            });
    }

    public async Task<bool> ExistsByExternalCodeAsync(
        string externalCode,
        int? excludedPartnerId = null)
    {
        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT CAST(
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM dbo.Partners
                        WHERE ExternalCode = @ExternalCode
                          AND (@ExcludedPartnerId IS NULL OR Id <> @ExcludedPartnerId)
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
                ExternalCode = externalCode,
                ExcludedPartnerId = excludedPartnerId
            });
    }

    public async Task<bool> ExistsByCroatianPinAsync(
        string? croatianPin,
        int? excludedPartnerId = null)
    {
        if (string.IsNullOrWhiteSpace(croatianPin))
        {
            return false;
        }

        using var connection = connectionFactory.CreateConnection();

        const string sql = """
            SELECT CAST(
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM dbo.Partners
                        WHERE CroatianPIN = @CroatianPIN
                          AND (@ExcludedPartnerId IS NULL OR Id <> @ExcludedPartnerId)
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
                CroatianPIN = croatianPin,
                ExcludedPartnerId = excludedPartnerId
            });
    }
}