﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseVan", TLCGenObjectTypeEnum.Fase, "FaseNaar")]
    public class NaloopModel : IInterSignaalGroepElement
    {
        #region Properties

        [HasDefault(false)]
        public string FaseVan { get; set; }
        [HasDefault(false)]
        public string FaseNaar { get; set; }
        public NaloopTypeEnum Type { get; set; }
        public bool VasteNaloop { get; set; }
        public bool InrijdenTijdensGroen { get; set; }
        public bool DetectieAfhankelijk { get; set; }
        public int? MaximaleVoorstart { get; set; }

        [XmlArrayItem(ElementName = "NaloopDetector")]
        public List<NaloopDetectorModel> Detectoren { get; set; }

        [XmlArrayItem(ElementName = "NaloopTijden")]
        public List<NaloopTijdModel> Tijden { get; set; }

        #endregion // Properties

        #region Constructor

        public NaloopModel()
        {
            Detectoren = new List<NaloopDetectorModel>();
            Tijden = new List<NaloopTijdModel>();
            VasteNaloop = true;
        }

        #endregion // Constructor
    }
}
