using System;
using UnityEngine;

[Serializable]
public struct BigNumber : IComparable<BigNumber>, IEquatable<BigNumber>
{
    [SerializeField] private double mantissa;
    [SerializeField] private long exponent;

    public double Mantissa => mantissa;
    public long Exponent => exponent;

    public static BigNumber Zero => new(0);
    public static BigNumber One => new(1);

    public BigNumber(double value)
    {
        mantissa = value;
        exponent = 0;

        Normalize();
    }

    public BigNumber(double mantissa, long exponent)
    {
        this.mantissa = mantissa;
        this.exponent = exponent;

        Normalize();
    }

    private void Normalize()
    {
        if (mantissa == 0)
        {
            exponent = 0;
            return;
        }

        double absMantissa = Math.Abs(mantissa);

        while (absMantissa >= 10d)
        {
            mantissa /= 10d;
            exponent++;
            absMantissa /= 10d;
        }

        while (absMantissa < 1d)
        {
            mantissa *= 10d;
            exponent--;
            absMantissa *= 10d;
        }
    }

    #region Operators

    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a.mantissa == 0)
            return b;

        if (b.mantissa == 0)
            return a;

        long exponentDifference = a.exponent - b.exponent;

        // Difference quá lớn thì số nhỏ gần như không còn ảnh hưởng.
        if (exponentDifference > 15)
            return a;

        if (exponentDifference < -15)
            return b;

        if (exponentDifference >= 0)
        {
            double adjustedB =
                b.mantissa * Math.Pow(10, -exponentDifference);

            return new BigNumber(
                a.mantissa + adjustedB,
                a.exponent);
        }
        else
        {
            double adjustedA =
                a.mantissa * Math.Pow(10, exponentDifference);

            return new BigNumber(
                adjustedA + b.mantissa,
                b.exponent);
        }
    }

    public static BigNumber operator -(BigNumber a, BigNumber b)
    {
        return a + new BigNumber(-b.mantissa, b.exponent);
    }

    public static BigNumber operator *(BigNumber a, BigNumber b)
    {
        return new BigNumber(
            a.mantissa * b.mantissa,
            a.exponent + b.exponent);
    }

    public static BigNumber operator *(BigNumber a, double multiplier)
    {
        return new BigNumber(
            a.mantissa * multiplier,
            a.exponent);
    }

    public static BigNumber operator *(double multiplier, BigNumber a)
    {
        return a * multiplier;
    }

    public static BigNumber operator /(BigNumber a, BigNumber b)
    {
        if (b.mantissa == 0)
            throw new DivideByZeroException();

        return new BigNumber(
            a.mantissa / b.mantissa,
            a.exponent - b.exponent);
    }

    public static BigNumber operator /(BigNumber a, double divider)
    {
        if (divider == 0)
            throw new DivideByZeroException();

        return new BigNumber(
            a.mantissa / divider,
            a.exponent);
    }

    #endregion

    #region Comparison

    public int CompareTo(BigNumber other)
    {
        if (mantissa >= 0 && other.mantissa < 0)
            return 1;

        if (mantissa < 0 && other.mantissa >= 0)
            return -1;

        bool negative = mantissa < 0;

        if (exponent != other.exponent)
        {
            int result = exponent.CompareTo(other.exponent);
            return negative ? -result : result;
        }

        return mantissa.CompareTo(other.mantissa);
    }

    public static bool operator >(BigNumber a, BigNumber b)
        => a.CompareTo(b) > 0;

    public static bool operator <(BigNumber a, BigNumber b)
        => a.CompareTo(b) < 0;

    public static bool operator >=(BigNumber a, BigNumber b)
        => a.CompareTo(b) >= 0;

    public static bool operator <=(BigNumber a, BigNumber b)
        => a.CompareTo(b) <= 0;

    public static bool operator ==(BigNumber a, BigNumber b)
        => a.Equals(b);

    public static bool operator !=(BigNumber a, BigNumber b)
        => !a.Equals(b);

    public bool Equals(BigNumber other)
    {
        return mantissa.Equals(other.mantissa)
               && exponent == other.exponent;
    }

    public override bool Equals(object obj)
    {
        return obj is BigNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(mantissa, exponent);
    }

    #endregion

    public bool CanAfford(BigNumber cost)
    {
        return this >= cost;
    }

    public override string ToString()
    {
        if (mantissa == 0)
            return "0";

        if (exponent < 3)
        {
            double value = mantissa * Math.Pow(10, exponent);
            return value.ToString("0.##");
        }

        return $"{mantissa:0.##}e{exponent}";
    }
}