# Pricing

An animal sells by weight: its weight in kilograms times the price per kilogram, rounded to
two places.

    price = weight * pricePerKg

An animal nobody weighed sells at the **flat price**, which is the price of a 450 kg animal.
`corral --flat` prices the whole herd that way.

A herd is worth the sum of its animals.
