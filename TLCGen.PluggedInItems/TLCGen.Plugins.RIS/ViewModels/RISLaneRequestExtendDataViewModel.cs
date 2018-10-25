﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISLaneRequestExtendDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private RISLaneRequestExtendDataModel _laneData;
        
        //public int LaneID
        //{
        //    get => _laneData.LaneID;
        //    set
        //    {
        //        _laneData.LaneID = value;
        //        RaisePropertyChanged<object>(broadcast: true);
        //    }
        //}

        public bool RISAanvraag
        {
            get => _laneData.RISAanvraag;
            set
            {
                _laneData.RISAanvraag = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AanvraagStart
        {
            get => _laneData.AanvraagStart;
            set
            {
                _laneData.AanvraagStart = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AanvraagEnd
        {
            get => _laneData.AanvraagEnd;
            set
            {
                _laneData.AanvraagEnd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool RISVerlengen
        {
            get => _laneData.RISVerlengen;
            set
            {
                _laneData.RISVerlengen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenStart
        {
            get => _laneData.VerlengenStart;
            set
            {
                _laneData.VerlengenStart = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenEnd
        {
            get => _laneData.VerlengenEnd;
            set
            {
                _laneData.VerlengenEnd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string SignalGroupName
        {
            get => _laneData.SignalGroupName;
            set
            {
                _laneData.SignalGroupName = value;
                UpdateRijstroken();
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int RijstrookIndex
        {
            get => _laneData.RijstrookIndex;
            set
            {
                _laneData.RijstrookIndex = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public RISStationTypeEnum Type
        {
            get => _laneData.Type;
            set
            {
                _laneData.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public List<int> Rijstroken { get; private set; }

        public void UpdateRijstroken()
        {
            if (DataAccess.TLCGenControllerDataProvider.Default.Controller == null) return;
            var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.FirstOrDefault(x => x.Naam == SignalGroupName);
            if (fc == null) return;
            Rijstroken = new List<int>();
            for (int i = 0; i < fc.AantalRijstroken; ++i)
            {
                Rijstroken.Add(i + 1);
            }
            if (!Rijstroken.Contains(RijstrookIndex)) RijstrookIndex = 1;
            RaisePropertyChanged(nameof(Rijstroken));
        }

        public object GetItem()
        {
            return _laneData;
        }

        public int CompareTo(object obj)
        {
            var other = obj as RISLaneRequestExtendDataViewModel;
            if(_laneData.SignalGroupName == other.SignalGroupName)
            {
                return RijstrookIndex.CompareTo(other.RijstrookIndex);
            }
            else
            {
                return string.CompareOrdinal(_laneData.SignalGroupName, other.SignalGroupName);
            }
        }

        public RISLaneRequestExtendDataViewModel(RISLaneRequestExtendDataModel laneData)
        {
            _laneData = laneData;
            UpdateRijstroken();
        }
    }
}