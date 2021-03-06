﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenRichtingGevoeligTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private RichtingGevoeligeAanvraagViewModel _SelectedRichtingGevoeligeAanvraag;
        private RichtingGevoeligVerlengViewModel _SelectedRichtingGevoeligVerleng;

        private ObservableCollection<string> _AlleDetectoren;
        private ObservableCollection<string> _DetectorenAanvraag1;
        private ObservableCollection<string> _DetectorenAanvraag2;
        private ObservableCollection<string> _DetectorenVerleng1;
        private ObservableCollection<string> _DetectorenVerleng2;
        private ObservableCollection<string> _Fasen;

        private string _SelectedFaseAanvraag;
        private string _SelectedDetectorAanvraag1;
        private string _SelectedDetectorAanvraag2;
        private string _SelectedFaseVerleng;
        private string _SelectedDetectorVerleng1;
        private string _SelectedDetectorVerleng2;

        #endregion // Fields

        #region Properties

        public ObservableCollection<string> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        public ObservableCollection<string> DetectorenAanvraag1
        {
            get
            {
                if (_DetectorenAanvraag1 == null)
                {
                    _DetectorenAanvraag1 = new ObservableCollection<string>();
                }
                return _DetectorenAanvraag1;
            }
        }

        public ObservableCollection<string> DetectorenAanvraag2
        {
            get
            {
                if (_DetectorenAanvraag2 == null)
                {
                    _DetectorenAanvraag2 = new ObservableCollection<string>();
                }
                return _DetectorenAanvraag2;
            }
        }

        public ObservableCollection<string> DetectorenVerleng1
        {
            get
            {
                if (_DetectorenVerleng1 == null)
                {
                    _DetectorenVerleng1 = new ObservableCollection<string>();
                }
                return _DetectorenVerleng1;
            }
        }

        public ObservableCollection<string> DetectorenVerleng2
        {
            get
            {
                if (_DetectorenVerleng2 == null)
                {
                    _DetectorenVerleng2 = new ObservableCollection<string>();
                }
                return _DetectorenVerleng2;
            }
        }

        public ObservableCollection<string> AlleDetectoren
        {
            get
            {
                if (_AlleDetectoren == null)
                {
                    _AlleDetectoren = new ObservableCollection<string>();
                }
                return _AlleDetectoren;
            }
        }

        public string SelectedFaseAanvraag
        {
            get { return _SelectedFaseAanvraag; }
            set
            {
                _SelectedFaseAanvraag = value;
                _SelectedDetectorAanvraag1 = null;
                _SelectedDetectorAanvraag2 = null;
                DetectorenAanvraag1.Clear();
                var fc = Controller.Fasen.Where(x => x.Naam == _SelectedFaseAanvraag).FirstOrDefault();
                if (fc != null)
                {
                    DetectorenAanvraag1.AddRange(fc.Detectoren.Where(x => x.Type != Models.Enumerations.DetectorTypeEnum.Knop &&
                                                                          x.Type != Models.Enumerations.DetectorTypeEnum.KnopBinnen &&
                                                                          x.Type != Models.Enumerations.DetectorTypeEnum.KnopBuiten)
                                                              .Select(x => x.Naam));
                }
                _SelectedDetectorAanvraag1 = DetectorenAanvraag1.LastOrDefault();
                DetectorenAanvraag2.Clear();
                if (fc != null)
                {
                    DetectorenAanvraag2.AddRange(fc.Detectoren.Where(x => x.Type != Models.Enumerations.DetectorTypeEnum.Knop &&
                                                                          x.Type != Models.Enumerations.DetectorTypeEnum.KnopBinnen &&
                                                                          x.Type != Models.Enumerations.DetectorTypeEnum.KnopBuiten)
                                                              .Select(x => x.Naam));
                }
                _SelectedDetectorAanvraag2 = DetectorenAanvraag2.Where(x => x != SelectedDetectorAanvraag1).LastOrDefault();
                RaisePropertyChanged("SelectedFaseAanvraag");
                RaisePropertyChanged("SelectedDetectorAanvraag1");
                RaisePropertyChanged("SelectedDetectorAanvraag2");
            }
        }
        public string SelectedDetectorAanvraag1
        {
            get { return _SelectedDetectorAanvraag1; }
            set
            {
                _SelectedDetectorAanvraag1 = value;
                RaisePropertyChanged("SelectedDetectorAanvraag1");
            }
        }
        public string SelectedDetectorAanvraag2
        {
            get { return _SelectedDetectorAanvraag2; }
            set
            {
                _SelectedDetectorAanvraag2 = value;
                RaisePropertyChanged("SelectedDetectorAanvraag2");
            }
        }

        public string SelectedFaseVerleng
        {
            get { return _SelectedFaseVerleng; }
            set
            {
                _SelectedFaseVerleng = value;
                _SelectedDetectorVerleng1 = null;
                _SelectedDetectorVerleng2 = null;
                DetectorenVerleng1.Clear();
                var fc = Controller.Fasen.Where(x => x.Naam == _SelectedFaseVerleng).FirstOrDefault();
                if (fc != null)
                {
                    DetectorenVerleng1.AddRange(fc.Detectoren.Where(x => x.Type != Models.Enumerations.DetectorTypeEnum.Knop &&
                                                                         x.Type != Models.Enumerations.DetectorTypeEnum.KnopBinnen &&
                                                                         x.Type != Models.Enumerations.DetectorTypeEnum.KnopBuiten)
                                                             .Select(x => x.Naam));
                }
                _SelectedDetectorVerleng1 = DetectorenVerleng1.LastOrDefault();
                DetectorenVerleng2.Clear();
                if (fc != null)
                {
                    DetectorenVerleng2.AddRange(fc.Detectoren.Where(x => x.Type != Models.Enumerations.DetectorTypeEnum.Knop &&
                                                                         x.Type != Models.Enumerations.DetectorTypeEnum.KnopBinnen &&
                                                                         x.Type != Models.Enumerations.DetectorTypeEnum.KnopBuiten)
                                                             .Select(x => x.Naam));
                }
                _SelectedDetectorVerleng2 = DetectorenVerleng2.Where(x => x != SelectedDetectorVerleng1).LastOrDefault();
                RaisePropertyChanged("SelectedFaseVerleng");
                RaisePropertyChanged("SelectedDetectorVerleng1");
                RaisePropertyChanged("SelectedDetectorVerleng2");
            }
        }
        public string SelectedDetectorVerleng1
        {
            get { return _SelectedDetectorVerleng1; }
            set
            {
                _SelectedDetectorVerleng1 = value;
                RaisePropertyChanged("SelectedDetectorVerleng1");
            }
        }
        public string SelectedDetectorVerleng2
        {
            get { return _SelectedDetectorVerleng2; }
            set
            {
                _SelectedDetectorVerleng2 = value;
                RaisePropertyChanged("SelectedDetectorVerleng2");
            }
        }


        public ObservableCollectionAroundList<RichtingGevoeligeAanvraagViewModel, RichtingGevoeligeAanvraagModel> RichtingGevoeligeAanvragen
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<RichtingGevoeligVerlengViewModel, RichtingGevoeligVerlengModel> RichtingGevoeligVerlengen
        {
            get;
            private set;
        }

        public RichtingGevoeligeAanvraagViewModel SelectedRichtingGevoeligeAanvraag
        {
            get { return _SelectedRichtingGevoeligeAanvraag; }
            set
            {
                _SelectedRichtingGevoeligeAanvraag = value;
                RaisePropertyChanged("SelectedRichtingGevoeligeAanvraag");
            }
        }


        public RichtingGevoeligVerlengViewModel SelectedRichtingGevoeligVerleng
        {
            get { return _SelectedRichtingGevoeligVerleng; }
            set
            {
                _SelectedRichtingGevoeligVerleng = value;
                RaisePropertyChanged("SelectedRichtingGevoeligVerleng");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddRichtingGevoeligeAanvraag;
        public ICommand AddRichtingGevoeligeAanvraag
        {
            get
            {
                if (_AddRichtingGevoeligeAanvraag == null)
                {
                    _AddRichtingGevoeligeAanvraag = new RelayCommand(AddRichtingGevoeligeAanvraag_Executed, AddRichtingGevoeligeAanvraag_CanExecute);
                }
                return _AddRichtingGevoeligeAanvraag;
            }
        }

        RelayCommand _AddRichtingGevoeligVerleng;
        public ICommand AddRichtingGevoeligVerleng
        {
            get
            {
                if (_AddRichtingGevoeligVerleng == null)
                {
                    _AddRichtingGevoeligVerleng = new RelayCommand(AddRichtingGevoeligVerleng_Executed, AddRichtingGevoeligVerleng_CanExecute);
                }
                return _AddRichtingGevoeligVerleng;
            }
        }

        RelayCommand _RemoveRichtingGevoeligeAanvraag;
        public ICommand RemoveRichtingGevoeligeAanvraag
        {
            get
            {
                if (_RemoveRichtingGevoeligeAanvraag == null)
                {
                    _RemoveRichtingGevoeligeAanvraag = new RelayCommand(RemoveRichtingGevoeligeAanvraag_Executed, RemoveRichtingGevoeligeAanvraag_CanExecute);
                }
                return _RemoveRichtingGevoeligeAanvraag;
            }
        }

        RelayCommand _RemoveRichtingGevoeligVerleng;
        public ICommand RemoveRichtingGevoeligVerleng
        {
            get
            {
                if (_RemoveRichtingGevoeligVerleng == null)
                {
                    _RemoveRichtingGevoeligVerleng = new RelayCommand(RemoveRichtingGevoeligVerleng_Executed, RemoveRichtingGevoeligVerleng_CanExecute);
                }
                return _RemoveRichtingGevoeligVerleng;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private bool AddRichtingGevoeligeAanvraag_CanExecute(object prm)
        {
            return SelectedFaseAanvraag != null && SelectedDetectorAanvraag1 != null && SelectedDetectorAanvraag2 != null &&
                SelectedDetectorAanvraag1 != SelectedDetectorAanvraag2;
        }

        private void AddRichtingGevoeligeAanvraag_Executed(object prm)
        {
            var rga = new RichtingGevoeligeAanvraagModel()
            {
                FaseCyclus = SelectedFaseAanvraag,
                VanDetector = SelectedDetectorAanvraag1,
                NaarDetector = SelectedDetectorAanvraag2
            };
            DefaultsProvider.Default.SetDefaultsOnModel(rga);
            RichtingGevoeligeAanvragen.Add(new RichtingGevoeligeAanvraagViewModel(rga));

            _SelectedFaseAanvraag = null;
            _SelectedDetectorAanvraag1 = null;
            _SelectedDetectorAanvraag2 = null;
            RaisePropertyChanged("SelectedFaseAanvraag");
            RaisePropertyChanged("SelectedDetectorAanvraag1");
            RaisePropertyChanged("SelectedDetectorAanvraag2");

            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        private bool AddRichtingGevoeligVerleng_CanExecute(object prm)
        {
            return SelectedFaseVerleng != null && SelectedDetectorVerleng1 != null && SelectedDetectorVerleng2 != null;
        }

        private void AddRichtingGevoeligVerleng_Executed(object prm)
        {
            var rgv = new RichtingGevoeligVerlengModel()
            {
                FaseCyclus = SelectedFaseVerleng,
                VanDetector = SelectedDetectorVerleng1,
                NaarDetector = SelectedDetectorVerleng2
            };
            DefaultsProvider.Default.SetDefaultsOnModel(rgv);
            RichtingGevoeligVerlengen.Add(new RichtingGevoeligVerlengViewModel(rgv));

            _SelectedFaseVerleng = null;
            _SelectedDetectorVerleng1 = null;
            _SelectedDetectorVerleng2 = null;
            RaisePropertyChanged("SelectedFaseVerleng");
            RaisePropertyChanged("SelectedDetectorVerleng1");
            RaisePropertyChanged("SelectedDetectorVerleng2");

            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        private bool RemoveRichtingGevoeligeAanvraag_CanExecute(object prm)
        {
            return SelectedRichtingGevoeligeAanvraag != null;
        }

        private void RemoveRichtingGevoeligeAanvraag_Executed(object prm)
        {
            RichtingGevoeligeAanvragen.Remove(SelectedRichtingGevoeligeAanvraag);
            SelectedRichtingGevoeligeAanvraag = null;
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        private bool RemoveRichtingGevoeligVerleng_CanExecute(object prm)
        {
            return SelectedRichtingGevoeligVerleng != null;
        }

        private void RemoveRichtingGevoeligVerleng_Executed(object prm)
        {
            RichtingGevoeligVerlengen.Remove(SelectedRichtingGevoeligVerleng);
            SelectedRichtingGevoeligVerleng = null;
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Command Functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Richting\ngevoelig";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            Fasen.Clear();
            AlleDetectoren.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(fcm.Naam);
                foreach(DetectorModel dm in fcm.Detectoren)
                {
                    AlleDetectoren.Add(dm.Naam);
                }
            }
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                if(base.Controller != null)
                {
                    RichtingGevoeligeAanvragen = new ObservableCollectionAroundList<RichtingGevoeligeAanvraagViewModel, RichtingGevoeligeAanvraagModel>(base.Controller.RichtingGevoeligeAanvragen);
                    RichtingGevoeligVerlengen = new ObservableCollectionAroundList<RichtingGevoeligVerlengViewModel, RichtingGevoeligVerlengModel>(base.Controller.RichtingGevoeligVerlengen);
                }
                else
                {
                    RichtingGevoeligeAanvragen = null;
                    RichtingGevoeligVerlengen = null;
                }
                RaisePropertyChanged("RichtingGevoeligeAanvragen");
                RaisePropertyChanged("RichtingGevoeligVerlengen");
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            RichtingGevoeligeAanvragen.Rebuild();
            RichtingGevoeligVerlengen.Rebuild();
        }

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            RichtingGevoeligeAanvragen.Rebuild();
            RichtingGevoeligVerlengen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public DetectorenRichtingGevoeligTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }


        #endregion // Constructor
    }
}
