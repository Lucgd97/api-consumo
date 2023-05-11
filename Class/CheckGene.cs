
namespace Consumo_Api_Lacuna_Genetics.Class
{
    public class CheckGene
    {
        public static bool Check(string strandEncoded, string geneEncoded)
        {
            // Decode the gene and the strand
            var gene = DnaConverter.Decode(geneEncoded);
            var strand = DnaConverter.Decode(strandEncoded);

            // Check if the strand is template and invert it if not
            if (strand.Substring(0, 3) != "CAT") // Complementary strand
            {
                // Temporary replaces
                strand = strand.Replace('G', 'W'); // C
                strand = strand.Replace('T', 'X'); // A
                strand = strand.Replace('A', 'Y'); // T
                strand = strand.Replace('C', 'Z'); // G

                // Final replaces
                strand = strand.Replace('W', 'C'); // C
                strand = strand.Replace('X', 'A'); // A
                strand = strand.Replace('Y', 'T'); // T
                strand = strand.Replace('Z', 'G'); // G
            }

            // Search gene into template strand
            var foundString = SearchString(gene, strand);
            if (foundString == null)
                return false;

            double dnaPresencePercent = foundString.FoundLength / Convert.ToDouble(gene.Length) * 100.0;

            return dnaPresencePercent > 50;
        }

        /// <summary>
        /// Algoritimo de busca do Gene no Template
        /// Obs: realiza a busca abaixo enquanto o tamanho da substring a ser buscada + a string já processada seja menor que o tamanho total do Gene
        /// 1. Inicia a busca da substring contendo os 3 primeiros caracteres do gene
        /// 2. A cada busca com sucesso, adiciona-se 1 caracter na substring encontrada (nova substring), reserva a substring encontrada e repete a busca
        /// 3. Se não encontrar a nova substring, esta é armazenada em uma lista e a nova substring "avança" 1 posição mantendo seu tamanho original
        /// 4. Após o fim do looping, a substring "vencedora" será a de maior tamanho (maior representatividade no template)
        /// </summary>
        /// <param name="gene">Gene a ser pesquisado</param>
        /// <param name="strandTemplate">DNA Template a ser verificado</param>
        /// <returns>A maior substring do gene encontrada dentro da string template</returns>
        private static FoundString? SearchString(string gene, string strandTemplate)
        {
            var searchInitialPosition = 0;
            var searchLength = 3;
            var stringLength = searchLength;
            var searchString = string.Empty;
            var lastSearchString = string.Empty;
            var foundStrings = new List<FoundString>();
            FoundString? foundString = null;

            while ((searchInitialPosition + searchLength) <= gene.Length)
            {
                searchString = gene.Substring(searchInitialPosition, searchLength);
                var pos = strandTemplate.IndexOf(searchString);
                if (pos >= 0) // Encontrou a substring na string template
                {
                    foundString = new FoundString
                    {
                        Found = searchString, // substring encontrada
                        FoundInitialPosition = pos,
                        FoundLength = searchLength
                    };
                    searchLength++; // adiciona mais 1 caracter a substring encontrada (tornando-a a nova substring)
                }
                else // não encontrou a nova substring
                {
                    if (foundString != null)
                        foundStrings.Add(foundString); // armazena a última substring encontrada
                    foundString = null;
                    searchInitialPosition++; // "Avança" a nova substring em 1 posição
                }
            }
            // Verifica se a substring foi encontrada na integralidade dentro do template - gene ativado em 100%
            if (foundString != null && foundStrings.Count == 0) // for 1 occurrence case 
                foundStrings.Add(foundString); 

            foundString = foundStrings.Any()
                ? foundStrings.MaxBy(x => x.FoundLength)  // Major found string
                : null; 

            return foundString;
        }
    }
}
