using System;

namespace Karkinosware {
    class FixedPointNumber {
        private const int INTEGER_DIGIT = 32;
        private const int DIGIT = 15; //符号1bit,整数部16bit,小数部15bit
        private int num;

        public FixedPointNumber(){}
        public FixedPointNumber(decimal num) {
            if(num >= (int)Math.Pow(2.0, INTEGER_DIGIT - DIGIT - 1)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("入力値は整数部分の上限は65535です");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            this.num = decimal.ToInt32(Math.Round(num * (1<<DIGIT)));
        }

        public static FixedPointNumber operator+ (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num + (long)y.num;
            if(zNum > ((long)1<<32) || ((GetMSB(x.num) ^ GetMSB(y.num)) == 0 && (GetMSB(x.num) ^ GetMSB(zNum)) == 1)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("加算の途中でオーバーフローが発生しました。");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                z.num = (int)zNum;
            }
            return z;
        }
        public static FixedPointNumber operator- (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num - (long)y.num;
            if(zNum > ((long)1<<32) || ((GetMSB(x.num) ^ GetMSB(y.num)) == 1 && (GetMSB(x.num) ^ GetMSB(zNum)) == 1)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("減算の途中でオーバーフローが発生しました。");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                z.num = (int)zNum;
            }
            return z;
        }
        public static FixedPointNumber operator* (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num * (long)y.num;
            zNum = zNum >> DIGIT;
            if(zNum > ((long)1<<32)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("乗算の途中でオーバーフローが発生しました。");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                z.num = (int)zNum;
            }
            return z;
        }
        public static FixedPointNumber operator/ (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num / (long)y.num;
            zNum = zNum << DIGIT;
            if(zNum > ((long)1<<32)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("除算の途中でオーバーフローが発生しました。");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                z.num = (int)zNum;
            }
            return z;
        }

        private static int GetMSB(int num){
            int msb = num >> (INTEGER_DIGIT-1) & 1;
            return msb;
        }private static int GetMSB(long num){
            int msb = (int)(num >> (INTEGER_DIGIT-1) & 1);
            return msb;
        }

        //10進数表記で出力
        public string ToStringValue() {
            decimal num = this.num;
            num /= (int)Math.Pow(2.0, DIGIT);
            return Convert.ToString(num);
        }

        //2進数表記で出力
        public string BinaryToStringValue(){
            var str = Convert.ToString(this.num, toBase:2);
            str = str.PadLeft(sizeof(int) * 8, '0');
            return str;
        }        
    }

    class Program {
        static void Main(string[] args) {
            FixedPointNumber x = new FixedPointNumber(65535m);
            FixedPointNumber y = new FixedPointNumber(-65535m);
            Console.WriteLine($"{x.BinaryToStringValue()}, {y.BinaryToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");
        }
    }
}