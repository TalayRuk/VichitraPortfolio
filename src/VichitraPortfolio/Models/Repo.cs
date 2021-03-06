﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VichitraPortfolio.Models
{
    public class Repo
    {
        public string Name { get; set; }
        public string Stargazers_count { get; set; }
        public string Html_url { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }

        public static List<Repo> GetRepos()
        {
            //Make a connection with the server where the API is located 
            var client = new RestClient("https://api.github.com/search/repositories?page=1&q=user:talayruk&sort=stars:>=1&order=desc");
            //Create the request, and add the physical path to the specific API controller an choose the HTTP method 
            //Can't add /Itmes.json to the account since there's no account just leave it empty string in order to get the json 
            var request = new RestRequest("", Method.GET);
            //Add parameters to our request. We have to set repository
            request.AddParameter("Access_token", EnvironmentVariables.AccessToken);
            request.AddHeader("User-Agent", "talayruk");
            //To get metadata in search results, specify the text-match media type in Accept header
            request.AddHeader("Accept", "application/vnd.github.v3.text-match+json");
            //Give the  client appropriate credentials & add /Itmes.json to the account string to get the response in JSON format **"items" is Json keys & its value is an array of JSON-formatted data about Repos
            client.Authenticator = new HttpBasicAuthenticator("/Itmes.json", EnvironmentVariables.AccessToken);
            //We initialize a new RestResponse variable named response

            var response = new RestResponse();
            //The request ismade w/an asynchronous method, allows us to await asynchronous calls in a "synchronous" way. We set response = to the response from our request, which we make in the method 1) and then cast as the type REstresponse

            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            //Console.WriteLine()) to process the request The response has content property
            //Trn the array stored as response.Content coverts the Json-formatted string response.Content into JObject (JObject comes from using NewtonSoft.Json.Linq library & is a .NET obj we can treat as JSON)
            //JSON key is "items" we can pull this array out as a JSON object by deserializing it. DeserializeObject went into the data for each repo and found those keys to create item objects for us
            //for this to work the property Name has to match the JSON key. This means that the Name property for our Repo class needs to be named "Name"
            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
            var repoList = JsonConvert.DeserializeObject<List<Repo>>(jsonResponse["items"].ToString());
            return repoList;
  

            //Console.WriteLine(jsonResponse["items"]); **B/c items is the key where the data is stored, can't change it to somethingelse
            //Console.ReadLine();
        }
        //GetRepos should show all the repositories, don't need another showrepositories unlike sending message we have to have another form to send the message but since we will not be sending these repos anywhere !
        
        //1)
        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            theClient.ExecuteAsync(theRequest, response => {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }
    }
}
