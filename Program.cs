using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Consumo_Api_Lacuna_Genetics.Class;
using Consumo_Api_Lacuna_Genetics.Entities;
using Consumo_Api_Lacuna_Genetics.Response;
using Newtonsoft.Json;

namespace Consumo_Api_Lacuna_Genetics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var baseAddress = new Uri("https://gene.lacuna.cc/");
            using var client = new HttpClient { BaseAddress = baseAddress };

            // Informações do usuário criado anteriormente
            var username = "Danilo12345";
            var email = "lucas@email.com";
            var password = "lucas123456";

            // Cria o usuário como no código anterior
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
                Console.WriteLine($"Error creating user: {createUserResponseObj.Message}");
                return;
            }

            Console.WriteLine("User created successfully");

            //Login com as informações do usuário criado anteriormente

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
                return;
            }

            // Obter o token de acesso
            var accessToken = loginResponseObj.AccessToken;

            // Adicionar o cabeçalho de autorização no cliente HTTP
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Codificar a sequência de DNA
            var dnaSequence = "ATCGATCG"; // substitua pela sua sequência de nucleobases
            var encoder = new DnaEncoder(dnaSequence);
            var binarySequence = encoder.EncodeBinary();
            var encodedString = encoder.EncodeString();

            Console.WriteLine($"Binary sequence: {BitConverter.ToString(binarySequence)}");
            Console.WriteLine($"Encoded string: {encodedString}");

            

            var jobResponse = await client.GetAsync($"api/dna/jobs?type=EncodeStrand&Data={dnaSequence}");

            try
            {
                jobResponse.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return;
            }

            // Processar a resposta do trabalho
            var jobResponseJson = await jobResponse.Content.ReadAsStringAsync();
            var jobResponseObj = JsonConvert.DeserializeObject<JobResponse>(jobResponseJson);

            if (jobResponseObj.Code != "Success")
            {
                Console.WriteLine($"Failed to create job: {jobResponseObj.Message}");
                return;
            }

            var jobId = jobResponseObj.Job.Id;
            var geneSequence = "TACCGCTTCATAAACCGCTAGACTGCATGATCGGG";
            await CheckGene(client, jobId, geneSequence);
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
    }    
}




