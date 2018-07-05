using Data.Providers;
using Models.Requests;
using Services.Interfaces;
using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;

namespace Services
{
    public class PasswordResetService : IPasswordResetService
    {
        readonly IDataProvider dataProvider;
        readonly IEmailSenderService emailSenderService;
        public PasswordResetService(IDataProvider dataProvider, IEmailSenderService emailSenderService)
        {
            this.dataProvider = dataProvider;
            this.emailSenderService = emailSenderService;
        }
        public Guid Create(ResetCreateRequest req)
        {
            Guid newToken = default(Guid);
            string userName = "";
            int userId = 0;
            dataProvider.ExecuteNonQuery(
                "PasswordReset_Create", inputParamMapper: (parameters) =>
                {
                    parameters.AddWithValue("@Email", req.Email);
                    parameters.Add("@Token", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
                    parameters.Add("@UserName", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                    parameters.Add("@UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
                }, returnParameters: (parameters) =>
                {
                    userName = (string)parameters["@UserName"].Value;
                    newToken = (Guid)parameters["@Token"].Value;
                    userId = (int)parameters["@UserId"].Value;
                });

            string openUrl = "password-reset?token=" + HttpUtility.UrlEncode(newToken.ToString());

            StringBuilder email = new StringBuilder();
            email.Append("Hi " + userName + ",");
            email.Append(Environment.NewLine + Environment.NewLine);
            email.Append("We've received a request to reset your RecruitHub password.");
            email.Append(Environment.NewLine + Environment.NewLine);
            email.Append("To reset your password please click on this link or cut and paste this URL into your browser:");
            email.Append(Environment.NewLine + Environment.NewLine);
            email.Append(openUrl);
            email.Append(Environment.NewLine + Environment.NewLine);
            email.Append("If you did not request a password reset, please ignore this email. Link will expire in 24 hours.");
            email.Append(Environment.NewLine + Environment.NewLine);
            email.Append("Thank you,");
            email.Append(Environment.NewLine + Environment.NewLine);
            email.Append("RecruitHub Team");
            string emailBody = email.ToString();

            emailSenderService.Send("RecruitHub", "email@example.net", userName, req.Email, "Password Reset", emailBody, null);

            return newToken;
        }

        public void Update(ResetUpdateRequest req)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            dataProvider.ExecuteNonQuery(
                "PasswordReset_Update",
                inputParamMapper: (parameters) =>
                {
                    parameters.AddWithValue("@Password", passwordHash);
                    parameters.AddWithValue("@Token", req.Token);
                });
        }

        public bool CheckToken(Guid token)
        {
            bool validToken = true;
            try
            {
                dataProvider.ExecuteNonQuery(
                "PasswordReset_GetByToken",
                inputParamMapper: (parameters) =>
                {
                    parameters.AddWithValue("@Token", token);
                });
            }
            catch (SqlException ex) when (ex.Number == 51000)
            {
                validToken = false;
            }

            return validToken;
        }
    }
}
