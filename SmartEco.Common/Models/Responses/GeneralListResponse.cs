
namespace SmartEco.Common.Models.Responses
{
    public record GeneralListResponse<T>(IEnumerable<T> Objects, int CountTotal);
}
