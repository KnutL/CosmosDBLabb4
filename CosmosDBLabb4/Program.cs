using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CosmosDBLabb4
{
    class Program
    {
        private const string EndpointUrl = "https://labb4cosmos.documents.azure.com:443/";
        private const string PrimaryKey = "TryyZUkiERWKMOI0hD4jm6FICq7JvBEYUeXTm4nN6aG3xfZriSyhpAYuwScSKU2qkdvVkuBqzJz4Ey2QcPgDng==";
        private DocumentClient client;

        private string legitEmail = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

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
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }

        }
        private async Task GetStartedDemo()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "DBLabb4" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "Anvädare" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "BildGodkänd" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DBLabb4"), new DocumentCollection { Id = "BildSkaGranskas" });

            Console.WriteLine("Vad vill du göra?");
            Console.WriteLine("1. Lägg till ny användare.\n2. Se vilka användare som ska granskas.\n3.Avsluta.");
            int input = int.Parse(Console.ReadLine());
            bool running = true;
            while (running)
            {
                if (input == 1)
                {
                    Console.WriteLine("Lägg till en email: ");
                    string email = Console.ReadLine();
                    var regexEmail = Regex.IsMatch(email, legitEmail);
                    if (regexEmail == false)
                    {
                        Console.WriteLine("Ogiltig email formatering.");
                    }
                    else
                    {
                        Console.WriteLine("Lägg till en profilbild: ");
                        string pbUrl = Console.ReadLine();
                        var user = new User { Email = email, BildUrl = pbUrl };
                        await this.CreateFamilyDocumentIfNotExists("DBLabb4", "BildSkaGranskas", user);
                    }
                }
                else if (input == 2)
                {
                    this.ViewAnvändare("DBLabb4", "BildSkaGranskas");
                }
                else if (input == 3)
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("Fel input");
                }
            }
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
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.Email));
                this.WriteToConsoleAndPromptToContinue($"Användaren {user.Email} finns redan.\n");
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), user);
                    this.WriteToConsoleAndPromptToContinue($"Skapade användaren {user.Email}");
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

