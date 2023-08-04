using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace SmartEco.Models
{
    public class TreeNodes
    {
        public TreeNodes()
        {
            Data = new List<DataNode>();
        }

        public List<DataNode> Data { get; set; }
        public string GetDataJson()
            => JsonConvert.SerializeObject(
                Data, 
                Formatting.Indented, 
                new JsonSerializerSettings() 
                { 
                    ContractResolver = new CamelCasePropertyNamesContractResolver() 
                });
    }

    public class DataNode
    {
        public DataNode()
        {
            State = new StateNode();
        }

        public string Id { get; set; }
        public string Parent { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public StateNode State { get; set; }
    }

    public class StateNode
    {
        public bool Opened { get; set; } = true;
        public bool Disabled { get; set; }
        public bool Selected { get; set; }
    }
}
