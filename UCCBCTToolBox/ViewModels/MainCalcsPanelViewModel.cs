using System;
using System.Collections.Generic;
using System.Diagnostics;
using UCCBCTToolBox.Classes;
using UCCBCTToolBox.Notifiers;

namespace UCCBCTToolBox.ViewModels
{
    internal class MainCalcsPanelViewModel : BindableBase
    {
        #region Private Properties
        private double _headTiltInput;
        private double _sLMInput;
        private double _iLMInput;
        private double _cSTInput;
        private double _aAInput;
        private double _c2Input;
        private double _cInput;
        private double _aInput;
        private bool _hTModeR;
        private bool _hTModeL;
        private bool _sLMModeR;
        private bool _sLMModeL;
        private bool _iLMModeR;
        private bool _iLMModeL;
        private bool _cSTModeR;
        private bool _cSTModeL;
        private bool _aAModeR;
        private bool _aAModeL;
        private bool _c2ModeR;
        private bool _c2ModeL;

        private string _fACText;
        private string _fASText;
        private string _fAText;
        private string _hACText;
        private string _hAxText;

        private List<string> _translations;
        private int _selectedTranslation;
        private Dictionary<string, List<string>> _categorizedTranslations;
        #endregion

        #region Public Static Properties
        public List<string> Translations
        {
            get => _translations;
            set => SetProperty(ref _translations, value);
        }
        public int SelectedTranslation
        {
            get => _selectedTranslation;
            set
            {
                SetProperty(ref _selectedTranslation, value);

                RaisePropertyChanged(nameof(FACLabel));
                RaisePropertyChanged(nameof(FASLabel));
                RaisePropertyChanged(nameof(FALabel));
                RaisePropertyChanged(nameof(HACLabel));
                RaisePropertyChanged(nameof(HaxLabel));
            }
        }
        public string FACLabel => _categorizedTranslations[Translations[SelectedTranslation]][0];
        public string FASLabel => _categorizedTranslations[Translations[SelectedTranslation]][1];
        public string FALabel => _categorizedTranslations[Translations[SelectedTranslation]][2];
        public string HACLabel => _categorizedTranslations[Translations[SelectedTranslation]][3];
        public string HaxLabel => _categorizedTranslations[Translations[SelectedTranslation]][4];
        public static double CValue { get; set; }
        public static double AValue { get; set; }
        public static double HTValue { get; set; }
        #endregion

        #region Constructors
        public MainCalcsPanelViewModel()
        {
            _hTModeR = true;
            _sLMModeR = true;
            _iLMModeR = true;
            _cSTModeR = true;
            _aAModeR = true;
            _c2ModeR = true;

            _fACText = "R/L";
            _fASText = "R/L";
            _fAText = "+/-";
            _hACText = "A/P";
            _hAxText = "R/L";
            _cInput = 0;
            _aInput = 0;

            _translations = new() { "EPIC", "Advanced Orthogonal", "Orthospinology", "NUCCA" };
            List<string> t0 = new() { "FAC", "FAS", "FA", "HAC", "Hax" },
            t1 = new() { "ACD", "CS", "AFP", "AHRy", "AXSP" },
            t2 = new() { "Laterality", "Lower Angle", "APL", "C1 Rotation", "AxSP" },
            t3 = new() { "Laterality", "Lower Angle", "APL", "C1 rotation", "AXSP" };
            _categorizedTranslations = new()
            {
                { Translations[0], t0 },
                { Translations[1], t1 },
                { Translations[2], t2 },
                { Translations[3], t3 }
            };
        }
        #endregion

