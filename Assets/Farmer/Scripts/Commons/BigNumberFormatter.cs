using System;

public static class BigNumberFormatter
{
    private static readonly string[] Suffixes =
    {
        "",
        "K",
        "M",
        "B",
        "T",
        "Qa",
        "Qi",
        "Sx",
        "Sp",
        "Oc",
        "No",
        "Dc"
    };

    public static string Format(BigNumber value)
    {
        if (value.Mantissa == 0)
            return "0";

        long group = value.Exponent / 3;

        if (group <= 0)
        {
            double number =
                value.Mantissa *
                Math.Pow(10, value.Exponent);

            return number.ToString("0.##");
        }

        long exponentOffset = value.Exponent % 3;

        double displayValue =
            value.Mantissa *
            Math.Pow(10, exponentOffset);

        if (group < Suffixes.Length)
        {
            return $"{displayValue:0.##}{Suffixes[group]}";
        }

        return $"{value.Mantissa:0.##}e{value.Exponent}";
    }
}