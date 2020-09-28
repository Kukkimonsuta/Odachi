using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Odachi.AspNetCore.Authentication.Basic
{
	public class BasicCredential
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public BasicCredentialClaim[] Claims { get; set; } = Array.Empty<BasicCredentialClaim>();
	}
}
