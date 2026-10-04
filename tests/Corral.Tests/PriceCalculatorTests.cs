using NUnit.Framework;

namespace Corral.Tests;

public class PriceCalculatorTests
{
	[Test]
	public void PriceFor_AnAnimal_IsItsWeightTimesThePrice()
	{
		var pricing = new PriceCalculator(4m);
		Assert.That(pricing.PriceFor(new Animal("0101", "LZY", 500)), Is.EqualTo(2000m));
	}

	[Test]
	public void PriceFor_AHerd_IsTheSumOfItsAnimals()
	{
		var herd = new Herd();
		herd.RegisterBrand("LZY", "Lazy Y Ranch");
		herd.Add(new Animal("0101", "LZY", 500));
		herd.Add(new Animal("0102", "LZY", 300));
		Assert.That(new PriceCalculator(4m).PriceFor(herd), Is.EqualTo(3200m));
	}

	[Test]
	public void FlatPrice_IgnoresTheWeight()
	{
		var pricing = new PriceCalculator(4m);
		Assert.That(pricing.FlatPrice(new Animal("0101", "LZY", 700)), Is.EqualTo(1800m));
	}
}
