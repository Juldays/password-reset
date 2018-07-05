using Models.Requests;
using Models.Responses;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers.Api
{
    [AllowAnonymous]
    public class PasswordResetController : ApiController
    {
        readonly IPasswordResetService passwordResetService;
        public PasswordResetController (IPasswordResetService passwordResetService)
        {
            this.passwordResetService = passwordResetService;
        }

        [Route("api/passwordreset"), HttpPost]
        public HttpResponseMessage Create(ResetCreateRequest req)
        {
            if (req == null)
            {
                ModelState.AddModelError("", "You did not add any body data");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            Guid token = passwordResetService.Create(req);
            ItemResponse<Guid> itemResponse = new ItemResponse<Guid>();
            itemResponse.Item = token;

            return Request.CreateResponse(HttpStatusCode.OK, itemResponse);
        }

        [Route("api/passwordreset"), HttpPut]
        public HttpResponseMessage Update(ResetUpdateRequest req)
        {
            if (req == null)
            {
                ModelState.AddModelError("", "You did not add any body data!");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            passwordResetService.Update(req);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [Route("api/passwordreset"), HttpGet]
        public HttpResponseMessage CheckToken(Guid token)
        {
            HttpResponseMessage requestMsg = null;
            if (token == null)
            {
                ModelState.AddModelError("", "You did not add token!");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            bool validToken = passwordResetService.CheckToken(token);

            if (validToken)
            {
                requestMsg = Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            else
            {
                requestMsg = Request.CreateResponse(HttpStatusCode.BadRequest, "Token is invalid");
            }

            return requestMsg;
        }
    }
}
