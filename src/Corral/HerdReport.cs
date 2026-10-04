using System.Globalization;
using System.Text;

using Humanizer;

namespace Corral;

/// <summary>A herd as text, for the command line.</summary>
public static class HerdReport
{
	/// <summary>
	/// One line for the herd, one per brand saying how many animals it has in each weight
	/// class, and the total.
	/// </summary>
	public static string Summarize(Herd herd, PriceCalculator pricing)
	{
		var text = new StringBuilder();
		text.AppendLine(Line($"{"animal".ToQuantity(herd.Animals.Count)}, {herd.TotalWeight():0} kg"));
		foreach (var brand in herd.Animals.GroupBy(a => a.Brand).OrderBy(g => g.Key))
		{
			decimal value = brand.Sum(pricing.PriceFor);
			var classes = brand.GroupBy(PriceCalculator.ClassOf)
				.OrderBy(g => g.Key)
				.Select(g => $"{g.Count()} {g.Key.Humanize(LetterCasing.LowerCase)}");
			text.AppendLine(Line($"  {brand.Key} ({herd.OwnerOf(brand.Key)}): {string.Join(", ", classes)}, {value:0.00}"));
		}
		text.Append(Line($"total {pricing.PriceFor(herd):0.00}"));
		return text.ToString();
	}

	static string Line(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
