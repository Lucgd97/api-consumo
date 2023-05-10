using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Consumo_Api_Lacuna_Genetics.Class;
using Consumo_Api_Lacuna_Genetics.Entities;
using Consumo_Api_Lacuna_Genetics.Response;
using Newtonsoft.Json;

namespace Consumo_Api_Lacuna_Genetics
{
    class Program
    {
        static readonly string _baseAddress = "https://gene.lacuna.cc/";

        static async Task Main(string[] args)
        {
            // Informações do usuário criado anteriormente
            var username = "Cris123";
            var email = "lucas@email.com";
            var password = "lucas123456";

            // Cria o usuário se não existir
            var createUserResponseObj = await CreateUser(username, email, password);
            if (createUserResponseObj == null)
                return;

            //Login com as informações do usuário criado anteriormente
            var loginResponseObj = await Login(username, password);
            if (loginResponseObj == null)
                return;

            // Obter um job
            var jobResponseObj = await GetJob(loginResponseObj.AccessToken);
            if (jobResponseObj == null)
                return;


            // Realiza o Job conforme seu tipo
            switch (jobResponseObj.Job.Type)
            {
                case "CheckGene":
                    {
                        await CheckGene(loginResponseObj.AccessToken, jobResponseObj);
                        break;
                    }
                case "DecodeStrand":
                    {
                        await DecodeStrand(loginResponseObj.AccessToken, jobResponseObj);
                        break;
                    }
                case "EncodeStrand":
                    {
                        await EncodeStrand(loginResponseObj.AccessToken, jobResponseObj);
                        break;
                    }
                default:
                    break;
            }
                

            Console.WriteLine("*** Fim ***");
        }

        private static async Task CheckGene(HttpClient client, string jobId, string geneSequence)
        {
            var jobGeneUrl = $"api/dna/jobs/{jobId}/gene";
            var checkGeneRequest = new CheckGeneRequest { IsActivated = false };
            var checkGeneContent = new StringContent(JsonConvert.SerializeObject(checkGeneRequest), Encoding.UTF8, "application/json");
            var checkGeneResponse = await client.PostAsync(jobGeneUrl, checkGeneContent);
            checkGeneResponse.EnsureSuccessStatusCode();
            var checkGeneResponseJson = await checkGeneResponse.Content.ReadAsStringAsync();
            var checkGeneResponseObj = JsonConvert.DeserializeObject<ApiResponse>(checkGeneResponseJson);
            if (checkGeneResponseObj.Code == "Success")
            {
                Console.WriteLine($"Gene sequence: {geneSequence}");
                Console.WriteLine($"Is activated: {checkGeneRequest.IsActivated}");
            }
            else
            {
                Console.WriteLine($"Failed to check gene: {checkGeneResponseObj.Message}");
            }
        }



        //private static bool Teste(string gene, string dnaTemplate)
        //{           

        //    // Verificar se a sequência começa com "CAT"
        //    if (!dnaTemplate.StartsWith("CAT"))
        //    {
        //        // Inverter a sequência de DNA se ela começa com "GTA"
        //        if (dnaTemplate.StartsWith("GTA"))
        //        {
        //            dnaTemplate = new string(dnaTemplate.Reverse().ToArray());
        //        }
        //        // Se a sequência não começa com "CAT" nem com "GTA", retornar false
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    string dnaGene = "TACCGCTTCATAAACCGCTAGACTGCATGATCGGGT";
        //    dnaTemplate = "CATCTCAGTCCTACTAAACTCGCGAAGCTCATACTAGCTACTAAACCGCTAGACTGCATGATCGCATAGCTAGCTACGCT";
        //    bool result = Teste(dnaGene, dnaTemplate);
        //    Console.WriteLine(result); // Deve imprimir True

        //    // Criar uma expressão regular com o gene
        //    string regexPattern = "(" + gene + "){1}";

        //    // Encontrar todas as ocorrências do gene na sequência de DNA
        //    MatchCollection matches = Regex.Matches(dnaTemplate, regexPattern);

        //    // Calcular a porcentagem de ocorrências do gene em relação ao tamanho da sequência de DNA
        //    double percentage = (double)(matches.Count * gene.Length) / dnaTemplate.Length * 100;

        //    // Verificar se a porcentagem é maior do que 50%
        //    return percentage > 50;


