using System.Globalization;

using Corral;

if (args.Contains("--help"))
{
	Console.WriteLine("Prints the sample herd and what it is woth.");
	Console.WriteLine();
	Console.WriteLine("corral [--flat] [--price <per kg>]");
	Console.WriteLine("  --flat    price every animal as if it weighed 450 kg");
	Console.WriteLine("  --price   price per kilogram, default 4.20");
	return;
}

decimal price = 4.20m;
int at = Array.IndexOf(args, "--price");
if (at >= 0 && at + 1 < args.Length)
	price = decimal.Parse(args[at + 1], CultureInfo.InvariantCulture);

var herd = new Herd();
herd.RegisterBrand("LZY", "Lazy Y Ranch");
herd.RegisterBrand("B7", "Bar Seven");
herd.Add(new Animal("0101", "LZY", 512));
herd.Add(new Animal("0102", "LZY", 338));
herd.Add(new Animal("0103", "LZY", 641));
herd.Add(new Animal("0201", "B7", 455));
herd.Add(new Animal("0202", "B7", 297));

Console.WriteLine(HerdReport.Summarize(herd, new Pricing(price), weighed: !args.Contains("--flat")));
