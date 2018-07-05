using Models.Requests;
using System;

namespace Services
{
    public interface IPasswordResetService
    {
        Guid Create(ResetCreateRequest req);
        void Update(ResetUpdateRequest req);
        bool CheckToken(Guid token);
    }
}