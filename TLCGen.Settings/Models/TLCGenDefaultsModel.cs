﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenDefaultsModel
    {
        [XmlArrayItem(ElementName = "FaseCyclus")]
        public List<FaseCyclusDefaultsModel> Fasen { get; set; }

        [XmlArrayItem(ElementName = "Detector")]
        public List<DetectorDefaultsModel> Detectoren { get; set; }
    }
}
