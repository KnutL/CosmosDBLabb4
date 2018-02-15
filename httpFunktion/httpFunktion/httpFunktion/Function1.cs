using System;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;

namespace httpFunktion
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("DBLabb4 Connection");
            string mode = req.GetQueryNameValuePairs()
                        .FirstOrDefault(q => string.Compare(q.Key, "mode", true) == 0)
                        .Value;

            var user = GetUser(mode);
            if (mode == "BildSkaGranskas")
            {
                return req.CreateResponse(HttpStatusCode.OK, user, "application/json");
            }
            else if (mode != "BildSkaGranskas" && mode != null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Incorrect input.Please pass the following command/s to view DB:\n?mode=BildSkaGranskas");
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Use the following command/s to view DB:\n?mode = BildSkaGranskas");
            }
        }

        // Reads ReviewQueueCollection from CosmosDB
        private static List<user> GetUser(string id)
        {
            string EndpointUrl = "https://labb4cosmos.documents.azure.com:443/";

            string PrimaryKey = "TryyZUkiERWKMOI0hD4jm6FICq7JvBEYUeXTm4nN6aG3xfZriSyhpAYuwScSKU2qkdvVkuBqzJz4Ey2QcPgDng==";

            string databaseName = "DBLabb4";

            string collectionName = "BildSkaGranskas";

            var client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            var userQuery = client.CreateDocumentQuery<user>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), "SELECT * FROM " + collectionName);

            var user = userQuery.ToList();

            return user;
        }
    }
}
