﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<FaseCyclusModel>
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private bool _IsSorting = false;
        private IList _SelectedFaseCycli = new ArrayList();

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusViewModel>();
                }
                return _Fasen;
            }
        }

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                RaisePropertyChanged("SelectedFaseCyclus");
                if (value != null) TemplatesProviderVM.SetSelectedApplyToItem(value.FaseCyclus);
            }
        }

        public IList SelectedFaseCycli
        {
            get { return _SelectedFaseCycli; }
            set
            {
                _SelectedFaseCycli = value;
                _SettingMultiple = false;
                RaisePropertyChanged("SelectedFaseCycli");
                if (value != null)
                {
                    var sl = new List<FaseCyclusModel>();
                    foreach(var s in value)
                    {
                        sl.Add((s as FaseCyclusViewModel).FaseCyclus);
                    }
                    TemplatesProviderVM.SetSelectedApplyToItems(sl);
                }
            }
        }

        private TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel> _TemplatesProviderVM;
        public TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel> TemplatesProviderVM
        {
            get
            {
                if (_TemplatesProviderVM == null)
                {
                    _TemplatesProviderVM = new TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel>(this);
                }
                return _TemplatesProviderVM;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddFaseCommand;
        public ICommand AddFaseCommand
        {
            get
            {
                if (_AddFaseCommand == null)
                {
                    _AddFaseCommand = new RelayCommand(AddNewFaseCommand_Executed, AddNewFaseCommand_CanExecute);
                }
                return _AddFaseCommand;
            }
        }


        RelayCommand _RemoveFaseCommand;
        public ICommand RemoveFaseCommand
        {
            get
            {
                if (_RemoveFaseCommand == null)
                {
                    _RemoveFaseCommand = new RelayCommand(RemoveFaseCommand_Executed, RemoveFaseCommand_CanExecute);
                }
                return _RemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewFaseCommand_Executed(object prm)
        {
            var fcm = new FaseCyclusModel();

            string newname;
            var inext = 0;
            foreach (var fcvm in Fasen)
            {
                if (int.TryParse(fcvm.Naam, out int inewname))
                {
                    inext = inewname > inext ? inewname : inext;
                }
            }
            do
            {
                inext++;
                newname = (inext < 10 ? "0" : "") + inext;
            }
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Fase, newname));

            fcm.Naam = newname;
            fcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(fcm.Naam);
            DefaultsProvider.Default.SetDefaultsOnModel(fcm, fcm.Type.ToString());
            var fcvm1 = new FaseCyclusViewModel(fcm);
            Fasen.Add(fcvm1);
        }

        bool AddNewFaseCommand_CanExecute(object prm)
        {
            return Fasen != null;
        }

        void RemoveFaseCommand_Executed(object prm)
        {
            bool changed = false;
            List<FaseCyclusModel> remfcs = new List<FaseCyclusModel>();
            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 0)
            {
                changed = true;
                foreach (FaseCyclusViewModel fcvm in SelectedFaseCycli)
                {
                    TLCGenControllerModifier.Default.RemoveSignalGroupFromController(fcvm.Naam);
                    remfcs.Add(fcvm.FaseCyclus);
                }

                SelectedFaseCycli = null;
            }
            else if (SelectedFaseCyclus != null)
            {
                changed = true;
                remfcs.Add(SelectedFaseCyclus.FaseCyclus);
                TLCGenControllerModifier.Default.RemoveSignalGroupFromController(SelectedFaseCyclus.Naam);
                SelectedFaseCyclus = null;
            }

            if(changed)
            {
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.Clear();
                foreach (var fc in _Controller.Fasen)
                {
                    Fasen.Add(new FaseCyclusViewModel(fc));
                }
                Fasen.CollectionChanged += Fasen_CollectionChanged;
                Messenger.Default.Send(new FasenChangedMessage(null, remfcs));
            }

        }

        bool RemoveFaseCommand_CanExecute(object prm)
        {
            return Fasen != null &&
                (SelectedFaseCyclus != null ||
                 SelectedFaseCycli != null && SelectedFaseCycli.Count > 0);
        }

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Overzicht";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
        }

        public override bool OnDeselectedPreview()
        {
            if (!Fasen.IsSorted())
            {
                _IsSorting = true;
                Fasen.BubbleSort();
                _Controller.Fasen.Clear();
                foreach(var fcvm in Fasen)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                }
                Messenger.Default.Send(new FasenSortedMessage(_Controller.Fasen));
                _IsSorting = false;
            }
            return true;
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
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.Clear();
                if (base.Controller != null)
                {
                    foreach (FaseCyclusModel fcm in base.Controller.Fasen)
                    {
                        var fcvm = new FaseCyclusViewModel(fcm);
                        fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
                        Fasen.Add(fcvm);
                    }
                    Fasen.CollectionChanged += Fasen_CollectionChanged;
                }
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Event handling

        private void OnFaseDetectorTypeChanged(FaseDetectorTypeChangedMessage message)
        {
            foreach (var fcm in Fasen)
            {
                fcm.UpdateHasKopmax();
            }
        }

        private void OnFaseDetectorVeiligheidsGroenChanged(FaseDetectorVeiligheidsGroenChangedMessage message)
        {
            foreach (var fcm in Fasen)
            {
                fcm.UpdateHasVeiligheidsGroen();
            }
        }

        #endregion // TLCGen Event handling

        #region Event handling

        private bool _SettingMultiple = false;
        private void FaseCyclus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<FaseCyclusViewModel>(sender, e.PropertyName, SelectedFaseCycli);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.NewItems)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                    fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.OldItems)
                {
                    _Controller.Fasen.Remove(fcvm.FaseCyclus);
                }
            }

            List<FaseCyclusModel> removedfasen = new List<FaseCyclusModel>();
            if (e.OldItems != null)
            {
                foreach (FaseCyclusViewModel item in e.OldItems)
                {
                    removedfasen.Add(item.FaseCyclus);
                }
            }

            List<FaseCyclusModel> addedfasen = new List<FaseCyclusModel>();
            if (e.NewItems != null)
            {
                foreach (FaseCyclusViewModel item in e.NewItems)
                {
                    addedfasen.Add(item.FaseCyclus);
                }
            }

            if (!_IsSorting)
            {
                Messenger.Default.Send(new FasenChangingMessage(addedfasen, removedfasen));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                Messenger.Default.Send(new ControllerDataChangedMessage());
            }
        }

        #endregion // Collection Changed

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<FaseCyclusModel> items)
        {
            if (_Controller == null)
                return;

            foreach (var fc in items)
            {
                if (!(TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, fc.Naam, TLCGenObjectTypeEnum.Fase) &&
                     (fc.Detectoren.Count == 0 || fc.Detectoren.All(x => TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, x.Naam, TLCGenObjectTypeEnum.Detector) != false))))
                {
                    MessageBox.Show("Error bij toevoegen van fase met naam " + fc.Naam + ".\nFase en/of bijbehorende detectie is niet uniek in de regeling.", "Error bij toepassen template");
                    return;
                }
            }
            foreach(var fc in items)
            {
                Fasen.Add(new FaseCyclusViewModel(fc));
            }
        }

        public void UpdateAfterApplyTemplate(FaseCyclusModel item)
        {
            var fc = Fasen.First(x => x.FaseCyclus == item);
            fc.RaisePropertyChanged("");
			Messenger.Default.Send(new DetectorenChangedMessage(null, null));
        }

        #endregion // IAllowTemplates

        #region Constructor

        public FasenLijstTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FaseDetectorTypeChangedMessage>(OnFaseDetectorTypeChanged));
            Messenger.Default.Register(this, new Action<FaseDetectorVeiligheidsGroenChangedMessage>(OnFaseDetectorVeiligheidsGroenChanged));
        }

        #endregion // Constructor
    }
}
