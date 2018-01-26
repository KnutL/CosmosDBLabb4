using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace CosmosDBLabb4
{
    class Program
    {
        private const string EndpointUrl = "https://labb4cosmos.documents.azure.com:443/";
        private const string PrimaryKey = "TryyZUkiERWKMOI0hD4jm6FICq7JvBEYUeXTm4nN6aG3xfZriSyhpAYuwScSKU2qkdvVkuBqzJz4Ey2QcPgDng==";
        private DocumentClient client;

        static void Main(string[] args)
        {

            try
            {
                Program p = new Program();
                p.GetStartedDemo().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }

        }
        private async Task GetStartedDemo()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "DBLabb4" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "Användare" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "BildGranskad" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "BildInteGranskad" });
            Labb användare = new Labb
            {
                Id = "Användare",
                användare = new Användare[]
                {

                new Användare { FirstName = "Christoffer", Email = "christoffer@hotmail.com" },
                new Användare { FirstName = "Knut", Email = "knutte@hotmail.com" }
                }
            };
            await this.CreateFamilyDocumentIfNotExists("DBLabb4", "Användare", användare);

            Labb bildgranskad = new Labb
            {
                Id = "Bild Granskad",
                bildGranskad = new BildGranskad[]
                    {
                new BildGranskad { FirstName = "Christoffer", URL = "https://media.discordapp.net/attachments/395942679982112778/403658969047891970/bandit.png" },
                    }
            };
            await this.CreateFamilyDocumentIfNotExists("DBLabb4", "BildGranskad", bildgranskad);
        }

        


        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
        public class Labb
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            public Användare[] användare { get; set; }
            public BildGranskad[] bildGranskad { get; set; }
            public BildInteGranskad bildInteGranskad { get; set; }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
        public class Användare
        {

            public string FirstName { get; set; }
            public string Email { get; set; }
        }
        public class BildGranskad
        {

            public string FirstName { get; set; }
            public string URL { get; set; }
        }
        public class BildInteGranskad
        {

            public string FirstName { get; set; }
            public string URL { get; set; }
        }
        private async Task CreateFamilyDocumentIfNotExists(string databaseName, string collectionName, Labb labb)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, labb.Id));
                this.WriteToConsoleAndPromptToContinue("Found {0}", labb.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), labb);
                    this.WriteToConsoleAndPromptToContinue("Created Labb {0}", labb.Id);
                }
                else
                {
                    throw;
                }
            }
        }

    }

}

