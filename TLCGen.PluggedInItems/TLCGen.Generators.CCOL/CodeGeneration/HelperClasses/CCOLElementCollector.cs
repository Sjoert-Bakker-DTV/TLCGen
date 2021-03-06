﻿using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public static class CCOLElementCollector
    {
        #region Static Fields

        private static int _koppelSignaalCount;
        private static bool _koppelSignaalCountSet;
        private static List<CCOLKoppelSignaal> _koppelSignalen;

        #endregion // Static Fields

        #region Static Properties

        public static void AddKoppelSignaal(int order, string name, CCOLKoppelSignaalRichtingEnum richting)
        {
            _koppelSignalen.Add(new CCOLKoppelSignaal() { Order = order, Name = name, Richting = richting });
        }

        public static void AddKoppelSignaal(int order, int count, string name, CCOLKoppelSignaalRichtingEnum richting)
        {
            if(_koppelSignalen.Any(x => x.Count == count))
            {
                // TODO: warn user
            }
            _koppelSignalen.Add(new CCOLKoppelSignaal() { Count = count, Order = order, Name = name, Richting = richting });
        }

        public static int GetKoppelSignaalCount(string name, CCOLKoppelSignaalRichtingEnum richting)
        {
            if (!_koppelSignaalCountSet)
            {
                _koppelSignalen.Sort((x, y) => (x.Order * 1000 + x.Count).CompareTo(y.Order * 1000 + y.Count));
                foreach (var k in _koppelSignalen)
                {
                    if (_koppelSignalen.Any(x => x.Count == _koppelSignaalCount))
                    {
                        // TODO : warn user
                    }
                    if (k.Count == 0)
                    {
                        k.Count = _koppelSignaalCount;
                        ++_koppelSignaalCount;
                    }
                }
            }
            var ks = _koppelSignalen.FirstOrDefault(x => x.Name == name && x.Richting == richting);
            if (ks != null) return ks.Count;
            else return 0;
        }

        #endregion // Static Properties

        #region Static Public Methods

        public static void Reset()
        {
            _koppelSignaalCount = 1;
            _koppelSignaalCountSet = false;
            _koppelSignalen = new List<CCOLKoppelSignaal>();
        }

        public static void AddAllMaxElements(CCOLElemListData[] lists)
        {
            lists[0].Elements.Add(new CCOLElement() { Define = "USMAX1", Commentaar = "Totaal aantal uitgangen" });
            lists[1].Elements.Add(new CCOLElement() { Define = "ISMAX1", Commentaar = "Totaal aantal ingangen" });
            lists[2].Elements.Add(new CCOLElement() { Define = "HEMAX1", Commentaar = "Totaal aantal hulpelementen" });
            lists[3].Elements.Add(new CCOLElement() { Define = "MEMAX1", Commentaar = "Totaal aantal geheugen elementen" });
            lists[4].Elements.Add(new CCOLElement() { Define = "TMMAX1", Commentaar = "Totaal aantal timers" });
            lists[5].Elements.Add(new CCOLElement() { Define = "CTMAX1", Commentaar = "Totaal aantal counters" });
            lists[6].Elements.Add(new CCOLElement() { Define = "SCHMAX1", Commentaar = "Totaal aantal schakelaars" });
            lists[7].Elements.Add(new CCOLElement() { Define = "PRMMAX1", Commentaar = "Totaal aantal parameters" });
        }

        public static CCOLElemListData[] CollectAllCCOLElements(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var lists = new CCOLElemListData[8];

            lists[0] = CollectAllUitgangen(controller, pgens);
            lists[1] = CollectAllIngangen(controller, pgens);
            lists[2] = CollectAllHulpElementen(controller, pgens);
            lists[3] = CollectAllGeheugenElementen(controller, pgens);
            lists[4] = CollectAllTimers(controller, pgens);
            lists[5] = CollectAllCounters(controller, pgens);
            lists[6] = CollectAllSchakelaars(controller, pgens);
            lists[7] = CollectAllParameters(controller, pgens);

            return lists;
        }

        #endregion // Static Public Methods

        #region Static Private Methods

        private static CCOLElemListData CollectAllUitgangen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "US_code" };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Uitgang))
                    {
                        data.Elements.Add(i);
                    }
                }
            }
            
            return data;
        }

        private static CCOLElemListData CollectAllIngangen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "IS_code" };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Ingang))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        private static CCOLElemListData CollectAllHulpElementen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "H_code" };

            // Collect everything

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.HulpElement))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            if (data.Elements.Count == 0)
            {
                data.Elements.Add(new CCOLElement() { Define = "hedummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllGeheugenElementen(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData { CCOLCode = "MM_code" };

            data.Elements.Add(new CCOLElement() { Define = "mperiod", Naam = "PERIOD", Commentaar = "Onthouden actieve periode" });

            // Collect everything
            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.GeheugenElement))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        private static CCOLElemListData CollectAllTimers(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "T_code",
                CCOLSetting = "T_max",
                CCOLTType = "T_type"
            };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Timer))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            if (data.Elements.Count == 0)
            {
                data.Elements.Add(new CCOLElement { Define = "tdummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllSchakelaars(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "SCH_code",
                CCOLSetting = "SCH"
            };

            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Schakelaar))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        private static CCOLElemListData CollectAllCounters(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "C_code",
                CCOLSetting = "C_max",
                CCOLTType = "C_type"
            };

            // Collect everything
            foreach (var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Counter))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            if (data.Elements.Count == 0)
            {
                data.Elements.Add(new CCOLElement() { Define = "ctdummy", Naam = "dummy" });
            }

            return data;
        }

        private static CCOLElemListData CollectAllParameters(ControllerModel controller, List<ICCOLCodePieceGenerator> pgens)
        {
            var data = new CCOLElemListData
            {
                CCOLCode = "PRM_code",
                CCOLSetting = "PRM",
                CCOLTType = "PRM_type"
            };

            // Collect everything
            data.Elements.Add(new CCOLElement() { Define = "prmfb", Naam = "FB", Instelling = controller.Data.Fasebewaking, TType = CCOLElementTimeTypeEnum.TS_type, Commentaar = "Instelling fasebewaking" });

            foreach(var pgen in pgens)
            {
                if (pgen.HasCCOLElements())
                {
                    foreach (var i in pgen.GetCCOLElements(CCOLElementTypeEnum.Parameter))
                    {
                        data.Elements.Add(i);
                    }
                }
            }

            return data;
        }

        #endregion // Static Private Methods

    }
}
