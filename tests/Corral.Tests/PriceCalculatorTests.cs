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

	[TestCase(300, 1080)]
	[TestCase(700, 3080)]
	public void PriceFor_AnAnimal_FollowsItsWeightClass(double weight, decimal expected)
	{
		var pricing = new PriceCalculator(4m);
		Assert.That(pricing.PriceFor(new Animal("0101", "LZY", weight)), Is.EqualTo(expected));
	}

	[TestCase(349.9, WeightClass.Light)]
	[TestCase(599.9, WeightClass.Standard)]
	[TestCase(600, WeightClass.Heavy)]
	public void ClassOf_SortsByWeight(double weight, WeightClass expected)
	{
		Assert.That(PriceCalculator.ClassOf(new Animal("0101", "LZY", weight)), Is.EqualTo(expected));
	}

	[Test]
	public void PriceFor_AHerd_IsTheSumOfItsAnimals()
	{
		var herd = new Herd();
		herd.RegisterBrand("LZY", "Lazy Y Ranch");
		herd.Add(new Animal("0101", "LZY", 500));
		herd.Add(new Animal("0102", "LZY", 300));
		Assert.That(new PriceCalculator(4m).PriceFor(herd), Is.EqualTo(3080m));
	}
}
