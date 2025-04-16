using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Models.Entities;
using System.Net.WebSockets;

namespace PersonnelManagement.Repositories.Interfaces
{
    public interface ICccdRepository
    {
        Task<bool> AddPersonnel(IFormFile cccd, Guid rolePersonnelId, string phoneNumber);
        Task<bool> UpdatePersonnel(string personnelId, UpdatePersonnel personnel);
        Task<bool> DeletePersonnel(string id);
        Task<Personnel?> GetPersonnelById(string id);
        Task<Personnel?> GetPersonnelByNumberId(string numberId);
        IEnumerable<Personnel> GetAllPersonnel();
        Task<Personnel?> GetInfoCccdByImage(IFormFile cccd, int options);
        Task<Personnel?> CheckAuthenByImage(IFormFile cccd);

        Task<Personnel?> ProcessCheckInOut(string numberId);
        Task<List<PersonnelHistory>> GetPersonnelHistory();

        // Later fix
        Task<object> CheckAuthenByWebcam(WebSocket webSocket, CancellationToken cancellationToken);

        Task<RolePersonnel> AddRolePersonnel(string roleName);
        Task<RolePersonnel> UpdateRolePersonnel(Guid roleId, string roleName);
        Task<bool> DeleteRolePersonnel(Guid roleId);
        Task<RolePersonnel?> GetRolePersonnelById(Guid roleId);
        Task<IEnumerable<RolePersonnel>> GetAllRolePersonnel();
        Task<bool> AssignRolePersonnelToPersonnel(Guid personnelId, Guid roleId);
    }
}
