﻿namespace CodeBase.Services.Randomizer
{
	public interface IRandomService : IService
	{
	int Next(int min, int max);
	}
}
