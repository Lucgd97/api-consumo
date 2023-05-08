using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumo_Api_Lacuna_Genetics.Class
{
    public class DnaEncoder
    {
        private string _dnaSequence;

        public DnaEncoder(string dnaSequence)
        {
            _dnaSequence = dnaSequence;
        }

        public byte[] EncodeBinary()
        {
            var binaryString = "";
            foreach (var nucleobase in _dnaSequence)
            {
                switch (nucleobase)
                {
                    case 'A':
                        binaryString += "00";
                        break;
                    case 'T':
                        binaryString += "01";
                        break;
                    case 'C':
                        binaryString += "10";
                        break;
                    case 'G':
                        binaryString += "11";
                        break;
                }
            }
            return Enumerable.Range(0, binaryString.Length / 8)
                .Select(i => Convert.ToByte(binaryString.Substring(i * 8, 8), 2))
                .ToArray();
        }

        public string EncodeString()
        {
            var encodedString = "";
            foreach (var nucleobase in _dnaSequence)
            {
                switch (nucleobase)
                {
                    case 'A':
                        encodedString += "00";
                        break;
                    case 'T':
                        encodedString += "01";
                        break;
                    case 'C':
                        encodedString += "10";
                        break;
                    case 'G':
                        encodedString += "11";
                        break;
                }
            }
            return encodedString;
        }

        public static string Decode(string encodedString)
        {
            var decodedString = "";
            for (int i = 0; i < encodedString.Length; i += 2)
            {
                var nucleobase = encodedString.Substring(i, 2);
                switch (nucleobase)
                {
                    case "00":
                        decodedString += "A";
                        break;
                    case "01":
                        decodedString += "T";
                        break;
                    case "10":
                        decodedString += "C";
                        break;
                    case "11":
                        decodedString += "G";
                        break;
                }
            }
            return decodedString;
        }
    }

