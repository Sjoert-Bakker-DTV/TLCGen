﻿using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Helpers;
using TLCGen.ModelManagement;
using System.Text.RegularExpressions;

namespace TLCGen.ViewModels
{
    public class DetectorViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private readonly DetectorModel _detector;
        private string _faseCyclus;
        private List<int> _rijstroken;

        #endregion // Fields

        #region Properties

        public DetectorModel Detector => _detector;

	    public int? Rijstrook
        {
            get => _detector.Rijstrook;
		    set
            {
                _detector.Rijstrook = value;
                RaisePropertyChanged<object>(nameof(Rijstrook), broadcast: true);
            }
        }

        public List<int> Rijstroken => _rijstroken;

	    public string FaseCyclus
        {
            get => _faseCyclus;
		    set
            {
                _faseCyclus = value;
                _rijstroken = new List<int>();
                if (DefaultsProvider.Default.Controller != null)
                {
                    if (DefaultsProvider.Default.Controller.Fasen.Any(x => x.Naam == value))
                    {
                        var f = DefaultsProvider.Default.Controller.Fasen.First(x => x.Naam == value);
                        for (int i = 0; i < f.AantalRijstroken; ++i)
                        {
                            _rijstroken.Add(i + 1);
                        }
                    }
                }
            }
        }

