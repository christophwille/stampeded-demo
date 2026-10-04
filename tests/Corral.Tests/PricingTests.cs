using NUnit.Framework;

namespace Corral.Tests;

public class PricingTests
{
	[Test]
	public void PriceFor_AnAnimal_IsItsWeightTimesThePrice()
	{
		var pricing = new Pricing(4m);
		Assert.That(pricing.PriceFor(new Animal("0101", "LZY", 500)), Is.EqualTo(2000m));
	}

	[Test]
	public void PriceFor_AHerd_IsTheSumOfItsAnimals()
	{
		var herd = new Herd();
		herd.RegisterBrand("LZY", "Lazy Y Ranch");
		herd.Add(new Animal("0101", "LZY", 500));
		herd.Add(new Animal("0102", "LZY", 300));
		Assert.That(new Pricing(4m).PriceFor(herd), Is.EqualTo(3200m));
	}

	[Test]
	public void FlatPrice_IgnoresTheWeight()
	{
		var pricing = new Pricing(4m);
		Assert.That(pricing.FlatPrice(new Animal("0101", "LZY", 700)), Is.EqualTo(1800m));
	}
}
