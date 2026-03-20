namespace FamilyHub.Api.Features.Common;

internal static class PaginationValidator
{
    public static void Validate(int page, int pageSize, int maxPageSize = 200)
    {
        if (page <= 0)
            throw new ArgumentException("Page skal være mindst 1.");

        if (pageSize <= 0 || pageSize > maxPageSize)
            throw new ArgumentException($"PageSize skal være mellem 1 og {maxPageSize}.");
    }
}
