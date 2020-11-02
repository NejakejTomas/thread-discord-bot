using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Encoding
{
	static class BaseConverter
	{
        private static readonly char[] Alphabet = new char[] {'\u180E', '\u200B', '\u202C', '\u2060', '\u2061', '\u2062', '\u2063', '\u2064', '\u2068', '\u2069', '\u206A', '\u206B', '\u206C', '\u206D', '\u206E', '\u206F' };
        private static readonly Dictionary<char, byte> AlphabetTranslation;

        static BaseConverter()
		{
            AlphabetTranslation = new();

            for (byte i = 0; i < Alphabet.Length; i++)
			{
                AlphabetTranslation.Add(Alphabet[i], i);
			}
		}

        public static string ToMyBase(Span<byte> data)
		{
            StringBuilder builder = new();

            for (int i = 0; i < data.Length; i++)
			{
                char first, second;
                byte current = data[i];

                second = Alphabet[current & 0xF];
                first = Alphabet[(current >> 4) & 0xF];

                builder.Append(first).Append(second);
            }

            return builder.ToString();
		}
        
        public static Span<byte> FromMyBase(ref Span<byte> buffer, string data)
		{
            for (int i = 0; i < data.Length; i += 2)
            {
                char first = data[i];
                char second = data[i + 1];
                byte current = (byte)((AlphabetTranslation[first] << 4) | AlphabetTranslation[second]);
                buffer[i / 2] = current;
			}

            return buffer;
		}
    }
}

/* Work:
 * MONGOLIAN VOWEL SEPARATOR        U+180E
 * ZERO WIDTH SPACE                 U+200B
 * LEFT-TO-RIGHT MARK               U+200E
 * RIGHT-TO-LEFT MARK               U+200F
 * LEFT-TO-RIGHT EMBEDDING          U+202A
 * RIGHT-TO-LEFT EMBEDDING          U+202B
 * POP DIRECTIONAL FORMATTING       U+202C
 * LEFT-TO-RIGHT OVERRIDE           U+202D
 * WORD JOINER                      U+2060
 * FUNCTION APPLICATION             U+2061
 * INVISIBLE TIMES                  U+2062
 * INVISIBLE SEPARATOR              U+2063
 * INVISIBLE PLUS                   U+2064
 * LEFT-TO-RIGHT ISOLATE            U+2066
 * RIGHT-TO-LEFT ISOLATE            U+2067
 * FIRST STRONG ISOLATE             U+2068
 * POP DIRECTIONAL ISOLATE          U+2069
 * INHIBIT SYMMETRIC SWAPPING       U+206A
 * ACTIVATE SYMMETRIC SWAPPING      U+206B
 * INHIBIT ARABIC FORM SHAPING      U+206C
 * ACTIVATE ARABIC FORM SHAPING     U+206D
 * NATIONAL DIGIT SHAPES            U+206E
 * NOMINAL DIGIT SHAPES             U+206F
 * 
 * Does not work:
 * !!!RIGHT-TO-LEFT OVERRIDE        U+202E!!!
 * !!!ZERO WIDTH NO-BREAK SPACE     U+FEFF!!!
 */