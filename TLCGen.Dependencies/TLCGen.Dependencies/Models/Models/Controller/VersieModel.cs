﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    [Serializable]
    public class VersieModel
    {
        [HasDefault(false)]
        public string Versie { get; set; }
        public DateTime Datum { get; set; }
        [HasDefault(false)]
        public string Ontwerper { get; set; }
        [HasDefault(false)]
        public string Commentaar { get; set; }

        public VersieModel()
        {
            Datum = new DateTime();
        }
    }
}
