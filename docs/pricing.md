# Pricing

An animal sells by weight, at what its **weight class** fetches per kilogram:

| Class | Weight | Per kilogram |
| --- | --- | --- |
| Light | up to 350 kg | 90 % of the price |
| Standard | 350 to 600 kg | the price |
| Heavy | above 600 kg | 110 % of the price |

    price = weight * pricePerKg * factor(class)

rounded to two places. A herd is worth the sum of its animals, and `corral` prints how many
animals of each brand fall into each class.
