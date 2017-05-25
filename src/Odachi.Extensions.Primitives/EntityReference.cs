using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Odachi.Abstractions;

namespace Odachi.Extensions.Primitives
{
	public class EntityReference : IEntityReference
	{
		public EntityReference(int id)
		{
			Id = id;
		}

		public int Id { get; }
	}
}
