using NUnit.Framework;

namespace Corral.Tests;

public class BrandRegistryTests
{
	[Test]
	public void Register_AgainChangesTheOwner()
	{
		var brands = new BrandRegistry();
		brands.Register("B7", "Bar Seven");
		brands.Register("B7", "Bar Seven Cattle Co.");
		Assert.That(brands.OwnerOf("B7"), Is.EqualTo("Bar Seven Cattle Co."));
	}

	[Test]
	public void Register_RefusesWhatIsNotABrand()
	{
		Assert.That(() => new BrandRegistry().Register("seven", "Bar Seven"), Throws.ArgumentException);
	}

	[Test]
	public void IsRegistered_IgnoresCase()
	{
		var brands = new BrandRegistry();
		brands.Register("LZY", "Lazy Y Ranch");
		Assert.That(brands.IsRegistered("lzy"), Is.True);
	}
}
