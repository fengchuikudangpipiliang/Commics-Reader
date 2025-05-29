namespace 小说漫画阅读器
{
    public class ChineseNumberConverter
    {
        private static readonly string[] ChineseDigits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        private static readonly string[] Units = { "", "十", "百", "千" };

        public static string ToChinese(int number)
        {
            if (number == 0) return ChineseDigits[0];
            if (number < 0) return "负" + ToChinese(-number);

            var str = number.ToString();
            int length = str.Length;
            string result = "";
            bool lastZero = false;

            for (int i = 0; i < length; i++)
            {
                int digit = str[i] - '0';
                int unitIndex = length - i - 1;

                if (digit != 0)
                {
                    if (lastZero)
                    {
                        result += "零";
                        lastZero = false;
                    }

                    if (!(digit == 1 && unitIndex == 1 && result == "")) // 特例：10应该是“十”，而不是“一十”
                        result += ChineseDigits[digit];

                    result += Units[unitIndex];
                }
                else
                {
                    lastZero = true;
                }
            }

            return result;
        }
    }
}
