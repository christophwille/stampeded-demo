using NUnit.Framework;

namespace Corral.Tests;

public class HerdTests
{
	static Herd Sample()
	{
		var brands = new BrandRegistry();
		brands.Register("LZY", "Lazy Y Ranch");
		brands.Register("B7", "Bar Seven");
		var herd = new Herd(brands);
		herd.Add(new Animal("0101", "LZY", 500));
		herd.Add(new Animal("0102", "LZY", 300));
		herd.Add(new Animal("0201", "B7", 450));
		return herd;
	}

	[Test]
	public void Add_RefusesAnUnregisteredBrand()
	{
		var herd = new Herd(new BrandRegistry());
		Assert.That(() => herd.Add(new Animal("0001", "XX", 400)), Throws.InvalidOperationException);
	}

	[Test]
	public void TotalWeight_SumsEveryAnimal()
	{
		Assert.That(Sample().TotalWeight(), Is.EqualTo(1250));
	}

	[Test]
	public void ByBrand_IgnoresCase()
	{
		Assert.That(Sample().ByBrand("lzy").Select(a => a.Tag), Is.EqualTo(new[] { "0101", "0102" }));
	}

	[TestCase("B7", true)]
	[TestCase("LZY", true)]
	[TestCase("77", false)]
	[TestCase("B", false)]
	[TestCase("lzy", false)]
	[TestCase("TOOLONG", false)]
	public void IsValidBrand_WantsTwoToFourCapitalsOrDigits(string brand, bool valid)
	{
		Assert.That(Herd.IsValidBrand(brand), Is.EqualTo(valid));
	}
}
