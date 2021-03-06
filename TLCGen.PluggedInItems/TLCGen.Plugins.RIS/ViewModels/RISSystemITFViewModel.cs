﻿using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISSystemITFViewModel : ViewModelBase, IViewModelWithItem
    {
        private RISSystemITFModel _model;

        public string SystemITF
        {
            get => _model.SystemITF;
            set
            {
                _model.SystemITF = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public RISSystemITFViewModel(RISSystemITFModel model)
        {
            _model = model;
        }

        public object GetItem()
        {
            return _model;
        }
    }
}
