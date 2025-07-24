using SocialConnectionsAPI.DTOs;

namespace SocialConnectionsAPI.Services
{
    public interface IConnectionService
    {
        Task<ServiceResult<ConnectionStatusResponse>> CreateConnectionAsync(ConnectionRequest request);
        Task<ServiceResult<ConnectionStatusResponse>> RemoveConnectionAsync(ConnectionRequest request);
        Task<ServiceResult<DegreeSeparationResponse>> GetDegreeOfSeparationAsync(string fromUserStrId, string toUserStrId);
    }
}
