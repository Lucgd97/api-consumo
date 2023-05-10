using System.Text;

namespace Consumo_Api_Lacuna_Genetics.Class
{
    public class DnaConverter
    {
        public static string Encode(string strand)
        {
            var binaryString = new StringBuilder(strand.Length * 2);

            // Converter para binário
            foreach (var nucleobase in strand)
            {
                switch (nucleobase)
                {
                    case 'A':
                        binaryString.Append("00");
                        break;
                    case 'T':
                        binaryString.Append("11");
                        break;
                    case 'C':
                        binaryString.Append("01");
                        break;
                    case 'G':
                        binaryString.Append("10");
                        break;
                }
            }

            // Converter para hexadecimal
            var bytes = Enumerable.Range(0, binaryString.Length / 8)
                .Select(i => Convert.ToByte(binaryString.ToString().Substring(i * 8, 8), 2))
                .ToArray();

            // Converter para Base64
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string encodedStrand)
        {
            // Converter de base64 para hexadecimal
            var bytes = Convert.FromBase64String(encodedStrand);

            // Converter para binário string
            var binaryString = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                binaryString += Convert.ToString(bytes[i], 2).PadLeft(8, '0');
            }

            // Converter para String / Nucleobases
            var nucleobases = new StringBuilder(binaryString.Length / 2);
            for (int i = 0; i < binaryString.Length; i+=2)
            {
                var binaryNucleobase = binaryString.Substring(i, 2);
                switch (binaryNucleobase)
                {
                    case "00":
                        nucleobases.Append("A");
                        break;
                    case "01":
                        nucleobases.Append("C");
                        break;
                    case "10":
                        nucleobases.Append("G");
                        break;
                    case "11":
                        nucleobases.Append("T");
                        break;
                }
            }

            return nucleobases.ToString();
        }
        //public static string InvertComplementary(string strand)
        //{
            

        //    var complementaryStrand = new StringBuilder(strand.Length);

        //    foreach (var nucleobase in strand)
        //    {
        //        switch (nucleobase)
        //        {
        //            case 'A':
        //                complementaryStrand.Append('T');
        //                break;
        //            case 'T':
        //                complementaryStrand.Append('A');
        //                break;
        //            case 'C':
        //                complementaryStrand.Append('G');
        //                break;
        //            case 'G':
        //                complementaryStrand.Append('C');
        //                break;
        //        }
        //    }

        //    return complementaryStrand.ToString();
        //}
        

    }
}
