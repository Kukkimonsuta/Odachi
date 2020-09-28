using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Odachi.AspNetCore.Authentication.ApiKey
{
	public class ApiKeyCredential
	{
		public string ApiKey { get; set; }
		public ApiKeyCredentialClaim[] Claims { get; set; } = Array.Empty<ApiKeyCredentialClaim>();
	}
}
