namespace Corral;

/// <summary>Turns weight into money.</summary>
public sealed class Pricing(decimal pricePerKg)
{
	public decimal PricePerKg { get; } = pricePerKg;

	/// <summary>The price of one animal by its weight.</summary>
	public decimal PriceFor(Animal animal) => Math.Round((decimal)animal.WeightKg * PricePerKg, 2);

	/// <summary>The price of every animal in a herd.</summary>
	public decimal PriceFor(Herd herd) => herd.Animals.Sum(PriceFor);

	/// <summary>
	/// What an animal sells for when nobody weighed it: the price of a 450 kg animal.
	/// </summary>
	public decimal FlatPrice(Animal animal) => Math.Round(450m * PricePerKg, 2);
}
