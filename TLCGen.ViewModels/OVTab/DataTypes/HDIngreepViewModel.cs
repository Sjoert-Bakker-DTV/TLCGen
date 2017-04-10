﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class HDIngreepViewModel : ViewModelBase
    {
        #region Fields

        private HDIngreepModel _HDIngreep;
        private HDIngreepMeerealiserendeFaseCyclusViewModel _SelectedMeerealiserendeFase;
        private ObservableCollection<string> _Fasen;
        private string _SelectedFase;
        private ControllerModel _Controller;

        #endregion // Fields

        #region Properties

        public HDIngreepModel HDIngreep
        {
            get { return _HDIngreep; }
            set
            {
                _HDIngreep = value;
            }
        }

        [Category("Opties")]
        public bool KAR
        {
            get { return _HDIngreep.KAR; }
            set
            {
                _HDIngreep.KAR = value;
                OnMonitoredPropertyChanged("KAR");
                if (value)
                {
                    _HDIngreep.DummyKARInmelding.Naam = "dummyhdkarin" + _HDIngreep.FaseCyclus;
                    _HDIngreep.DummyKARUitmelding.Naam = "dummyhdkaruit" + _HDIngreep.FaseCyclus;
                }
                else
                {
                    _HDIngreep.DummyKARInmelding = new DetectorModel() { Dummy = true };
                    _HDIngreep.DummyKARUitmelding = new DetectorModel() { Dummy = true };
                    _HDIngreep.DummyKARInmelding = null;
                    _HDIngreep.DummyKARUitmelding = null;
                }
                Messenger.Default.Send(new OVIngrepenChangedMessage());
            }
        }

        public bool Opticom
        {
            get { return _HDIngreep.Opticom; }
            set
            {
                _HDIngreep.Opticom = value;
                OnMonitoredPropertyChanged("Opticom");
            }
        }

        public bool Sirene
        {
            get { return _HDIngreep.Sirene; }
            set
            {
                _HDIngreep.Sirene = value;
                OnMonitoredPropertyChanged("Sirene");
            }
        }

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get { return _HDIngreep.RijTijdOngehinderd; }
            set
            {
                _HDIngreep.RijTijdOngehinderd = value;
                OnMonitoredPropertyChanged("RijTijdOngehinderd");
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get { return _HDIngreep.RijTijdBeperktgehinderd; }
            set
            {
                _HDIngreep.RijTijdBeperktgehinderd = value;
                OnMonitoredPropertyChanged("RijTijdBeperktgehinderd");
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get { return _HDIngreep.RijTijdGehinderd; }
            set
            {
                _HDIngreep.RijTijdGehinderd = value;
                OnMonitoredPropertyChanged("RijTijdGehinderd");
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get { return _HDIngreep.GroenBewaking; }
            set
            {
                _HDIngreep.GroenBewaking = value;
                OnMonitoredPropertyChanged("GroenBewaking");
            }
        }

        [Browsable(false)]
        public ObservableCollection<string> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        [Browsable(false)]
        public string SelectedFase
        {
            get { return _SelectedFase; }
            set
            {
                _SelectedFase = value;
                OnPropertyChanged("SelectedFase");
            }
        }

        [Browsable(false)]
        public HDIngreepMeerealiserendeFaseCyclusViewModel SelectedMeerealiserendeFase
        {
            get { return _SelectedMeerealiserendeFase; }
            set
            {
                _SelectedMeerealiserendeFase = value;
                OnPropertyChanged("SelectedMeerealiserendeFase");
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<HDIngreepMeerealiserendeFaseCyclusViewModel, HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFasen
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddMeerealiserendeFaseCommand;
        public ICommand AddMeerealiserendeFaseCommand
        {
            get
            {
                if (_AddMeerealiserendeFaseCommand == null)
                {
                    _AddMeerealiserendeFaseCommand = new RelayCommand(AddNewMeerealiserendeFaseCommand_Executed, AddNewMeerealiserendeFaseCommand_CanExecute);
                }
                return _AddMeerealiserendeFaseCommand;
            }
        }


        RelayCommand _RemoveMeerealiserendeFaseCommand;
        public ICommand RemoveMeerealiserendeFaseCommand
        {
            get
            {
                if (_RemoveMeerealiserendeFaseCommand == null)
                {
                    _RemoveMeerealiserendeFaseCommand = new RelayCommand(RemoveMeerealiserendeFaseCommand_Executed, RemoveMeerealiserendeFaseCommand_CanExecute);
                }
                return _RemoveMeerealiserendeFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewMeerealiserendeFaseCommand_Executed(object prm)
        {
            if (!(MeerealiserendeFasen.Where(x => x.FaseCyclus.FaseCyclus == SelectedFase).Count() > 0))
            {
                MeerealiserendeFasen.Add(
                    new HDIngreepMeerealiserendeFaseCyclusViewModel(
                        new HDIngreepMeerealiserendeFaseCyclusModel() { FaseCyclus = SelectedFase }));
            }

            BuildFasenList();

            _HDIngreep.MeerealiserendeFaseCycli.BubbleSort();
            MeerealiserendeFasen.Rebuild();

            if (MeerealiserendeFasen.Count > 0)
                SelectedMeerealiserendeFase = MeerealiserendeFasen[MeerealiserendeFasen.Count - 1];
        }

        bool AddNewMeerealiserendeFaseCommand_CanExecute(object prm)
        {
            return MeerealiserendeFasen != null && SelectedFase != null;
        }

        void RemoveMeerealiserendeFaseCommand_Executed(object prm)
        {
            MeerealiserendeFasen.Remove(SelectedMeerealiserendeFase);

            BuildFasenList();

            _HDIngreep.MeerealiserendeFaseCycli.BubbleSort();
            MeerealiserendeFasen.Rebuild();

            if (MeerealiserendeFasen.Count > 0)
                SelectedMeerealiserendeFase = MeerealiserendeFasen[MeerealiserendeFasen.Count - 1];
            else
                SelectedMeerealiserendeFase = null;
        }

        bool RemoveMeerealiserendeFaseCommand_CanExecute(object prm)
        {
            return SelectedMeerealiserendeFase != null && MeerealiserendeFasen != null && MeerealiserendeFasen.Count > 0;
        }

        #endregion // Command functionality

        #region Private Methods
        
        private void BuildFasenList()
        {
            Fasen.Clear();
            foreach (FaseCyclusModel m in _Controller.Fasen)
            {
                if (m.Naam != _HDIngreep.FaseCyclus && 
                    m.HDIngreep &&
                    !(_HDIngreep.MeerealiserendeFaseCycli.Where(x => x.FaseCyclus == m.Naam).Count() > 0))
                    Fasen.Add(m.Naam);
            }
            if (Fasen.Count > 0)
            {
                SelectedFase = Fasen[0];
            }
        }

        #endregion // Private Methods

        #region Constructor

        public HDIngreepViewModel(ControllerModel controller, HDIngreepModel hdingreep)
        {
            _HDIngreep = hdingreep;
            _Controller = controller;

            BuildFasenList();

            MeerealiserendeFasen = new ObservableCollectionAroundList<HDIngreepMeerealiserendeFaseCyclusViewModel, HDIngreepMeerealiserendeFaseCyclusModel>(hdingreep.MeerealiserendeFaseCycli);
        }

        #endregion // Constructor
    }
}