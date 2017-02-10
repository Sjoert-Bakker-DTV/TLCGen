﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLMeeAanvragenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        
        private string _hmad; // help element meeaanvraag detector name

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (MeeaanvraagModel ma in c.InterSignaalGroep.Meeaanvragen)
            {
                if (ma.DetectieAfhankelijk)
                {
                    foreach(MeeaanvraagDetectorModel dm in ma.Detectoren)
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_hmad}{dm.MeeaanvraagDetector}",
                                CCOLElementTypeEnum.HulpElement));
                    }
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Meeaanvragen */");
                    sb.AppendLine($"{ts}/* ------------ */");
                    bool hasdetafh = false;
                    if(c.InterSignaalGroep.Meeaanvragen.Count > 0)
                    {
                        hasdetafh = c.InterSignaalGroep.Meeaanvragen.Where(x => x.DetectieAfhankelijk == true).Any();
                    }
                    if(hasdetafh)
                    {
                        sb.AppendLine($"{ts}/* Bewaar meldingen van detectie voor het zetten van een meeaanvraag */");
                        foreach (MeeaanvraagModel ma in c.InterSignaalGroep.Meeaanvragen)
                        {
                            if (ma.DetectieAfhankelijk)
                            {
                                foreach(MeeaanvraagDetectorModel dm in ma.Detectoren)
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]= SG[{_fcpf}{ma.FaseVan}] ? FALSE : IH[{_hpf}{_hmad}{dm.MeeaanvraagDetector}] || D[{_dpf}{dm.MeeaanvraagDetector}] && !G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}];");
                                }
                            }
                        }
                        sb.AppendLine();
                    }
                    foreach (MeeaanvraagModel ma in c.InterSignaalGroep.Meeaanvragen)
                    {
                        if(!ma.DetectieAfhankelijk)
                        {
                            switch(ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (!G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (SG[{_fcpf}{ma.FaseVan}]));");
                                    break;
                            }
                        }
                        else
                        {
                            sb.Append($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) ((");
                            int i = 0;
                            foreach (MeeaanvraagDetectorModel dm in ma.Detectoren)
                            {
                                if (i == 1)
                                {
                                    sb.Append(" || ");
                                }
                                ++i;
                                sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                            }

                            switch (ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($") && !G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($") && RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine($") && SG[{_fcpf}{ma.FaseVan}]));");
                                    break;
                            }
                        }
                    }
                    sb.AppendLine("");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }
    }
}
