﻿using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclusVoorAansturing")]
    public class WaarschuwingsGroepModel
    {
        [ModelName(TLCGenObjectTypeEnum.WaarschuwingsGroep)]
        [HasDefault(false)]
        public string Naam { get; set; }
        public bool Lichten { get; set; }
        public bool Bellen { get; set; }
        [HasDefault(false)]
        public string FaseCyclusVoorAansturing { get; set; }

        [IOElement("wl", BitmappedItemTypeEnum.Uitgang, "Naam")]
        public BitmapCoordinatenDataModel LichtenBitmapData { get; set; }

        [IOElement("bel", BitmappedItemTypeEnum.Uitgang, "Naam")]
        public BitmapCoordinatenDataModel BellenBitmapData { get; set; }

        public bool ShouldSerializeLichtenBitmapData()
        {
            return Lichten;
        }

        public bool ShouldSerializeBellenBitmapData()
        {
            return Bellen;
        }

        public WaarschuwingsGroepModel()
        {
            LichtenBitmapData = new BitmapCoordinatenDataModel();
            BellenBitmapData = new BitmapCoordinatenDataModel();
        }
    }
}