        //public static bool IsGeneActivated(string gene, string dnaTemplate)
        //{
        //    // Inverte a fita complementar se a sequência começar com C-A-T
        //    if (dnaTemplate.StartsWith("CAT"))
        //    {
        //        dnaTemplate = DnaConverter.InvertComplementary(dnaTemplate);
        //    }

        //    // Procura por uma sequência correspondente ao gene na fita modelo
        //    var match = Regex.Match(dnaTemplate, @"[ACGT]{" + gene.Length + "}");

        //    if (match.Success && match.Value == gene)
        //    {
        //        // Calcula a porcentagem de correspondência entre o gene e a fita modelo
        //        float percentMatch = ((float)gene.Length / (float)dnaTemplate.Length) * 100;

        //        // Verifica se a porcentagem de correspondência é maior que 50%
        //        if (percentMatch > 50)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}


        //public async Task SendGeneActivationStatus(string jobId, bool isActivated, string accessToken)
        //{
        //    // Cria a URL da rota com o ID do trabalho
        //    var url = $"api/dna/jobs/{jobId}/gene";

        //    // Cria o corpo da solicitação
        //    var requestData = new
        //    {
        //        isActivated = isActivated
        //    };

        //    // Cria um objeto HttpClient e adiciona o cabeçalho de autorização
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //        // Converte o corpo da solicitação em JSON e envia a solicitação POST
        //        var json = JsonConvert.SerializeObject(requestData);
        //        var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

        //        // Lê a resposta da API e verifica se a operação foi bem sucedida
        //        var responseJson = await response.Content.ReadAsStringAsync();
        //        dynamic responseObject = JsonConvert.DeserializeObject(responseJson);

        //        if (responseObject.code == "Success")
        //        {
        //            Console.WriteLine("Gene activation status sent successfully.");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Failed to send gene activation status: " + responseObject.message);
        //        }
        //    }
        //}


        private static void Teste()
        {
            var encodeStrand = "CATCGTCAGGAC";
            Console.WriteLine(DnaConverter.Encode(encodeStrand));

            var decodeStrand = "TbSh";
            Console.WriteLine(DnaConverter.Decode(decodeStrand));

            // Checkgene
            var geneEncoded = "SDLtKpxdcx27fIvPR1klDxc1tKpOY2cEsRUZxh4DouWm1T2Yx6sAes1XJQtSG8TUKM5HRf+OZuTK6HiHQ63/uNqlKXIbjEdrNDrc";
            var strandEncoded = "ThmVF5eTJyPnfqg8K3Fk3+EbuKMcyxLdZMcjokW3ryEbj6TKSonk81HQs+mVTnIgcrTDMYEif3cHt7PQcITeMM6HIZzNmZVYN8VRTBgoSBVUAzCQ836f+A8swpLWFILN8oIaDizWUtgfa/m70DNf3m+de2xTTT1Q5ymMoNcCgSNChFqVDjnofrBXcYQ8XNbSqTmNnBLEVGcYeA6LlptU9mMerAHrNVyULUhvE1CjOR0X/jmbkyuh4h0Ot/7j5TX+VlOdrWrbAgIT7YScWn3Jjv4iApjdDj+hTExBJHiNIs/GvaWIGR2ZJa3R3/DE7dKM68NE8OKcIEgU+2Edmt6Khsu01pzYQXThiN3qCy5O3V56tDi0";
            var gene = DnaConverter.Decode(geneEncoded);
            var strand = DnaConverter.Decode(strandEncoded);
            Console.WriteLine(gene);
            Console.WriteLine(strand);

            var dnaGene = "TAAACCGCTAGACTGCATGATCG";
            var dnaTemplate = "CATCTCAGTCCTACTAAACTCGCGAAGCTCATACTAGCTACTAAACCGCTAGACTGCATGATCGCATAGCTAGCTACGCT";

            // Tentar resolver com Regex:
            var match = Regex.Match(dnaTemplate, @"[ACGT]{" + dnaGene.Length + "}");

            if (match.Success && match.Value == dnaGene)
            {
                Console.WriteLine("DNA gene found in DNA template!");

                // Calcular a porcentagem de correspondência entre a sequência de DNA e o template
                float percentMatch = ((float)dnaGene.Length / (float)dnaTemplate.Length) * 100;
                Console.WriteLine("Percentagem de correspondência: " + percentMatch + "%");
            }
            else
            {
                Console.WriteLine("DNA gene not found in DNA template.");
            }
        }


        private static async Task<CreateUserResponse> CreateUser(string username, string email, string password)
        {
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };

            var createUserRequest = new CreateUserRequest
            {
                Username = username,
                Email = email,
                Password = password
            };

