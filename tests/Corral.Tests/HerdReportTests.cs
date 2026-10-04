using NUnit.Framework;

namespace Corral.Tests;

public class HerdReportTests
{
	[Test]
	public void Summarize_ListsBrandsInOrderAndTheTotal()
	{
		var brands = new BrandRegistry();
		brands.Register("LZY", "Lazy Y Ranch");
		brands.Register("B7", "Bar Seven");
		var herd = new Herd(brands);
		herd.Add(new Animal("0101", "LZY", 500));
		herd.Add(new Animal("0201", "B7", 450));

		string[] lines = HerdReport.Summarize(herd, new Pricing(4m)).Split(Environment.NewLine);

		Assert.That(lines, Is.EqualTo(new[] {
			"2 animals, 950 kg",
			"  B7 (Bar Seven): 1 head, 1800.00",
			"  LZY (Lazy Y Ranch): 1 head, 2000.00",
			"total 3800.00",
		}));
	}
}
