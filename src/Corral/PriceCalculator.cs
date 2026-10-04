namespace Corral;

/// <summary>Turns weight into money.</summary>
public sealed class PriceCalculator(decimal pricePerKg)
{
	public decimal PricePerKg { get; } = pricePerKg;

	/// <summary>
	/// Which class an animal sells in. Buyers pay less per kilogram for a light animal and
	/// more for a heavy one.
	/// </summary>
	public static WeightClass ClassOf(Animal animal) => animal.WeightKg switch {
		< 350 => WeightClass.Light,
		< 600 => WeightClass.Standard,
		_ => WeightClass.Heavy,
	};

	static decimal Factor(WeightClass weightClass) => weightClass switch {
		WeightClass.Light => 0.9m,
		WeightClass.Heavy => 1.1m,
		_ => 1m,
	};

	/// <summary>The price of one animal: its weight, at what its class fetches per kilogram.</summary>
	public decimal PriceFor(Animal animal)
		=> Math.Round((decimal)animal.WeightKg * PricePerKg * Factor(ClassOf(animal)), 2);

	/// <summary>The price of every animal in a herd.</summary>
	public decimal PriceFor(Herd herd) => herd.Animals.Sum(PriceFor);
}
