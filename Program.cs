using Newtonsoft.Json;
using OAuth2.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OAuth2
{
    class Program
    {
        static readonly string authUrl = "";
        static readonly string clientId = "";
        static readonly string clientSecret = "";
        static readonly string companyName = "";
        static readonly string callbackUrl = "http://127.0.0.1:8888/";
        static readonly string basepath = "c:\\w\\tmp\\";

        //        static readonly string refresh_token = "9b22e0795c09c6183d4484bec49dce89";
        static string accessToken = "";

        static void Main(string[] args)
        {

            //NEXT LINE RECIEVES REFRESH_TOKEN BY SENDING YOU TO THE LOGIN PAGE
            var refresh_token = Auth.GetRefreshToken(authUrl, clientId, clientSecret, callbackUrl)?.RefreshToken;

            //AFTER WE HAVE THE REFRESH TOKEN WE CAN GET ACCESS TOKEN AND USE IT TO CALL API
            Console.WriteLine("Getting access token ...");
            accessToken = Auth.RefreshAccessToken(authUrl, clientId, clientSecret, refresh_token);

            GetSchema("Style");
            GetSchema("Material");

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        public static void GetSchema(string masterfolder)
        {
            if (!Directory.Exists(Path.Combine(basepath, masterfolder)))
                Directory.CreateDirectory(Path.Combine(basepath, masterfolder));

            Console.WriteLine(String.Format($"Loading {masterfolder} API..."));


            Console.WriteLine("Listing Style folders ...");
            var client = new RestClient("https://prd-beproduct-papi-as-e-svc1.azurewebsites.net/");
            var request = new RestRequest("/api/" + companyName + $"/{masterfolder}/Folders", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var response = client.Execute<dynamic>(request);
            var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Console.Write(response.Content);
            Console.WriteLine("Press any key...");
            Console.ReadKey();

            foreach (var folder in result)
            {
                Console.WriteLine("Getting  schema ...");
                request = new RestRequest("/api/" + companyName + $"/{masterfolder}/FolderSchema?folderId=" + folder.id, Method.GET);

                request.AddHeader("Authorization", "Bearer " + accessToken);
                response = client.Execute<dynamic>(request);
                result = JsonConvert.DeserializeObject<dynamic>(response.Content);

                File.WriteAllText(Path.Combine(basepath, masterfolder, $"{folder.name} - {folder.id}.json"), response.Content);
            }
        }


    }
}
