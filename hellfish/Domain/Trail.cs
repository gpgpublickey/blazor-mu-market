﻿using System;
namespace hellfish.Domain
{
	public class Trail
	{
		public Trail()
		{
		}

		public int Id { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

		public string? Image { get; set; }

		public string? Location { get; set; }

		public int TimeInMinutes { get; set; }

		public string TimeFormated => $"{TimeInMinutes/60}h {TimeInMinutes%60}m";

		public int Length { get; set; }

		public IEnumerable<RouteInstruction> Route { get; set; } = Array.Empty<RouteInstruction>();
	}
}

