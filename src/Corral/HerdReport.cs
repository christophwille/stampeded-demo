using System.Globalization;
using System.Text;

using Humanizer;

namespace Corral;

/// <summary>A herd as text, for the command line.</summary>
public static class HerdReport
{
	/// <summary>
	/// One line for the herd, one per brand, and the total. With <paramref name="weighed"/>
	/// off, every animal counts at the flat price.
	/// </summary>
	public static string Summarize(Herd herd, Pricing pricing, bool weighed = true)
	{
		var text = new StringBuilder();
		text.AppendLine(Line($"{"animal".ToQuantity(herd.Animals.Count)}, {herd.TotalWeight():0} kg"));
		foreach (var brand in herd.Animals.GroupBy(a => a.Brand).OrderBy(g => g.Key))
		{
			decimal value = brand.Sum(a => weighed ? pricing.PriceFor(a) : pricing.FlatPrice(a));
			text.AppendLine(Line($"  {brand.Key} ({herd.Brands.OwnerOf(brand.Key)}): {brand.Count()} head, {value:0.00}"));
		}
		text.Append(Line($"total {pricing.PriceFor(herd):0.00}"));
		return text.ToString();
	}

	static string Line(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
