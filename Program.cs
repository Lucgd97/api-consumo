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
            var username = "Lucasgilioducci97";
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

            // Fazer a solicitação para um novo trabalho
            var jobRequest = new JobRequest
            {
                Type = "EncodeStrand",
                Data = dnaSequence
            };

            var jobRequestJson = JsonConvert.SerializeObject(jobRequest);
            var jobRequestContent = new StringContent(jobRequestJson, Encoding.UTF8, "application/json");

            var jobResponse = await client.PostAsync("api/dna/jobs", jobRequestContent);
            jobResponse.EnsureSuccessStatusCode();

            // Processar a resposta do trabalho
            var jobResponseJson = await jobResponse.Content.ReadAsStringAsync();
            var jobResponseObj = JsonConvert.DeserializeObject<JobResponse>(jobResponseJson);

            if (jobResponseObj.Code != "Success")
            {
                Console.WriteLine($"Error requesting job: {jobResponseObj.Message}");
                return;
            }

            if (jobResponseObj.Job == null)
            {
                Console.WriteLine("Job object is null");
                return;
            }

            string jobId = jobResponseObj.Job.Id;

            Console.WriteLine($"Job requested successfully. Job ID: {jobId}");


            // Fazer solicitação GET para obter um trabalho
            var response = await client.GetAsync($"api/dna/jobs/{jobId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var job = JsonConvert.DeserializeObject<JobResponse>(content);


            // Verificar se há um trabalho e se é para decodificar a fita de DNA
            if (job != null && job.Job.Type == "DecodeStrand")
            {
                // Decodificar a fita de DNA
                var decodedStrand = DnaEncoder.Decode(job.Job.StrandEncoded);
                var requestBody = new { strand = decodedStrand };

                // Fazer solicitação POST para enviar o resultado da decodificação
                var requestUrl = $"api/dna/jobs/{job.Job.Id}/decode";
                var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var request = await client.PostAsync(requestUrl, requestContent);

                // Verificar a resposta
                var responseContent = await request.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                if (result.Code == "Success")
                {
                    Console.WriteLine("Fita de DNA decodificada com sucesso!");
                }
                else
                {
                    Console.WriteLine("Erro ao decodificar a fita de DNA.");
                }
            }
            else
            {
                Console.WriteLine("Não há trabalho para decodificar a fita de DNA.");
            }

        }
    }
    
}




