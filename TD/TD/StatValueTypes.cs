using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    struct IntStat
    {
        public int BaseValue { get; set; }
        public int FlatModifier { get; set; }
        public float PercentageModifier { get; set; }

        public int Value
        {
            get { return (int)((BaseValue + FlatModifier) * PercentageModifier); }
        }

        public IntStat(int value)
            : this()
        {
            BaseValue = value;
            PercentageModifier = 1.0f;
        }

        public static implicit operator int(IntStat intStat)
        {
            return intStat.Value;
        }

        public static implicit operator IntStat(int i)
        {
            return new IntStat(i);
        }
    }

    struct FloatStat
    {
        public float BaseValue { get; set; }
        public float FlatModifier { get; set; }
        public float PercentageModifier { get; set; }

        public int Value
        {
            get { return (int)((BaseValue + FlatModifier) * PercentageModifier); }
        }

        public FloatStat(float value)
            : this()
        {
            BaseValue = value;
            PercentageModifier = 1.0f;
        }

        public static implicit operator float(FloatStat intStat)
        {
            return intStat.Value;
        }

        public static implicit operator FloatStat(float f)
        {
            return new FloatStat(f);
        }
    }
}