        #region Public Properties
        public double HeadTiltInput
        {
            get { return _headTiltInput; }
            set { SetProperty(ref _headTiltInput, value); RaisePropertyChanged(nameof(FACCalc)); }
        }
        public bool HTModeR
        {
            get { return _hTModeR; }
            set 
            {
                SetProperty(ref _hTModeR, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }
        public bool HTModeL
        {
            get { return _hTModeL; }
            set { SetProperty(ref _hTModeL, value); RaisePropertyChanged(nameof(FACCalc)); }
        }
        public double SLMInput
        {
            get { return _sLMInput; }
            set { SetProperty(ref _sLMInput, value); RaisePropertyChanged(nameof(FACCalc)); }
        }
        public bool SLMModeR
        {
            get { return _sLMModeR; }
            set
            {
                SetProperty(ref _sLMModeR, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }
        public bool SLMModeL
        {
            get { return _sLMModeL; }
            set { SetProperty(ref _sLMModeL, value); RaisePropertyChanged(nameof(FACCalc)); }
        }
        public double ILMInput
        {
            get { return _iLMInput; }
            set { SetProperty(ref _iLMInput, value); RaisePropertyChanged(nameof(FASCalc)); RaisePropertyChanged(nameof(FACalc)); }
        }
        public bool ILMModeR
        {
            get { return _iLMModeR; }
            set { SetProperty(ref _iLMModeR, value); RaisePropertyChanged(nameof(FASCalc)); RaisePropertyChanged(nameof(FACalc)); }
        }
        public bool ILMModeL
        {
            get { return _iLMModeL; }
            set { SetProperty(ref _iLMModeL, value); RaisePropertyChanged(nameof(FASCalc)); RaisePropertyChanged(nameof(FACalc)); }
        }
        public double CSTInput
        {
            get { return _cSTInput; }
            set { SetProperty(ref _cSTInput, value); RaisePropertyChanged(nameof(FASCalc)); }
        }
        public bool CSTModeR
        {
            get { return _cSTModeR; }
            set { SetProperty(ref _cSTModeR, value); RaisePropertyChanged(nameof(FASCalc)); RaisePropertyChanged(nameof(ARText)); }
        }
        public bool CSTModeL
        {
            get { return _cSTModeL; }
            set { SetProperty(ref _cSTModeL, value); RaisePropertyChanged(nameof(FASCalc)); RaisePropertyChanged(nameof(ARText)); }
        }
        public double AAInput
        {
            get { return _aAInput; }
            set { SetProperty(ref _aAInput, value); RaisePropertyChanged(nameof(HACCalc)); }
        }
        public bool AAModeR
        {
            get { return _aAModeR; }
            set
            {
                SetProperty(ref _aAModeR, value);
                RaisePropertyChanged(nameof(HACCalc));
            }
        }
        public bool AAModeL
        {
            get { return _aAModeL; }
            set
            {
                SetProperty(ref _aAModeL, value);
                RaisePropertyChanged(nameof(HACCalc));
            }
        }
        public double C2Input
        {
            get { return _c2Input; }
            set
            {
                SetProperty(ref _c2Input, value);
                RaisePropertyChanged(nameof(HAxCalc));
            }
        }
        public bool C2ModeR
        {
            get 
            {
                return _c2ModeR;
            }
            set
            {
                SetProperty(ref _c2ModeR, value);
                RaisePropertyChanged(nameof(HAxCalc));
            }
        }
        public bool C2ModeL
        {
            get { return _c2ModeL; }
            set { SetProperty(ref _c2ModeL, value); RaisePropertyChanged(nameof(HAxCalc)); }
        }
        public double CInput
        {
            get { return _cInput; }
            set { SetProperty(ref _cInput, value); }
        }
        public double AInput
        {
            get { return _aInput; }
            set { SetProperty(ref _aInput, value); }
        }
        public double FACCalc
        {
            get
            {
                double sum = 0;
                if (HTModeR)
                    sum += HeadTiltInput;
                if (HTModeL)
                    sum -= HeadTiltInput;
                if (SLMModeR)
                    sum += SLMInput;
                if (SLMModeL)
                    sum -= SLMInput;

                SetConditionalStrings(new bool[] { sum >= 0.0, sum < 0.0 }, new string[] { "R", "L" }, ref _fACText);
                RaisePropertyChanged(nameof(FACText));
                RaisePropertyChanged(nameof(FACalc));
                RaisePropertyChanged(nameof(HACCalc));

                return Math.Round(Math.Abs(sum), 2);
            }
        }
        public double FASCalc
        {
            get
            {
                double sum = 0;
                if (ILMModeL)
                    sum += ILMInput;
                if (ILMModeR)
                    sum -= ILMInput;
                if (CSTModeL)
                    sum += CSTInput;
                if (CSTModeR)
                    sum -= CSTInput;

                SetConditionalStrings(new bool[] { sum >= 0.0, sum < 0.0 }, new string[] { "R", "L" }, ref _fASText);
                RaisePropertyChanged(nameof(FASText));

                return Math.Round(Math.Abs(sum), 2);
            }
        }
        public double FACalc
        {
            get
            {
                double sum = 0;
                if (ILMModeL)
                    sum -= ILMInput;
                if (ILMModeR)
                    sum += ILMInput;
                if (SLMModeR)
                    sum += SLMInput;
                if (SLMModeL)
                    sum -= SLMInput;

                sum /= 2.0;
                if (FACText != null && FACText == "L")
                    sum *= -1.0;

                SetConditionalStrings(new bool[] { sum >= 0.0, sum < 0.0 }, new string[] { "+", "-" }, ref _fAText);
                RaisePropertyChanged(nameof(FAText));

                return Math.Round(Math.Abs(sum), 2);
            }
        }

        public double HACCalc
        {
            get
            {
                SetConditionalStrings(new bool[] { (FACText == "R" && AAModeR) || (FACText == "L" && AAModeL), (FACText == "L" && AAModeR) || (FACText == "R" && AAModeL) },
                    new string[] { "A", "P" }, ref _hACText);
                RaisePropertyChanged(nameof(HACText));

                return Math.Round(AAInput, 2);
            }
        }

        public double HAxCalc
        {
            get
            {
                SetConditionalStrings(new bool[] { C2ModeR, C2ModeL }, new string[] { "R", "L" }, ref _hAxText);
                RaisePropertyChanged(nameof(HAxText));

                return Math.Round(C2Input, 2);
            }
        }

        // Orthospinology public properties
        public double HF => (FACalc * 0.4) + ((AInput - CInput) / 2.0) + (FACText != FASText ? (Math.Abs(FACCalc - FASCalc) * 0.25) : ((FACCalc - FASCalc) * 0.25));

        public double RA => Math.Round(Math.Asin(Math.Sqrt(Math.Pow(HF, 2.0) + Math.Pow(HACCalc, 2.0)) / 20.0) * (180.0 / Math.PI), 2);

        public string FACText
        {
            get { return _fACText; }
        }
        public string FASText
        {
            get { return _fASText; }
        }
        public string FAText
        {
            get { return _fAText; }
        }
        public string HACText
        {
            get { return _hACText; }
        }
        public string HAxText
        {
            get { return _hAxText; }
        }
        public string ARText
        {
            get { return CSTModeR ? "R" : "L"; }
        }
        #endregion

        #region Public Methods
        public void ResetAllInput()
        {
            HeadTiltInput = 0;
            HTModeR = true;
            HTModeL = false;
            SLMInput = 0;
            SLMModeR = true;
            SLMModeL = false;
            ILMInput = 0;
            ILMModeR = true;
            ILMModeL = false;
            CSTInput = 0;
            CSTModeR = true;
            CSTModeL = false;
            AAInput = 0;
            AAModeR = true;
            AAModeL = false;
            C2Input = 0;
            C2ModeR = true;
            C2ModeL = false;
            AInput = 0;
            CInput = 0;

            // Reset Statics
            HTValue = 0;
            AValue = 0;
            CValue = 0;
        }
        public void Autoload()
        {
            Tool current = Global.CurrentTool;
            Debug.WriteLine("Autoload called, active tool is " + current.Name);
            if (current.Name == ThreePointCircle.ToolName)
            {
                if (ThreePointCircle.ToolType == ThreePointCircle.CTool)
                {
                    CInput = CValue;
                }
                if (ThreePointCircle.ToolType == ThreePointCircle.ATool)
                {
                    AInput = AValue;
                }
            }
            if (current.Name == HAVATool.ToolName)
            {
                HeadTiltInput = Math.Round(Math.Abs(HTValue), 2);
                HTModeL = HTValue < 0;
                HTModeR = HTValue >= 0;
            }
        }
        #endregion

        #region Private Methods
        private void SetConditionalStrings(bool[] conditions, string[] correspondingValues, ref string storage)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i])
                {
                    storage = correspondingValues[i];
                    break;
                }
            }
        }

        internal void OnOrthospinologyCalculateClick()
        {
            RaisePropertyChanged(nameof(HF));
            RaisePropertyChanged(nameof(RA));
        }
        #endregion
    }
}
