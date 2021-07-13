using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalTwin
{
    public class Equipment
    {
        public string assetId { get; set; }
        public string type { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string serial { get; set; }
        public string group { get; set; }
        public string vendor { get; set; }     
        
        public List<Relationship> relationships { get; set; }

    }

    public class Relationship
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
    }
}
