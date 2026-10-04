namespace Corral;

/// <summary>The animals on one ranch, and who owns each brand among them.</summary>
public sealed class Herd
{
	readonly List<Animal> animals = [];
	readonly Dictionary<string, string> owners = new(StringComparer.OrdinalIgnoreCase);
	double? totalWeight;

	public IReadOnlyList<Animal> Animals => animals;

	/// <summary>Adds an animal. Its brand has to be registered first.</summary>
	public void Add(Animal animal)
	{
		if (!owners.ContainsKey(animal.Brand))
			throw new InvalidOperationException($"Brand '{animal.Brand}' is not registered.");
		animals.Add(animal);
		totalWeight = null;
	}

	/// <summary>Takes an animal out of the herd. False when no animal carries that tag.</summary>
	public bool Remove(string tag)
	{
		return animals.RemoveAll(a => a.Tag == tag) > 0;
	}

	/// <summary>Empties the herd, as after a sale of the whole ranch. The brands stay.</summary>
	public void Clear()
	{
		if (animals.Count == 0)
			return;
		animals.Clear();
		totalWeight = null;
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

	/// <summary>
	/// The weight of the whole herd. Summed once and kept until the herd changes: the report
	/// asks for it per line.
	/// </summary>
	public double TotalWeight() => totalWeight ??= animals.Sum(a => a.WeightKg);

	public IEnumerable<Animal> ByBrand(string brand)
		=> animals.Where(a => a.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));

	/// <summary>The animals in their second year, which is when they are usually sold.</summary>
	public IEnumerable<Animal> Yearlings(DateOnly today)
		=> animals.Where(a => a.AgeInYears(today) == 1);
}
