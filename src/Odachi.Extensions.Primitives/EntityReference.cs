using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Odachi.Abstractions;

namespace Odachi.Extensions.Primitives
{
	[Obsolete("Entity reference concept is obsolete, see `Odachi.Abstractions.IEntityReference`. Will be removed in next major version.")]
	public class EntityReference : IEntityReference
	{
		public EntityReference(int id)
		{
			Id = id;
		}

		public int Id { get; }
	}
}
