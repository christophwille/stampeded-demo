namespace Corral;

/// <summary>The animals on one ranch, and who owns each brand among them.</summary>
public sealed class Herd
{
	readonly List<Animal> animals = [];
	readonly Dictionary<string, string> owners = new(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyList<Animal> Animals => animals;

	/// <summary>Adds an animal. Its brand has to be registered first.</summary>
	public void Add(Animal animal)
	{
		if (!owners.ContainsKey(animal.Brand))
			throw new InvalidOperationException($"Brand '{animal.Brand}' is not registered.");
		animals.Add(animal);
	}

	/// <summary>A brand is two to four capital letters or digits, e.g. "B7" or "LZY".</summary>
	public static bool IsValidBrand(string brand)
		=> brand.Length is >= 2 and <= 4 && brand.All(c => char.IsAsciiLetterUpper(c) || char.IsAsciiDigit(c));

	/// <summary>Says who owns a brand. Registering it again changes the owner.</summary>
	public void RegisterBrand(string brand, string owner)
	{
		if (!IsValidBrand(brand))
			throw new ArgumentException($"'{brand}' is not a valid brand.", nameof(brand));
		owners[brand] = owner;
	}

	public string? OwnerOf(string brand) => owners.GetValueOrDefault(brand);

	public double TotalWeight() => animals.Sum(a => a.WeightKg);

	public IEnumerable<Animal> ByBrand(string brand)
		=> animals.Where(a => a.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase))
			.OrderBy(a => a.Tag, StringComparer.Ordinal);
}
