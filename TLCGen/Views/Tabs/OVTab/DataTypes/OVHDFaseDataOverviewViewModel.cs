﻿using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVHDFaseDataOverviewViewModel : ViewModelBase
	{
		#region Fields

		private FaseCyclusModel _faseCyclus;
		private OVIngreepViewModel _OVIngreep;
		private HDIngreepViewModel _HDIngreep;
		private ControllerModel _controller;
		private OVOverzichtTabViewModel _overVM;

		#endregion // Fields

		#region Properties

		public string FaseCyclusNaam => _faseCyclus.Naam;

		public bool HasOVIngreep
		{
			get => _faseCyclus.OVIngreep;
			set
			{
				_faseCyclus.OVIngreep = value;
				if (value)
				{
					var ov = new OVIngreepModel();
					Settings.DefaultsProvider.Default.SetDefaultsOnModel(ov);
					Settings.DefaultsProvider.Default.SetDefaultsOnModel(ov.MeldingenData);
					ov.FaseCyclus = _faseCyclus.Naam;
					ov.MeldingenData.Inmeldingen.Add(new OVIngreepInUitMeldingModel()
					{
						AntiJutterTijdToepassen = true,
						AntiJutterTijd = 15,
						InUit = OVIngreepInUitMeldingTypeEnum.Inmelding,
						Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
					});
					ov.MeldingenData.Uitmeldingen.Add(new OVIngreepInUitMeldingModel()
					{
						AntiJutterTijdToepassen = false,
						InUit = OVIngreepInUitMeldingTypeEnum.Uitmelding,
						Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
					});
					_controller.OVData.OVIngrepen.Add(ov);
					_controller.OVData.OVIngrepen.BubbleSort();
					OVIngreep = new OVIngreepViewModel(ov);
					OVIngreep.PropertyChanged += _overVM.OVIngreep_PropertyChanged;
                    /* Trick to add dummy detectors */
                    MessengerInstance.Send(new OVIngreepMeldingChangedMessage(ov.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
				}
				else
				{
					if (OVIngreep != null)
					{
						_controller.OVData.OVIngrepen.Remove(OVIngreep.OVIngreep);
						OVIngreep = null;
					}
					else
					{
						var ovi = _controller.OVData.OVIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
						if(ovi != null)
						{
							_controller.OVData.OVIngrepen.Remove(ovi);
						}
					}
				}
				MessengerInstance.Send(new OVIngrepenChangedMessage());
				RaisePropertyChanged();
			}
		}

        public bool HasOVIngreepKAR => OVIngreep.OVIngreep.HasOVIngreepKAR();

        public bool HasOVIngreepVECOM => OVIngreep.OVIngreep.HasOVIngreepVecom();

        public bool HasHDIngreep
		{
			get => _faseCyclus.HDIngreep;
			set
			{
				_faseCyclus.HDIngreep = value;
				if (value)
				{
					var hd = new HDIngreepModel();
					Settings.DefaultsProvider.Default.SetDefaultsOnModel(hd);
					hd.FaseCyclus = _faseCyclus.Naam;
					_controller.OVData.HDIngrepen.Add(hd);
					_controller.OVData.HDIngrepen.BubbleSort();
					HDIngreep = new HDIngreepViewModel(_controller, hd);
					HDIngreep.PropertyChanged += _overVM.HDIngreep_PropertyChanged;
					/* Trick to add dummy detectors */
					if (hd.KAR)
					{
						HDIngreep.KAR = true;
					}
				}
				else
				{
					if (HDIngreep != null)
					{
						_controller.OVData.HDIngrepen.Remove(HDIngreep.HDIngreep);
						HDIngreep = null;
					}
					else
					{
						var hdi = _controller.OVData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
						if (hdi != null)
						{
							_controller.OVData.HDIngrepen.Remove(hdi);
						}
					}
				}
				MessengerInstance.Send(new OVIngrepenChangedMessage());
				Integrity.TLCGenControllerModifier.Default.CorrectModel_AlteredHDIngrepen();
				RaisePropertyChanged();
			}
		}

		public OVIngreepViewModel OVIngreep
		{
			get => _OVIngreep;
			set
			{
				_OVIngreep = value;
				RaisePropertyChanged();
			}
		}

		public HDIngreepViewModel HDIngreep
		{
			get => _HDIngreep;
			set
			{
				_HDIngreep = value;
				RaisePropertyChanged();
			}
		}

		#endregion // Properties

		#region Constructor

		public OVHDFaseDataOverviewViewModel(FaseCyclusModel faseCyclus, OVOverzichtTabViewModel overvm, ControllerModel controller)
		{
			_faseCyclus = faseCyclus;
			_controller = controller;
			_overVM = overvm;
			if (_faseCyclus.OVIngreep)
			{
				var ovi = controller.OVData.OVIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
				if (ovi != null)
				{
					OVIngreep = new OVIngreepViewModel(ovi);
					OVIngreep.PropertyChanged += _overVM.OVIngreep_PropertyChanged;
		        
				}
			}
			if (_faseCyclus.HDIngreep)
			{
				var hdi = controller.OVData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
				if (hdi != null)
				{
					HDIngreep = new HDIngreepViewModel(controller, hdi);
					HDIngreep.PropertyChanged += _overVM.HDIngreep_PropertyChanged;
				}
			}
		}

		#endregion // Constructor
	}
}
