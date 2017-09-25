using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class Appointments
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string AppointmentName { get; set; }

        [NotNull]
        public string DateOfApp { get; set; }

        [NotNull]
        public string Time { get; set; }

        [NotNull]
        public string Meridiem { get; set; }
    }
}
