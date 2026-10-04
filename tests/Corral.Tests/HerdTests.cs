using NUnit.Framework;

namespace Corral.Tests;

public class HerdTests
{
	static Herd Sample()
	{
		var herd = new Herd();
		herd.RegisterBrand("LZY", "Lazy Y Ranch");
		herd.RegisterBrand("B7", "Bar Seven");
		herd.Add(new Animal("0101", "LZY", 500));
		herd.Add(new Animal("0102", "LZY", 300));
		herd.Add(new Animal("0201", "B7", 450));
		return herd;
	}

	[Test]
	public void Add_RefusesAnUnregisteredBrand()
	{
		var herd = new Herd();
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
	[TestCase("B", false)]
	[TestCase("lzy", false)]
	[TestCase("TOOLONG", false)]
	public void IsValidBrand_WantsTwoToFourCapitalsOrDigits(string brand, bool valid)
	{
		Assert.That(Herd.IsValidBrand(brand), Is.EqualTo(valid));
	}

	[Test]
	public void RegisterBrand_AgainChangesTheOwner()
	{
		var herd = Sample();
		herd.RegisterBrand("B7", "Bar Seven Cattle Co.");
		Assert.That(herd.OwnerOf("B7"), Is.EqualTo("Bar Seven Cattle Co."));
	}
}
