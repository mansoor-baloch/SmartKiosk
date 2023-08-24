using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp.Models
{
    class DbParameter
    {
        public string Name { get; set; }
        public ParameterDirection Direction { get; set; }
        public object Value { get; set; }

        public DbParameter(string paramName, ParameterDirection paramDirection, object paramValue)
        {
            Name = paramName;
            Direction = paramDirection;
            Value = paramValue;
        }
    }
}
