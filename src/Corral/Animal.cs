namespace Corral;

/// <summary>One head of cattle, identified by its ear tag.</summary>
public sealed record Animal(string Tag, string Brand, double WeightKg)
{
	/// <summary>The day it was born, where the ranch wrote that down.</summary>
	public DateOnly? Born { get; init; }

	/// <summary>Full years lived by a given day, or null for an animal of unknown age.</summary>
	public int? AgeInYears(DateOnly today)
	{
		if (Born is not { } born)
			return null;
		int years = today.Year - born.Year;
		return born.AddYears(years) > today ? years - 1 : years;
	}
}
