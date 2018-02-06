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

        private string legitEmail = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        private string legitURL = @"/(https?:\/\/.*\.(?:png|jpg|jpeg))/i";

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

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "BildSkaGranskas" });
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
        private async Task CreateFamilyDocumentIfNotExists(string databaseName, string collectionName, User user)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.email));
                this.WriteToConsoleAndPromptToContinue("Found {0}", user.id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), user);
                    this.WriteToConsoleAndPromptToContinue("Created User {0}", user.id);
                }
                else
                {
                    throw;
                }
            }
        }
        private void ViewAnvändare(string databaseName, string collectionName)
        {

            IQueryable<User> userQuery = this.client.CreateDocumentQuery<User>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    "SELECT * FROM User");

            Console.WriteLine("Viewing review queue collection:");
            foreach (User user in userQuery)
            {
                Console.WriteLine($"\t{user}");
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}

