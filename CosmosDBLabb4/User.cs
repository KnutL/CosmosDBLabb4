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
    class User
    {
        [JsonProperty(PropertyName ="id")]
        public int id { get; set; }
        public string Email { get; set; }
        public string BildUrl { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
