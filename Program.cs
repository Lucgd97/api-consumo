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
            var username = "Lucas";
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
                        await CheckGeneJob(loginResponseObj.AccessToken, jobResponseObj);
                        break;
                    }
                case "DecodeStrand":
                    {
                        await DecodeStrandJob(loginResponseObj.AccessToken, jobResponseObj);
                        break;
                    }
                case "EncodeStrand":
                    {
                        await EncodeStrandJob(loginResponseObj.AccessToken, jobResponseObj);
                        break;
                    }
                default:
                    break;
            }
            
            Console.WriteLine("*** Fim ***");
        }

        private static async Task<CreateUserResponse> CreateUser(string username, string email, string password)
        {
            var api = new ApiHelper(_baseAddress);
            var createUserRequest = new CreateUserRequest
            {
                Username = username,
                Email = email,
                Password = password
            };

            Console.WriteLine($"Checking if the user {username} is created...");
            var response = await api.PostAsync(createUserRequest, "api/users/create");
            if (response == null)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<CreateUserResponse>(responseJson);

            if (responseObj.Code != "Success")
            {
                if (responseObj.Message != $"User {username} already exists")
                {
                    FailureMessage($"Error creating user: {responseObj.Message}");
                    return null;
                }
                SuccessfulMessage($"User {username} already created");
            }
            else
                SuccessfulMessage("User created successfully");

            return responseObj;
        }

        private static async Task<LoginResponse> Login(string username, string password)
        {
            Console.WriteLine($"Logging in user {username}...");

            var api = new ApiHelper(_baseAddress);
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await api.PostAsync(loginRequest, "api/users/login");
            if (response == null)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

            if (responseObj.Code != "Success")
            {
                FailureMessage($"Error logging in: {responseObj.Message}");
                return null;
            }

            SuccessfulMessage($"Logged in user {username}!");

            return responseObj;
        }

        private static async Task<JobResponse> GetJob(string accessToken)
        {
            Console.WriteLine("");
            Console.WriteLine($"Getting a Job...");

            var api = new ApiHelper(_baseAddress);

            var response = await api.GetAsync("api/dna/jobs", accessToken);
            if (response == null)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<JobResponse>(responseJson);

            if (responseObj.Code != "Success")
            {
                FailureMessage($"Error getting a job: {responseObj.Message}");
                return null;
            }

            SuccessfulMessage($"Job {responseObj.Job.Type} received.");

            return responseObj;
        }

        private static async Task EncodeStrandJob(string accessToken, JobResponse jobResponse)
        {
            Console.WriteLine("");
            Console.WriteLine($"Trying to solve the job type {jobResponse.Job.Type}...");

            // Converter atributo Strand para Base64
            var strandEncodedBase64 = DnaConverter.Encode(jobResponse.Job.Strand);

            // Responder ao Job
            Console.WriteLine($"Solved!");
            Console.WriteLine($"Sending job response...");

            var api = new ApiHelper(_baseAddress);
            var encodeStrandRequest = new EncodeStrandRequest { StrandEncoded = strandEncodedBase64 };

            var response = await api.PostAsync(encodeStrandRequest, $"api/dna/jobs/{jobResponse.Job.Id}/encode", accessToken);
            if (response == null)
                return;

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<EncodeStrandResponse>(responseJson);

            if (responseObj.Code != "Success")
            {
                if (responseObj.Code == "Fail")
                    FailureMessage($"Sorry, not yet! The Job {jobResponse.Job.Type} is incorrect! Try again.");
                else
                    FailureMessage($"Error trying to response the job: {responseObj.Message}");
                return;
            }

            JobSuccessfulMessage($"Congrats! The Job {jobResponse.Job.Type} is correct!");
        }

        private static async Task DecodeStrandJob(string accessToken, JobResponse jobResponse)
        {
            Console.WriteLine("");
            Console.WriteLine($"Trying to solve the job type {jobResponse.Job.Type}...");

            // Converter atributo Strand Base64 para String (Nucleobases)
            var strand = DnaConverter.Decode(jobResponse.Job.StrandEncoded);

            // Responder ao Job
            Console.WriteLine($"Solved!");
            Console.WriteLine($"Sending job response...");

            var api = new ApiHelper(_baseAddress);
            var decodeStrandRequest = new DecodeStrandRequest { Strand = strand };

            var response = await api.PostAsync(decodeStrandRequest, $"api/dna/jobs/{jobResponse.Job.Id}/decode", accessToken);
            if (response == null)
                return;

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<DecodeStrandResponse>(responseJson);

            if (responseObj.Code != "Success")
            {
                if (responseObj.Code == "Fail")
                    FailureMessage($"Sorry, not yet! The Job {jobResponse.Job.Type} is incorrect! Try again.");
                else
                    FailureMessage($"Error trying to response the job: {responseObj.Message}");
                return;
            }

            JobSuccessfulMessage($"Congrats! The Job {jobResponse.Job.Type} is correct!");
        }

        private static async Task CheckGeneJob(string accessToken, JobResponse jobResponse)
        {
            Console.WriteLine("");
            Console.WriteLine($"Trying to solve the job type {jobResponse.Job.Type}...");
            Console.WriteLine("Checking if the gene is activated...");
            var geneActivated = CheckGene.Check(jobResponse.Job.StrandEncoded, jobResponse.Job.GeneEncoded);

            // Responder ao Job
            Console.WriteLine($"Solved!");
            Console.WriteLine($"Sending job response...");

            var api = new ApiHelper(_baseAddress);
            var checkGeneRequest = new CheckGeneRequest { IsActivated = geneActivated };

            var response = await api.PostAsync(checkGeneRequest, $"api/dna/jobs/{jobResponse.Job.Id}/gene", accessToken);
            if (response == null)
                return;

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<CheckGeneResponse>(responseJson);

            if (responseObj.Code != "Success")
            {
                if (responseObj.Code == "Fail")
                    FailureMessage($"Sorry, not yet! The Job {jobResponse.Job.Type} is incorrect! Try again.");
                else
                    FailureMessage($"Error trying to response the job: {responseObj.Message}");
                return;
            }

            JobSuccessfulMessage($"Congrats! The Job {jobResponse.Job.Type} is correct!");
        }

        private static void JobSuccessfulMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void SuccessfulMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void FailureMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}




