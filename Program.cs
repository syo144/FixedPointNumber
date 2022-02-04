using System;

namespace Karkinosware {
    class FixedPointNumber {
        private const int INTEGER_DIGIT = 32;
        private const int DIGIT = 15; //符号1bit,整数部16bit,小数部15bit
        private int num;

        public FixedPointNumber(){}
        public FixedPointNumber(decimal num) {
            if(num >= (int)Math.Pow(2.0, INTEGER_DIGIT - DIGIT - 1)){
                Console.WriteLine("入力値は整数部分の上限は65535です");
                return;
            }
            this.num = decimal.ToInt32(Math.Round(num * (1<<DIGIT)));
        }

        public static FixedPointNumber operator+ (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num + (long)y.num;
            z.num = (int)zNum;
            return z;
        }
        public static FixedPointNumber operator- (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            z.num = x.num - y.num;
            return z;
        }
        public static FixedPointNumber operator* (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num * (long)y.num;
            z.num = (int)(zNum >> DIGIT);
            return z;
        }
        public static FixedPointNumber operator/ (FixedPointNumber x, FixedPointNumber y) {
            FixedPointNumber z = new FixedPointNumber();
            long zNum = (long)x.num / (long)y.num;
            z.num = (int)(zNum << DIGIT);
            return z;
        }

        public void PrintValue() {
            decimal num = this.num;
            num /= (int)Math.Pow(2.0, DIGIT);
            Console.WriteLine(num);
        }

        //2進数表記にする
        public void BinaryPrintValue(){
            var str = Convert.ToString(this.num, toBase: 2);
            Console.WriteLine(str.PadLeft(sizeof(int) * 8, '0'));
        }        
    }
}

class Program {
    static void Main(string[] args) {
        
        //入力して、固定小数に変換
        Karkinosware.FixedPointNumber xFP = new Karkinosware.FixedPointNumber(6000m);
        Karkinosware.FixedPointNumber yFP = new Karkinosware.FixedPointNumber(10m);
        
        //処理結果
        xFP.PrintValue();
        xFP.BinaryPrintValue();
        yFP.PrintValue();
        yFP.BinaryPrintValue();
/*
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("///// 加算 /////");
        Karkinosware.FixedPointNumber addFP = xFP + yFP;
        addFP.PrintValue();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("///// 減算 /////");
        Karkinosware.FixedPointNumber subFP = xFP - yFP;
        subFP.PrintValue();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("///// 乗算 /////");
        Karkinosware.FixedPointNumber mulFP = xFP * yFP;
        mulFP.PrintValue();
*/
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("///// 除算 /////");
        Karkinosware.FixedPointNumber divFP = xFP / yFP;
        divFP.PrintValue();

        Console.ForegroundColor = ConsoleColor.White;
    }
}
