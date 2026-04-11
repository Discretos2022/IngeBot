using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot.Models.System
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ColumnAttribut : Attribute
    {

        public string Name;

        public ColumnAttribut(string name)
        {
            Name = name;
        }

    }
}