            var createUserJson = JsonConvert.SerializeObject(createUserRequest);
            var createUserContent = new StringContent(createUserJson, Encoding.UTF8, "application/json");

            var createUserResponse = await client.PostAsync("api/users/create", createUserContent);
            createUserResponse.EnsureSuccessStatusCode();

            var createUserResponseJson = await createUserResponse.Content.ReadAsStringAsync();
            var createUserResponseObj = JsonConvert.DeserializeObject<CreateUserResponse>(createUserResponseJson);

            if (createUserResponseObj.Code != "Success")
            {
                if (createUserResponseObj.Message != $"User {username} already exists")
                {
                    Console.WriteLine($"Error creating user: {createUserResponseObj.Message}");
                    return null;
                }
            }
            else
                Console.WriteLine("User created successfully");

            return createUserResponseObj;
        }

        private static async Task<LoginResponse> Login(string username, string password)
        {
            Console.WriteLine($"Logging in user {username}...");

            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };

            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var loginJson = JsonConvert.SerializeObject(loginRequest);
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync("api/users/login", loginContent);
            loginResponse.EnsureSuccessStatusCode();

            var loginResponseJson = await loginResponse.Content.ReadAsStringAsync();
            var loginResponseObj = JsonConvert.DeserializeObject<LoginResponse>(loginResponseJson);

            if (loginResponseObj.Code != "Success")
            {
                Console.WriteLine($"Error logging in: {loginResponseObj.Message}");
                return null;
            }

            Console.WriteLine($"Logged in user {username}!");

            return loginResponseObj;
        }

        private static async Task<JobResponse> GetJob(string accessToken)
        {
            Console.WriteLine($"Getting a Job...");

            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var jobResponse = await client.GetAsync("api/dna/jobs");
            jobResponse.EnsureSuccessStatusCode();

            var jobResponseJson = await jobResponse.Content.ReadAsStringAsync();
            var jobResponseObj = JsonConvert.DeserializeObject<JobResponse>(jobResponseJson);

            if (jobResponseObj.Code != "Success")
            {
                Console.WriteLine($"Error getting a job: {jobResponseObj.Message}");
                return null;
            }

            Console.WriteLine($"Job {jobResponseObj.Job.Type} received.");

            return jobResponseObj;
        }

        private static async Task CheckGene(string accessToken, JobResponse jobResponse)
        {
            // Decode Gene base64 to String

            // Decode Strand base64 to String

            // Avaliar se o Gene está ativado
            var geneActivated = false;

            // Responder ao Job
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var checkGeneRequest = new CheckGeneRequest
            {
                IsActivated = geneActivated
            };

            var checkGeneJson = JsonConvert.SerializeObject(checkGeneRequest);
            var checkGeneContent = new StringContent(checkGeneJson, Encoding.UTF8, "application/json");

            var checkGeneResponse = await client.PostAsync($"api/dna/jobs/{jobResponse.Job.Id}/gene", checkGeneContent);
            checkGeneResponse.EnsureSuccessStatusCode();

            var checkGeneResponseJson = await checkGeneResponse.Content.ReadAsStringAsync();
            var checkGeneResponseObj = JsonConvert.DeserializeObject<CheckGeneResponse>(checkGeneResponseJson);

            if (checkGeneResponseObj.Code != "Success")
            {
                if (checkGeneResponseObj.Code == "Fail")
                    Console.WriteLine($"Sorry, not yet! The Job {jobResponse.Job.Type} is incorrect! Try again.");
                else
                    Console.WriteLine($"Error trying to response the job: {checkGeneResponseObj.Message}");
            }

            Console.WriteLine($"Congrats! The Job {jobResponse.Job.Type} is correct!");
        }

        private static async Task EncodeStrand(string accessToken, JobResponse jobResponse)
        {
            Console.WriteLine($"Trying to solve the job type {jobResponse.Job.Type}...");

            // Converter atributo Strand para Base64
            var strandEncodedBase64 = DnaConverter.Encode(jobResponse.Job.Strand);

            // Responder ao Job
            Console.WriteLine($"Solved!");
            Console.WriteLine($"Sending job response...");
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var encodeStrandRequest = new EncodeStrandRequest
            {
                StrandEncoded = strandEncodedBase64
            };

            var encodeStrandJson = JsonConvert.SerializeObject(encodeStrandRequest);
            var encodeStrandContent = new StringContent(encodeStrandJson, Encoding.UTF8, "application/json");

            var encodeStrandResponse = await client.PostAsync($"api/dna/jobs/{jobResponse.Job.Id}/encode", encodeStrandContent);
            encodeStrandResponse.EnsureSuccessStatusCode();

            var encodeStrandResponseJson = await encodeStrandResponse.Content.ReadAsStringAsync();
            var encodeStrandResponseObj = JsonConvert.DeserializeObject<EncodeStrandResponse>(encodeStrandResponseJson);

            if (encodeStrandResponseObj.Code != "Success")
            {
                if (encodeStrandResponseObj.Code == "Fail")
                    Console.WriteLine($"Sorry, not yet! The Job {jobResponse.Job.Type} is incorrect! Try again.");
                else
                    Console.WriteLine($"Error trying to response the job: {encodeStrandResponseObj.Message}");
            }

            Console.WriteLine($"Congrats! The Job {jobResponse.Job.Type} is correct!");
        }

        private static async Task DecodeStrand(string accessToken, JobResponse jobResponse)
        {
            Console.WriteLine($"Trying to solve the job type {jobResponse.Job.Type}...");

            // Converter atributo Strand Base64 para String (Nucleobases)
            var strand = DnaConverter.Decode(jobResponse.Job.StrandEncoded);

            // Responder ao Job
            Console.WriteLine($"Solved!");
            Console.WriteLine($"Sending job response...");
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var decodeStrandRequest = new DecodeStrandRequest
            {
                Strand = strand
            };

            var decodeStrandJson = JsonConvert.SerializeObject(decodeStrandRequest);
            var decodeStrandContent = new StringContent(decodeStrandJson, Encoding.UTF8, "application/json");

            var decodeStrandResponse = await client.PostAsync($"api/dna/jobs/{jobResponse.Job.Id}/decode", decodeStrandContent);
            decodeStrandResponse.EnsureSuccessStatusCode();

            var decodeStrandResponseJson = await decodeStrandResponse.Content.ReadAsStringAsync();
            var decodeStrandResponseObj = JsonConvert.DeserializeObject<DecodeStrandResponse>(decodeStrandResponseJson);

            if (decodeStrandResponseObj.Code != "Success")
            {
                if (decodeStrandResponseObj.Code == "Fail")
                    Console.WriteLine($"Sorry, not yet! The Job {jobResponse.Job.Type} is incorrect! Try again.");
                else
                    Console.WriteLine($"Error trying to response the job: {decodeStrandResponseObj.Message}");
            }

            Console.WriteLine($"Congrats! The Job {jobResponse.Job.Type} is correct!");
        }

    }
}

