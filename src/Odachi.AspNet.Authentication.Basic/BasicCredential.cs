﻿using Microsoft.AspNet.Authentication;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Odachi.AspNet.Authentication.Basic
{
	public class BasicCredential
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public BasicCredentialClaim[] Claims { get; set; } = new BasicCredentialClaim[0];
	}

	public class BasicCredentialClaim
	{
		public string Type { get; set; }
		public string Value { get; set; }
	}
}