        public string Naam
        {
            get => _detector.Naam;
	        set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, value))
                    {
                        string oldname = _detector.Naam;

                        _detector.Naam = value;
                        
                        // Notify the messenger
                        MessengerInstance.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Detector, oldname, _detector.Naam));
                    }
                }
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public string VissimNaam
        {
            get => _detector.VissimNaam;
	        set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, value, true))
                    {
                        _detector.VissimNaam = value;
                    }
                }
                RaisePropertyChanged<object>(nameof(VissimNaam), broadcast: true);
            }
        }

        public DetectorTypeEnum Type
        {
            get => _detector.Type;
	        set
            {
                _detector.Type = value;
                if(FaseCyclus != null && TLCGenControllerDataProvider.Default.Controller.Fasen.Any(x => x.Naam == FaseCyclus))
                {
                    var fctype = TLCGenControllerDataProvider.Default.Controller.Fasen.First(x => x.Naam == FaseCyclus).Type;
                    DefaultsProvider.Default.SetDefaultsOnModel(_detector, Type.ToString(), fctype.ToString());
                }
                else
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_detector, Type.ToString());
                }
                RaisePropertyChanged(string.Empty); // Update all properties
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
                Messenger.Default.Send(new FaseDetectorTypeChangedMessage(Naam, value));
            }
        }

        public bool IsDrukknop => Type == DetectorTypeEnum.Knop ||
                                  Type == DetectorTypeEnum.KnopBinnen || 
                                  Type == DetectorTypeEnum.KnopBuiten;

	    public int? TDB
        {
            get => _detector.TDB;
		    set
            {
                if (value == null || value >= 0)
                    _detector.TDB = value;
                RaisePropertyChanged<object>(nameof(TDB), broadcast: true);
            }
        }

        public int? TDH
        {
            get => _detector.TDH;
	        set
            {
                if (value == null || value >= 0)
                    _detector.TDH = value;
                RaisePropertyChanged<object>(nameof(TDH), broadcast: true);
            }
        }

        public int? TOG
        {
            get => _detector.TOG;
	        set
            {
                if (value == null || value >= 0)
                    _detector.TOG = value;
                RaisePropertyChanged<object>(nameof(TOG), broadcast: true);
            }
        }

        public int? TBG
        {
            get => _detector.TBG;
	        set
            {
                if (value == null || value >= 0)
                    _detector.TBG = value;
                RaisePropertyChanged<object>(nameof(TBG), broadcast: true);
            }
        }

        public int? TFL
        {
            get => _detector.TFL;
	        set
            {
                if (value == null || value >= 0)
                    _detector.TFL = value;
                RaisePropertyChanged<object>(nameof(TFL), broadcast: true);
            }
        }

        public int? CFL
        {
            get => _detector.CFL;
	        set
            {
                if (value == null || value >= 0)
                    _detector.CFL = value;
                RaisePropertyChanged<object>(nameof(CFL), broadcast: true);
            }
        }

        public DetectorAanvraagTypeEnum Aanvraag
        {
            get => _detector.Aanvraag;
	        set
            {
                _detector.Aanvraag = value;
                RaisePropertyChanged<object>(nameof(Aanvraag), broadcast: true);
            }
        }

        public DetectorVerlengenTypeEnum Verlengen
        {
            get => _detector.Verlengen;
	        set
            {
                _detector.Verlengen = value;
                RaisePropertyChanged<object>(nameof(Verlengen), broadcast: true);
            }
        }

        public bool AanvraagHardOpStraat
        {
            get => _detector.AanvraagHardOpStraat;
            set
            {
                _detector.AanvraagHardOpStraat = value;
                RaisePropertyChanged<object>(nameof(AanvraagHardOpStraat), broadcast: true);
            }
        }

        public bool VerlengenHardOpStraat
        {
            get => _detector.VerlengenHardOpStraat;
            set
            {
                _detector.VerlengenHardOpStraat = value;
                RaisePropertyChanged<object>(nameof(VerlengenHardOpStraat), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum AanvraagBijStoring
        {
            get => _detector.AanvraagBijStoring;
	        set
            {
                _detector.AanvraagBijStoring = value;
                RaisePropertyChanged<object>(nameof(AanvraagBijStoring), broadcast: true);
            }
        }

        public bool AanvraagDirect
        {
            get => _detector.AanvraagDirect;
	        set
            {
                _detector.AanvraagDirect = value;
                RaisePropertyChanged<object>(nameof(AanvraagDirect), broadcast: true);
            }
        }

        public bool Wachtlicht
        {
            get => _detector.Wachtlicht;
	        set
            {
                _detector.Wachtlicht = value;
                RaisePropertyChanged<object>(nameof(Wachtlicht), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum VeiligheidsGroen
        {
            get => _detector.VeiligheidsGroen;
	        set
            {
                _detector.VeiligheidsGroen = value;
                if (value != NooitAltijdAanUitEnum.Nooit)
                {
                    if (VeiligheidsGroenHiaat == 0) VeiligheidsGroenHiaat = 20;
                    if (VeiligheidsGroenVolgtijd == 0) VeiligheidsGroenVolgtijd = 25;
                }
                MessengerInstance.Send(new FaseDetectorVeiligheidsGroenChangedMessage(Naam, value));
                RaisePropertyChanged<object>(nameof(VeiligheidsGroen), broadcast: true);
            }
        }

        public int VeiligheidsGroenHiaat
        {
            get => _detector.VeiligheidsGroenHiaat;
            set
            {
                _detector.VeiligheidsGroenHiaat = value;
                RaisePropertyChanged<object>(nameof(VeiligheidsGroenHiaat), broadcast: true);
            }
        }

        public int VeiligheidsGroenVolgtijd
        {
            get => _detector.VeiligheidsGroenVolgtijd;
            set
            {
                _detector.VeiligheidsGroenVolgtijd = value;
                RaisePropertyChanged<object>(nameof(VeiligheidsGroenVolgtijd), broadcast: true);
            }
        }

        public bool ResetAanvraag
        {
            get => _detector.ResetAanvraag;
            set
            {
                _detector.ResetAanvraag = value;
                RaisePropertyChanged<object>(nameof(ResetAanvraag), broadcast: true);
            }
        }

        public int ResetAanvraagTijdsduur
        {
            get => _detector.ResetAanvraagTijdsduur;
            set
            {
                _detector.ResetAanvraagTijdsduur = value;
                RaisePropertyChanged<object>(nameof(ResetAanvraagTijdsduur), broadcast: true);
            }
        }

        public int Q1
        {
            get => _detector.Simulatie.Q1;
	        set
            {
                _detector.Simulatie.Q1 = value;
                RaisePropertyChanged<object>(nameof(Q1), broadcast: true);
            }
        }

        public int Q2
        {
            get => _detector.Simulatie.Q2;
	        set
            {
                _detector.Simulatie.Q2 = value;
                RaisePropertyChanged<object>(nameof(Q2), broadcast: true);
            }
        }

        public int Q3
        {
            get => _detector.Simulatie.Q3;
	        set
            {
                _detector.Simulatie.Q3 = value;
                RaisePropertyChanged<object>(nameof(Q3), broadcast: true);
            }
        }

        public int Q4
        {
            get => _detector.Simulatie.Q4;
	        set
            {
                _detector.Simulatie.Q4 = value;
                RaisePropertyChanged<object>(nameof(Q4), broadcast: true);
            }
        }

        public int Stopline
        {
            get => _detector.Simulatie.Stopline;
	        set
            {
                _detector.Simulatie.Stopline = value;
                RaisePropertyChanged<object>(nameof(Stopline), broadcast: true);
            }
        }

        public string FCNr
        {
            get => _detector.Simulatie.FCNr;
	        set
            {
                _detector.Simulatie.FCNr = value;
                RaisePropertyChanged<object>(nameof(FCNr), broadcast: true);
            }
        }

        public bool DetectorCanRequest =>
            FaseCyclus != null &&
            Type != DetectorTypeEnum.VecomDetector &&
            Type != DetectorTypeEnum.OpticomIngang &&
            Type != DetectorTypeEnum.WisselDetector;

        public bool DetectorCanRequestDirect =>
            FaseCyclus != null &&
            Type != DetectorTypeEnum.VecomDetector &&
            Type != DetectorTypeEnum.OpticomIngang &&
            Type != DetectorTypeEnum.WisselDetector &&
            Type != DetectorTypeEnum.Knop &&
            Type != DetectorTypeEnum.KnopBinnen &&
            Type != DetectorTypeEnum.KnopBuiten;

        public bool DetectorCanExtend =>
            FaseCyclus != null &&
            Type != DetectorTypeEnum.OpticomIngang &&
            Type != DetectorTypeEnum.VecomDetector &&
            Type != DetectorTypeEnum.WisselDetector;

        public bool DetectorCanHaveTDH =>
            Type != DetectorTypeEnum.VecomDetector &&
            Type != DetectorTypeEnum.WisselDetector &&
            Type != DetectorTypeEnum.Knop &&
            Type != DetectorTypeEnum.KnopBinnen &&
            Type != DetectorTypeEnum.KnopBuiten;

        public bool DetectorCanHaveTDB =>
            Type != DetectorTypeEnum.VecomDetector &&
            Type != DetectorTypeEnum.WisselDetector &&
            Type != DetectorTypeEnum.Knop &&
            Type != DetectorTypeEnum.KnopBinnen &&
            Type != DetectorTypeEnum.KnopBuiten;

        public bool DetectorCanHaveWaitingLight =>
            Type == DetectorTypeEnum.Knop ||
            Type == DetectorTypeEnum.KnopBuiten ||
            Type == DetectorTypeEnum.KnopBinnen;

        public bool DetectorCanHaveLaneNumber => 
            FaseCyclus != null &&
            Type != DetectorTypeEnum.OpticomIngang &&
            Type != DetectorTypeEnum.VecomDetector &&
            Type != DetectorTypeEnum.WisselDetector;

        public bool AanvraagDirectPossible => 
            FaseCyclus != null && (Type == DetectorTypeEnum.Kop || Type == DetectorTypeEnum.Lang);

	    #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is DetectorViewModel fcvm)) throw new InvalidCastException();
	        var myName = FaseCyclus == null ? Naam : Naam.Replace(FaseCyclus, "");
	        var hisName = fcvm.FaseCyclus == null ? fcvm.Naam : fcvm.Naam.Replace(fcvm.FaseCyclus, "");
            if (Regex.IsMatch(myName, @".*[a-zA-Z]$"))
            {
                if (!Regex.IsMatch(hisName, @".*[a-zA-Z]$"))
                {
                    hisName = hisName + "0";
                }
            }
            if (Regex.IsMatch(hisName, @".*[a-zA-Z]$"))
            {
                if (!Regex.IsMatch(myName, @".*[a-zA-Z]$"))
                {
                    myName = myName + "0";
                }
            }
            if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
	        else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
	        return string.Compare(
		        FaseCyclus == null ? myName : FaseCyclus + myName, 
		        fcvm.FaseCyclus == null ? hisName : fcvm.FaseCyclus + hisName, 
		        StringComparison.Ordinal);
        }

        #endregion // IComparable

        #region Constructor

        public DetectorViewModel(DetectorModel detector)
        {
            _detector = detector;
            // Fix potential missing value from old or invalid templates
            if (_detector.Rijstrook == null) _detector.Rijstrook = 1;
        }

        #endregion // Constructor
    }
}
