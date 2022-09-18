using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GI.Formats
{
    public class PatternCategory
    {
        private PatternCategory(string value)
        {
            Value = value;
        }
        public string Value { get; private set; }
        public static PatternCategory Hexadecimal =>  new PatternCategory("((0(x|X)|#)[0-9a-fA-F]{1,4})");
        public static PatternCategory EndLine => new PatternCategory("(?!\\w)");
        public static PatternCategory WholeNumber => new PatternCategory("(-?\\d{1,4})");
        public static PatternCategory Double => new PatternCategory("(-?\\d*(\\.|,)?\\d*(E|e)?-?\\d{1,3})");

        public static PatternCategory Any => new PatternCategory($".*");

        public static PatternCategory Starter(string s) => new PatternCategory($"(?<=\\W{s})");



        public override string ToString()
        {
            return Value;
        }

        public static string InitFormatSectionbuilder(string Section)
        {
            return $"\\[{Section}\\].*(?:\\r?\\n\\s*[^\\]\\[\\s].*)+";
        }
    }
}
