using System;

namespace Incapsulation.RationalNumbers
{
    public struct Rational
    {
        private readonly int _numerator;
        private readonly int _denominator;

        public Rational(int numerator, int denominator = 1)
        {
            IsNan = denominator == 0;
            _numerator = numerator;
            _denominator = numerator == 0 ? 1 : denominator;
        }

        public bool IsNan { get; }

        private int Divisor
        {
            get
            {
                if (_numerator == 0 || _numerator == 1 || _denominator == 1)
                    return 1;

                return _numerator < 0 && _denominator > 0 ? GetMaxCommonDivisor(_numerator, _denominator) :
                    _numerator > 0 && _denominator < 0 ? -GetMaxCommonDivisor(_numerator, _denominator) :
                    _numerator < 0 && _denominator < 0 ? -GetMaxCommonDivisor(_numerator, _denominator) :
                    _numerator > 0 && _denominator > 0 ? GetMaxCommonDivisor(_numerator, _denominator) :
                    -GetMaxCommonDivisor(_numerator, _denominator);
            }
        }

        private static int GetMaxCommonDivisor(int num, int den)
        {
            num = Math.Abs(num);
            den = Math.Abs(den);

            while (num != den)
            {
                if (num > den)
                    num -= den;
                else
                    den -= num;
            }
            return num;
        }

        public int Numerator
        {
            get => Divisor == 1 ? _numerator : _numerator / Divisor;
            private set => Numerator = _numerator;
        }

        public int Denominator
        {
            get => Divisor == 1 ? _denominator : _denominator / Divisor;
            private set => Denominator = _denominator;
        }

        public static Rational operator +(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan) return new Rational(1, 0);

            if (a.Denominator == b.Denominator)
                return new Rational(a.Numerator + b.Numerator, a.Denominator);

            (int a, int b) multipliers;
            if (a.Denominator < b.Denominator)
            {
                if (b.Denominator % a.Denominator == 0)
                    return new Rational(a._numerator * (b.Denominator / a.Denominator) + b._numerator, b._denominator);

                multipliers = GetMultipliers(a.Denominator, b.Denominator);
                a *= multipliers.a;
                b *= multipliers.b;
                return new Rational(a._numerator + b._numerator, a._denominator);
            }

            if (a.Denominator % b.Denominator == 0)
                return new Rational(a._numerator + b._numerator * (a.Denominator / b.Denominator), a._denominator);

            multipliers = GetMultipliers(b.Denominator, a.Denominator);
            a *= multipliers.b;
            b *= multipliers.a;
            return new Rational(a._numerator + b._numerator, a._denominator);
        }

        public static Rational operator -(Rational a, Rational b)
        {
            return a + new Rational(-b.Numerator, b.Denominator);
        }

        public static Rational operator *(Rational rational, int x)
        {
            return new Rational(rational.Numerator * x, rational.Denominator * x);
        }

        public static Rational operator *(int x, Rational rational)
        {
            return rational * x;
        }

        public static Rational operator *(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan) return new Rational(1, 0);

            return new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
        }

        public static Rational operator /(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan || b.Denominator == 0) return new Rational(1, 0);

            return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        public static implicit operator double(Rational rational)
        {
            if (rational.IsNan || rational.Denominator == 0)
                return double.NaN;
            return (double) rational.Numerator / rational.Denominator;
        }

        public static implicit operator Rational(int x)
        {
            return x == 0 ? new Rational(0, 1) : new Rational(x, 1);
        }

        public static explicit operator int(Rational rational)
        {
            if (rational.IsNan || rational.Numerator % rational.Denominator != 0)
                throw new ArgumentException();
            return rational.Numerator / rational.Denominator;
        }

        private static (int, int) GetMultipliers(int x, int y)
        {
            return y % x == 0 ? (y / x, y) : (y, x);
        }
    }
}