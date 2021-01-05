using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestToPSCommand
{
    [Cmdlet(VerbsCommon.Get, "MXL3Firewall")]
    [OutputType(typeof(MXFirewallRule))]

    public class GetMXL3FirewallCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Token { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string NetId { get; set; } 

        private static async Task<IList<MXFirewallRule>> GetFWRules(string Token, string NetId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);

                // Created the request header above

                var streamTask = client.GetStreamAsync($"https://api.meraki.com/api/v0/networks/{NetId}/l3FirewallRules");

                // converting the serialized JSON response into a C# object
                return await JsonSerializer.DeserializeAsync<IList<MXFirewallRule>>(await streamTask);               
            }
        }

        private static IList<MXFirewallRule> ProcessRecordAsync(string Token, string NetId)
        {
            var task = GetFWRules(Token, NetId);
            task.Wait();
            return task.Result;
        }

        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose(Token);
        }

        protected override void ProcessRecord()
        {
            WriteVerbose("Entering Get Firewall Rules Call");
            var list = ProcessRecordAsync(Token, NetId);
            //var list = new List<String>{ "Apple", "Orange" };
            WriteObject(list, true);

            WriteVerbose("Exiting foreach");
        }

        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
    public class MXFirewallRule
    {
        public string comment { get; set; }
        public string policy { get; set; }
        public string protocol { get; set; }
        public string destPort { get; set; }
        public string destCidr { get; set; }
        public string srcPort { get; set; }
        public string srcCidr { get; set; }
        public bool syslogEnabled { get; set; }
    }

}