//main
//// Adicionar o cabeçalho de autorização no cliente HTTP
//client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

//// Codificar a sequência de DNA
//var dnaSequence = "ATCGATCG"; // substitua pela sua sequência de nucleobases
//var encoder = new DnaEncoder(dnaSequence);
//var binarySequence = encoder.EncodeBinary();
//var encodedString = encoder.EncodeString();

//Console.WriteLine($"Binary sequence: {BitConverter.ToString(binarySequence)}");
//Console.WriteLine($"Encoded string: {encodedString}");

//var jobResponse = await client.GetAsync($"api/dna/jobs?type=EncodeStrand&Data={dnaSequence}");

//try
//{
//    jobResponse.EnsureSuccessStatusCode();
//}
//catch (HttpRequestException ex)
//{
//    Console.WriteLine($"Error: {ex.Message}");
//    return;
//}

//// Processar a resposta do trabalho
//var jobResponseJson = await jobResponse.Content.ReadAsStringAsync();
//var jobResponseObj = JsonConvert.DeserializeObject<JobResponse>(jobResponseJson);

//if (jobResponseObj.Code != "Success")
//{
//    Console.WriteLine($"Failed to create job: {jobResponseObj.Message}");
//    return;
//}

//var jobId = jobResponseObj.Job.Id;
//var geneSequence = "TACCGCTTCATAAACCGCTAGACTGCATGATCGGG";
//await CheckGene(client, jobId, geneSequence);

//// Tentar resolver com Regex:
//string pattern = "^" + Regex.Escape(dnaGene) + "$";
//bool isMatch = Regex.IsMatch(dnaTemplate, pattern, RegexOptions.IgnoreCase);

//if (isMatch)
//{
//    Console.WriteLine("As strings são semelhantes!");
//}
//else
//{
//    Console.WriteLine("As strings não são semelhantes!");
//}



