using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    class ContactDetails
    {

        [PrimaryKey]
        public string contactID { get; set; }

        [Unique]
        public string firstName { get; set; }

        [Unique]
        public string lastName { get; set; }

        [NotNull]
        public string phoneNumber { get; set; }


    }
}
