using System;
using System.Windows;
using UCCBCTToolBox.Notifiers;

namespace UCCBCTToolBox.ViewModels
{
    internal class ProductRegistrationViewModel : BindableBase
    {
        #region Private Properties
        private string _serial;
        private string _errmsg;
        private string _sucmsg;
        private DateTime _trialEndDate;
        private DateTime _regDate;
        #endregion

        public ProductRegistrationViewModel()
        {
            _serial = string.Empty;
            _errmsg = string.Empty;
            _sucmsg = string.Empty;
            _trialEndDate = DateTime.MinValue;
            _regDate = DateTime.MaxValue;
        }

        #region Public Properties
        public string Serial
        {
            get { return _serial; }
            set
            {
                SetProperty(ref _serial, value);
                RaisePropertyChanged(nameof(SuccessBlockVisibility));
                RaisePropertyChanged(nameof(TrialBlockVisibility));
                RaisePropertyChanged(nameof(FailureBlockVisibility));
            }
        }
        public DateTime TrialEnd
        {
            get { return _trialEndDate; }
            set { SetProperty(ref _trialEndDate, value); }
        }
        public DateTime RegDate
        {
            get { return _regDate; }
            set { SetProperty(ref _regDate, value); }
        }
        public string ErrorMessage
        {
            get => _errmsg;
            set { SetProperty(ref _errmsg, value); }
        }

        public string SuccessMessage
        {
            get => _sucmsg;
            set { SetProperty(ref _sucmsg, value); }
        }
        public bool IsTrialEnd => TrialEnd < DateTime.Now;
        public bool IsLicenseValid => Global.GetSavedValueFromRegistry("Serial") != null;
        public Visibility SuccessBlockVisibility => IsLicenseValid ? Visibility.Visible : Visibility.Collapsed;
        public Visibility FailureBlockVisibility => Serial != string.Empty && !IsLicenseValid ? Visibility.Visible : Visibility.Collapsed;
        public Visibility TrialBlockVisibility => IsLicenseValid ? Visibility.Collapsed : Visibility.Visible;
        #endregion
    }
}
