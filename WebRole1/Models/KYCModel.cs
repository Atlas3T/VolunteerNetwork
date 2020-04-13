using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{

    public class KYCModel
    {
        public string status { get; set; }
        public KYCModelData data { get; set; }
    }

    public class KYCModelData
    {
        public string status { get; set; }
        public string recordId { get; set; }
        public string refId { get; set; }
        public bool isArchived { get; set; }
        public string blockPassID { get; set; }
        public string inreviewDate { get; set; }
        public string waitingDate { get; set; }
    }
}