namespace Corral;

/// <summary>The animals on one ranch.</summary>
public sealed class Herd(IBrandRegistry brands)
{
	readonly List<Animal> animals = [];

	public IReadOnlyList<Animal> Animals => animals;

	/// <summary>Who owns the brands these animals carry.</summary>
	public IBrandRegistry Brands => brands;

	/// <summary>Adds an animal. Its brand has to be registered first.</summary>
	public void Add(Animal animal)
	{
		if (!brands.IsRegistered(animal.Brand))
			throw new InvalidOperationException($"Brand '{animal.Brand}' is not registered.");
		animals.Add(animal);
	}

	public double TotalWeight() => animals.Sum(a => a.WeightKg);

	public IEnumerable<Animal> ByBrand(string brand)
		=> animals.Where(a => a.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));

	/// <summary>The animals in their second year, which is when they are usually sold.</summary>
	public IEnumerable<Animal> Yearlings(DateOnly today)
		=> animals.Where(a => a.AgeInYears(today) == 1);

	/// <summary>
	/// A brand is two to four capital letters or digits, with a letter among them: "B7" or
	/// "LZY", but not "77".
	/// </summary>
	public static bool IsValidBrand(string brand)
	{
		if (brand.Length is < 2 or > 4)
			return false;
		if (brand.All(char.IsAsciiDigit))
			return false;
		return brand.All(c => char.IsAsciiLetterUpper(c) || char.IsAsciiDigit(c));
	}
}
