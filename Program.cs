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


            //get 5.1
            // Decode strand
            string endpoint = "api/dna/jobs";
            HttpResponseMessage response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject(responseBody);

                string jobId = responseObject.job.id;
                string jobType = responseObject.job.type;
                string strand = responseObject.job.strand;
                string strandEncoded = responseObject.job.strandEncoded;
                string geneEncoded = responseObject.job.geneEncoded;

                Console.WriteLine($"Job ID: {jobId}");
                Console.WriteLine($"Job Type: {jobType}");
                Console.WriteLine($"Strand: {strand}");
                Console.WriteLine($"Strand Encoded: {strandEncoded}");
                Console.WriteLine($"Gene Encoded: {geneEncoded}");
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject(responseBody);

                string code = responseObject.code;
                string message = responseObject.message;

                Console.WriteLine($"Error: {code}, Message: {message}");
            }
        }
    }    
}




