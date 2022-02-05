using System;
using System.Text.RegularExpressions;

namespace Karkinosware {
    /* 
     *「123.456 + 123.456」や「0.99 / 10」のように入力したら浮動小数点に変換され計算結果が出力されます。
     * 入力値の整数部分の上限は65535です。
     * 
    */
    class FixedPointNumber {
        private const int INTEGER_DIGIT = 32;
        private const int DIGIT = 15; //符号1bit,整数部16bit,小数部15bit
        private int num;

        public FixedPointNumber(){}
        public FixedPointNumber(string str){
            str = Regex.Replace(str, " ", "");
            bool isMatch = Regex.IsMatch(str, "^[-]?[0-9]+[.]?[0-9]*[/+-/*///%][-]?[0-9]+[.]?[0-9]*$");
            if(isMatch){
                string[] numStr = new string[2];
                numStr[0] = Regex.Match(str, "^[-]?[0-9]+[.]?[0-9]*").Value;
                str = Regex.Replace(str,"^[-]?[0-9]+[.]?[0-9]*","");
                string calcStr = Regex.Match(str, "^[/+-/*///%]").Value;
                str = Regex.Replace(str,"^[/+-/*///%]","");
                numStr[1] = Regex.Match(str, "[-]?[0-9]+[.]?[0-9]*").Value;
                //Console.WriteLine("calcStr:"+calcStr+", num1:"+numStr[0] + ", num2:"+numStr[1]);
                
                FixedPointNumber x = new FixedPointNumber(Convert.ToDecimal(numStr[0]));
                FixedPointNumber y = new FixedPointNumber(Convert.ToDecimal(numStr[1]));
                switch (calcStr)
                {
                    case "+":
                        this.num = (x+y).num;
                        break;
                    case "-":
                        this.num = (x-y).num;
                        break;
                    case "*":
                        this.num = (x*y).num;
                        break;
                    case "/":
                        this.num = (x/y).num;
                        break;
                    case "%":
                        this.num = (x%y).num;
                        break;
                    default:
                        break;
                }
            } else {
                Console.WriteLine("入力形式が違います。");
            }
        }
        public FixedPointNumber(decimal num) {
            if(num >= (int)Math.Pow(2.0, INTEGER_DIGIT - DIGIT - 1)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("入力値の整数部分の上限は65535です");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            this.num = decimal.ToInt32(Math.Round(num * (1<<DIGIT)));
        }

        public static FixedPointNumber operator+ (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num + (long)y.num;
            if(zNum > ((long)1<<32) || ((GetSignBit(x.num) ^ GetSignBit(y.num)) == 0 && (GetSignBit(x.num) ^ GetSignBit(zNum)) == 1)){
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
            if(zNum > ((long)1<<32) || ((GetSignBit(x.num) ^ GetSignBit(y.num)) == 1 && (GetSignBit(x.num) ^ GetSignBit(zNum)) == 1)){
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
            if(zNum > ((long)1<<32) || (GetSignBit(x.num) ^ GetSignBit(y.num)) != GetSignBit(zNum)){
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
            long zNum = (long)x.num << DIGIT;
            zNum = zNum / (long)y.num;
            if(zNum > ((long)1<<32) || (GetSignBit(x.num) ^ GetSignBit(y.num)) != GetSignBit(zNum)){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("除算の途中でオーバーフローが発生しました。");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                z.num = (int)zNum;
            }
            return z;
        }
        public static FixedPointNumber operator% (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long quotient = (long)x.num / (long)y.num;
            z.num = (int)((long)x.num - quotient * (long)y.num);
            return z;
        }

        private static int GetSignBit(int num){
            int msb = num >> (INTEGER_DIGIT-1) & 1;
            return msb;
        }private static int GetSignBit(long num){
            int msb = (int)(num >> (INTEGER_DIGIT-1) & 1);
            return msb;
        }

        //10進数表記の文字列に変換
        public string ToStringValue() {
            decimal num = this.num;
            num /= (int)Math.Pow(2.0, DIGIT);
            return Convert.ToString(num);
        }

        //2進数表記の文字列に変換
        public string BinaryToStringValue(){
            var str = Convert.ToString(this.num, toBase:2);
            str = str.PadLeft(sizeof(int) * 8, '0');
            return str;
        }        
    }

    class Program {
        static void Main(string[] args) {
            FixedPointNumber x;
            FixedPointNumber y;

            var str = System.Console.ReadLine();
            if(str != null){
                x = new FixedPointNumber(str);
            } else {
                x = new FixedPointNumber(0m);
                Console.WriteLine("nullです");
            }
            Console.WriteLine(x.ToStringValue());

            /*
            x = new FixedPointNumber(10000m);
            y = new FixedPointNumber(0.5m);
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");

            x = new FixedPointNumber(65535m);
            y = new FixedPointNumber(-65535m);
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");

            x = new FixedPointNumber(0.5m);
            y = new FixedPointNumber(0.5m);
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");

            x = new FixedPointNumber(0.1m);
            y = new FixedPointNumber(0.1m);
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");

            x = new FixedPointNumber(65535m);
            y = new FixedPointNumber(-2m);
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");

            x = new FixedPointNumber(65535m);
            y = new FixedPointNumber(-0.5m);
            Console.WriteLine($"{x.ToStringValue()} + {y.ToStringValue()} = {(x + y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} - {y.ToStringValue()} = {(x - y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} * {y.ToStringValue()} = {(x * y).ToStringValue()}");
            Console.WriteLine($"{x.ToStringValue()} / {y.ToStringValue()} = {(x / y).ToStringValue()}");
            */
        }
    }
}