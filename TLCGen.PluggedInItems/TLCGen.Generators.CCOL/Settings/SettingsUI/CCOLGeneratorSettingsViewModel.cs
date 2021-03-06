﻿using System.Collections.Generic;
using GalaSoft.MvvmLight;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;

namespace TLCGen.Generators.CCOL
{
    public class CCOLGeneratorSettingsViewModel : ViewModelBase
    {
        #region Fields

        private CCOLGeneratorSettingsModel _Settings;
        private CCOLGenerator _Generator;

        #endregion // Fields

        #region Properties

        public CCOLGeneratorVisualSettingsModel VisualSettings
        {
            get
            {
                return _Settings?.VisualSettings;
            }
        }

        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL9
        {
            get
            {
                return _Settings?.VisualSettingsCCOL9;
            }
        }


        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL95
        {
            get
            {
                return _Settings?.VisualSettingsCCOL95;
            }
        }

        public List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>> CodePieceGeneratorSettings
        {
            get
            {
                return _Settings?.CodePieceGeneratorSettings;
            }
        }

        public List<CCOLGeneratorCodeStringSettingModel> Prefixes
        {
            get
            {
                return _Settings?.Prefixes;
            }
        }

        public string TabSpace
        {
            get { return _Settings.TabSpace; }
            set
            {
                _Settings.TabSpace = value;
                RaisePropertyChanged("TabSpace");
            }
        }

        #endregion // Properties

        #region Commands

		/* 
		 * For potential future use
		 * 
        RelayCommand _saveSettingsCommand;
        public ICommand SaveSettingsCommand
        {
            get
            {
                if (_saveSettingsCommand == null)
                {
                    _saveSettingsCommand = new RelayCommand(SaveSettingsCommand_Executed, SaveSettingsCommand_CanExecute);
                }
                return _saveSettingsCommand;
            }
        }

        private void SaveSettingsCommand_Executed(object obj)
        {
            throw new NotImplementedException();
        }

        private bool SaveSettingsCommand_CanExecute(object obj)
        {
            throw new NotImplementedException();
        }

		*/

        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Constructor

        public CCOLGeneratorSettingsViewModel(CCOLGeneratorSettingsModel settings, CCOLGenerator generator)
        {
            _Settings = settings;
            _Generator = generator;
        }

        #endregion // Constructor
    }
}
