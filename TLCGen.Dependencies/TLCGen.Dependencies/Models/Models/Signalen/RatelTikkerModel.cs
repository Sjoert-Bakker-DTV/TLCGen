﻿using System;
using System.Collections.Generic;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class RatelTikkerModel
    {
        public RateltikkerTypeEnum Type { get; set; }
        public int NaloopTijd { get; set; }
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public List<RatelTikkerDetectorModel> Detectoren { get; set; }

        [IOElement("rt", BitmappedItemTypeEnum.Uitgang, "FaseCyclus")]
        public BitmapCoordinatenDataModel BitmapData { get; set; }

        public bool ShouldSerializeNaloopTijd()
        {
            return Type == RateltikkerTypeEnum.Hoeflake;
        }

        public RatelTikkerModel()
        {
            BitmapData = new BitmapCoordinatenDataModel();
            Detectoren = new List<RatelTikkerDetectorModel>();
        }
    }
}
