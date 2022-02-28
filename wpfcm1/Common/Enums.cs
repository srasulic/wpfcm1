using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfcm1.Common
{
    public class Enums
    {
        public enum DocumentType
        {
            OutboxDocument = 1,
            ConfirmedDocument = 2,
            GeneratedDocument = 3,
            InboxDocument = 4,
            PendDocument = 5 
        }
    }
}
