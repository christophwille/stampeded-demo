namespace Corral;

/// <summary>Who owns which brand.</summary>
public interface IBrandRegistry
{
	/// <summary>Says who owns a brand. Registering it again changes the owner.</summary>
	void Register(string brand, string owner);

	string? OwnerOf(string brand);

	bool IsRegistered(string brand);
}

/// <summary>A registry kept in memory, which is all one ranch needs.</summary>
public sealed class BrandRegistry : IBrandRegistry
{
	readonly Dictionary<string, string> owners = new(StringComparer.OrdinalIgnoreCase);

	public void Register(string brand, string owner)
	{
		if (!Herd.IsValidBrand(brand))
			throw new ArgumentException($"'{brand}' is not a valid brand.", nameof(brand));
		if (string.IsNullOrWhiteSpace(owner))
			throw new ArgumentException("A brand belongs to somebody.", nameof(owner));
		owners[brand] = owner;
	}

	public string? OwnerOf(string brand) => owners.GetValueOrDefault(brand);

	public bool IsRegistered(string brand) => owners.ContainsKey(brand);
}
