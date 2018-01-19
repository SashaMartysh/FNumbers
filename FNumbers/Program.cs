using System;
using System.Text;

// Этот проэкт посвящен работе с большими числами.
namespace FNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\tThis is an example.\n");
            Random rnd = new Random();
            int i1 = rnd.Next(0, int.MaxValue);
            int i2 = rnd.Next(0, int.MaxValue);
            FNumber f1 = new FNumber(i1); 
            FNumber f2 = new FNumber(i2);

            Console.WriteLine("First number is: " + f1.ToString() + " (" + i1.ToString("N") + ")");
            Console.WriteLine("Second number is: " + f2.ToString() + " (" + i2.ToString("N") + ")");
            Console.WriteLine("\nTheir summa is: " + (f1 + f2));
            Console.WriteLine("Their subtraction is: " + (f1 - f2));
            int random  = new Random().Next(0, 1000);
            Console.WriteLine("Also " + f1 + " / " + random + " = " + (f1/random));
            Console.WriteLine(f1 + " > " + f2 + " : " + (f1>f2));

            Console.ReadKey();
        }
    }

    // "Fantastic Number" is used for operate such numbers: 0.05, 500, 2K, 3.5M, 500B etc...
    public class FNumber
    {
        public float value { get; private set; }
        public Rank rank { get; private set; }

        public FNumber() : this (0f, Rank.units)
        { }

        public FNumber (float value) : this (value, Rank.units)
        { }

        public FNumber(int value) : this(value, Rank.units)
        { }

        public FNumber(long value) : this(value, Rank.units)
        { }

        public FNumber(float value, Rank rank)
        {
            this.value = value;
            this.rank = rank;
            Refresh();
        }

        // Обновляет число, чтобы числовое значение было не больше 999 и не менее 1.
        private void Refresh()
        {
            while (Math.Abs(this.value) > 1000f && (int)this.rank < 8)
            {
                this.value /= 1000f;
                this.rank++;
            }
            while (Math.Abs(this.value) < 1.0f && this.rank > 0)
            {
                this.value *= 1000f;
                this.rank--;
            }
        }

        // Показывает лишь три значащие знака в числе.
        public override string ToString()
        {
            Refresh();
            StringBuilder strBuilder = new StringBuilder();
            if (Math.Abs(value) >= 100f)
                strBuilder.Append(value.ToString("###."));
            else if (Math.Abs(value) >= 10f)
                strBuilder.Append(value.ToString("##.#"));
            else
                strBuilder.Append(value.ToString("0.##"));
            if (rank > 0)
                strBuilder.Append(rank);
            return strBuilder.ToString();
        }

        public float ToFloat()
        {
            while (rank > 0)
            {
                value *= 1000f;
                rank--;
            }
            return value;
        }
        public int ToInt()
        {
            while (rank > 0)
            {
                value *= 1000f;
                rank--;
            }
            return (int)value;
        }
        public long ToLong()
        {
            while (rank > 0)
            {
                value *= 1000f;
                rank--;
            }
            return (long)value;
        }

        // Операция сложения. 
        public static FNumber operator + (FNumber fn1, FNumber fn2)
        {
            // Нахожу, какое из числ большее, чтобы к нему потом прибавить меньшее.
            FNumber min;
            FNumber max;
            if ((int)fn2.rank > (int)fn1.rank)
            {
                max = fn2;
                min = fn1;
            }
            else
            {
                max = fn1;
                min = fn2;
            }

            // Перевожу меньшее число к рангу большего.
            while ((int)min.rank < (int)max.rank)
            {
                min.value /= 1000f;
                min.rank++;
            }

            // Добавляю и возвращаю.
            return new FNumber(max.value + min.value, max.rank);
        }
        public static FNumber operator + (FNumber fNumber, float f)
        {
            return (fNumber + new FNumber(f, Rank.units));
        }
        public static FNumber operator + (FNumber fNumber, int i)
        {
            return (fNumber + new FNumber(i, Rank.units));
        }
        public static FNumber operator + (float f, FNumber fNumber)
        {
            return (fNumber + new FNumber(f, Rank.units));
        }
        public static FNumber operator + (int i, FNumber fNumber)
        {
            return (fNumber + new FNumber(i, Rank.units));
        }

        // Операция вычитания. 
        public static FNumber operator - (FNumber fn1, FNumber fn2)
        {
            // Нахожу, какое из чисел больше по модулю, привожу другое число к его рангу.
            FNumber maxByABS;
            FNumber minByABS;
            if (fn1.rank > fn2.rank)
            {
                maxByABS = fn1;
                minByABS = fn2;
            }
             else if (fn1.rank < fn2.rank)
             {
                 maxByABS = fn2;
                 minByABS = fn1;
             }
             else  // если они равны по рангу, то неважно.
             {
                maxByABS = fn2;
                minByABS = fn1;
            }

            // Привожу числа к ожному рангу.
            while (minByABS.rank < maxByABS.rank)
            {
                minByABS.value /= 1000f;
                minByABS.rank ++;
            }

            // Делаю вычитание.
            return new FNumber (fn1.value - fn2.value, fn1.rank);
        }
        public static FNumber operator - (FNumber fn, float f)
        {
            return (fn - new FNumber(f, Rank.units));
        }
        public static FNumber operator - (FNumber fn, int i)
        {
            return (fn - new FNumber(i, Rank.units));
        }
        public static FNumber operator - (float f, FNumber fn)
        {
            return (new FNumber(f, Rank.units) - fn);
        }
        public static FNumber operator - (int i, FNumber fn)
        {
            return (new FNumber(i, Rank.units) - fn);
        }
        public static FNumber operator - (FNumber fNumber) 
        {
            // Возвращаем с противоположным знаком.
            return new FNumber(-1 * fNumber.value, fNumber.rank);
        }

        // Оператор умножения. // Multiplication operator.
        public static FNumber operator * (FNumber fn, float f)
        {
            return new FNumber (fn.value * f, fn.rank);
        }
        public static FNumber operator * (FNumber fn, int i)
        {
            return new FNumber(fn.value * i, fn.rank);
        }
        public static FNumber operator * (float f, FNumber fn)
        {
            return new FNumber(fn.value * f, fn.rank);
        }
        public static FNumber operator * (int i, FNumber fn)
        {
            return new FNumber(fn.value * i, fn.rank);
        }

        // Оператор деления. // Division operator.
        public static FNumber operator / (FNumber fn, float f)
        {
            return new FNumber(fn.value / f, fn.rank);
        }
        public static FNumber operator / (FNumber fn, int i)
        {
            return new FNumber(fn.value / i, fn.rank);
        }
        public static float operator / (float f, FNumber fn)
        {
            // Понижаю ранг до обычных чисел.
            while (fn.rank > 0)
            {
                fn.value *= 1000f;
                fn.rank -- ;
            }

            return f/fn.value;
        }
        public static float operator / (int i, FNumber fn)
        {
            // Понижаю ранг до обычных чисел.
            while (fn.rank > 0)
            {
                fn.value *= 1000f;
                fn.rank--;
            }

            return i / fn.value;
        }
        public static float operator / (FNumber fn1, FNumber fn2)
        {
            // Нахожу, какое из числ большее, чтобы к его рангу приравнять ранг меньшего.
            FNumber min;
            FNumber max;
            if ((int)fn2.rank > (int)fn1.rank)
            {
                max = fn2;
                min = fn1;
            }
            else
            {
                max = fn1;
                min = fn2;
            }

            // Перевожу меньшее число к рангу большего.
            while ((int)min.rank < (int)max.rank)
            {
                min.value /= 1000f;
                min.rank++;
            }

            return fn1.value / fn2.value;
        }

        public static bool operator == (FNumber fNumber1, FNumber fNumber2)
        {
            if (fNumber1.rank == fNumber2.rank && fNumber1.value == fNumber2.value)
                return true;
            else return false;
        }

        public static bool operator != (FNumber fNumber1, FNumber fNumber2)
        {
            if (fNumber1.rank == fNumber2.rank && fNumber1.value == fNumber2.value)
                return false;
            else return true;
        }

        // Оператор сравнения
        public static bool operator < (FNumber fNumber1, FNumber fNumber2)
        {
            // Сравниваем по знаку.
            if (fNumber1.value < 0f && fNumber2.value > 0f)
                return true;
            if (fNumber1.value > 0f && fNumber2.value < 0f)
                return false;
            
            // Если знаки у них одинаковые, то пытаемся сравнить по рангу.
            if (fNumber1.value > 0f)
            {
                // В плюсовом диапазон высший ранг означает большее число.
                if (fNumber1.rank < fNumber2.rank)
                    return true;
                if (fNumber1.rank > fNumber2.rank)
                    return false;
            }
            else
            {
                // В отрицательном диапазон высший ранг означает меньшее число.
                if (fNumber1.rank > fNumber2.rank)
                    return true;
                if (fNumber1.rank < fNumber2.rank)
                    return false;
            }
            
            // Если ранги равны, то сравниваем по значению.
            if (fNumber1.value < fNumber2.value)
                return true;
            if (fNumber1.value > fNumber2.value)
                return false;

            return false;
        }

        // Оператор сравнения
        public static bool operator > (FNumber fNumber1, FNumber fNumber2)
        {
            // Сравниваем по знаку.
            if (fNumber1.value > 0f && fNumber2.value < 0f)
                return true;
            if (fNumber1.value < 0f && fNumber2.value > 0f)
                return false;

            // Если знаки у них одинаковые, то пытаемся сравнить по рангу.
            if (fNumber1.value > 0f)
            {
                // В плюсовом диапазон высший ранг означает большее число.
                if (fNumber1.rank > fNumber2.rank)
                    return true;
                if (fNumber1.rank < fNumber2.rank)
                    return false;
            }
            else
            {
                // В отрицательном диапазон высший ранг означает меньшее число.
                if (fNumber1.rank < fNumber2.rank)
                    return true;
                if (fNumber1.rank > fNumber2.rank)
                    return false;
            }

            // Если ранги равны, то сравниваем по значению.
            if (fNumber1.value > fNumber2.value)
                return true;
            if (fNumber1.value < fNumber2.value)
                return false;

            return false;
        }

        // Оператор сравнения
        public static bool operator <= (FNumber fNumber1, FNumber fNumber2)
        {
            // Сравниваем по знаку.
            if (fNumber1.value < 0f && fNumber2.value > 0f)
                return true;
            if (fNumber1.value > 0f && fNumber2.value < 0f)
                return false;

            // Если знаки у них одинаковые, то пытаемся сравнить по рангу.
            if (fNumber1.value > 0f)
            {
                // В плюсовом диапазон высший ранг означает большее число.
                if (fNumber1.rank < fNumber2.rank)
                    return true;
                if (fNumber1.rank > fNumber2.rank)
                    return false;
            }
            else
            {
                // В отрицательном диапазон высший ранг означает меньшее число.
                if (fNumber1.rank > fNumber2.rank)
                    return true;
                if (fNumber1.rank < fNumber2.rank)
                    return false;
            }

            // Если ранги равны, то сравниваем по значению.
            if (fNumber1.value <= fNumber2.value)
                return true;
            if (fNumber1.value > fNumber2.value)
                return false;

            return false;
        }

        // Оператор сравнения
        public static bool operator >= (FNumber fNumber1, FNumber fNumber2)
        {
            // Сравниваем по знаку.
            if (fNumber1.value > 0f && fNumber2.value < 0f)
                return true;
            if (fNumber1.value < 0f && fNumber2.value > 0f)
                return false;

            // Если знаки у них одинаковые, то пытаемся сравнить по рангу.
            if (fNumber1.value > 0f)
            {
                // В плюсовом диапазон высший ранг означает большее число.
                if (fNumber1.rank > fNumber2.rank)
                    return true;
                if (fNumber1.rank < fNumber2.rank)
                    return false;
            }
            else
            {
                // В отрицательном диапазон высший ранг означает меньшее число.
                if (fNumber1.rank < fNumber2.rank)
                    return true;
                if (fNumber1.rank > fNumber2.rank)
                    return false;
            }

            // Если ранги равны, то сравниваем по значению.
            if (fNumber1.value >= fNumber2.value)
                return true;
            if (fNumber1.value < fNumber2.value)
                return false;

            return false;
        }

        // Names of large numbers, how they are used in such countries: US, English Canadian, Australian, and modern British.
        public enum Rank
        {
            units = 0,
            K = 1, // thousands (kilos)
            M = 2, // millions      10^6
            B = 3, // billions      10^9
            T = 4, // trillions     10^12
            q = 5, // quadrillions  10^15
            Q = 6, // quintillion   10^18
            s = 7, // sextillions   10^21
            S = 8  // septillion    10^24
        }
    }
}
