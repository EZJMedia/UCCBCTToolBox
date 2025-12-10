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
            get => _headTiltInput;
            set
            {
                SetProperty(ref _headTiltInput, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }

        public bool HTModeR
        {
            get => _hTModeR;
            set
            {
                SetProperty(ref _hTModeR, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }

        public bool HTModeL
        {
            get => _hTModeL;
            set
            {
                SetProperty(ref _hTModeL, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }

        public double SLMInput
        {
            get => _sLMInput;
            set
            {
                SetProperty(ref _sLMInput, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }

        public bool SLMModeR
        {
            get => _sLMModeR;
            set
            {
                SetProperty(ref _sLMModeR, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }

        public bool SLMModeL
        {
            get => _sLMModeL;
            set
            {
                SetProperty(ref _sLMModeL, value);
                RaisePropertyChanged(nameof(FACCalc));
            }
        }

        public double ILMInput
        {
            get => _iLMInput;
            set
            {
                SetProperty(ref _iLMInput, value);
                RaisePropertyChanged(nameof(FASCalc));
                RaisePropertyChanged(nameof(FACalc));
            }
        }

        public bool ILMModeR
        {
            get => _iLMModeR;
            set
            {
                SetProperty(ref _iLMModeR, value);
                RaisePropertyChanged(nameof(FASCalc));
                RaisePropertyChanged(nameof(FACalc));
            }
        }

        public bool ILMModeL
        {
            get => _iLMModeL;
            set
            {
                SetProperty(ref _iLMModeL, value);
                RaisePropertyChanged(nameof(FASCalc));
                RaisePropertyChanged(nameof(FACalc));
            }
        }

        public double CSTInput
        {
            get => _cSTInput;
            set
            {
                SetProperty(ref _cSTInput, value);
                RaisePropertyChanged(nameof(FASCalc));
            }
        }

        public bool CSTModeR
        {
            get => _cSTModeR;
            set
            {
                SetProperty(ref _cSTModeR, value);
                RaisePropertyChanged(nameof(FASCalc));
                RaisePropertyChanged(nameof(ARText));
            }
        }

        public bool CSTModeL
        {
            get => _cSTModeL;
            set
            {
                SetProperty(ref _cSTModeL, value);
                RaisePropertyChanged(nameof(FASCalc));
                RaisePropertyChanged(nameof(ARText));
            }
        }

        public double AAInput
        {
            get => _aAInput;
            set
            {
                SetProperty(ref _aAInput, value);
                RaisePropertyChanged(nameof(HACCalc));
            }
        }

        public bool AAModeR
        {
            get => _aAModeR;
            set
            {
                SetProperty(ref _aAModeR, value);
                RaisePropertyChanged(nameof(HACCalc));
            }
        }

        public bool AAModeL
        {
            get => _aAModeL;
            set
            {
                SetProperty(ref _aAModeL, value);
                RaisePropertyChanged(nameof(HACCalc));
            }
        }

        public double C2Input
        {
            get => _c2Input;
            set
            {
                SetProperty(ref _c2Input, value);
                RaisePropertyChanged(nameof(HAxCalc));
            }
        }

        public bool C2ModeR
        {
            get => _c2ModeR;
            set
            {
                SetProperty(ref _c2ModeR, value);
                RaisePropertyChanged(nameof(HAxCalc));
            }
        }

        public bool C2ModeL
        {
            get => _c2ModeL;
            set
            {
                SetProperty(ref _c2ModeL, value);
                RaisePropertyChanged(nameof(HAxCalc));
            }
        }

        public double CInput
        {
            get => _cInput;
            set => SetProperty(ref _cInput, value);
        }

        public double AInput
        {
            get => _aInput;
            set => SetProperty(ref _aInput, value);
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

                SetConditionalStrings(
                    new bool[] { sum >= 0.0, sum < 0.0 },
                    new string[] { "R", "L" },
                    ref _fACText);

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

                SetConditionalStrings(
                    new bool[] { sum >= 0.0, sum < 0.0 },
                    new string[] { "R", "L" },
                    ref _fASText);

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

                SetConditionalStrings(
                    new bool[] { sum >= 0.0, sum < 0.0 },
                    new string[] { "+", "-" },
                    ref _fAText);

                RaisePropertyChanged(nameof(FAText));

                return Math.Round(Math.Abs(sum), 2);
            }
        }

        public double HACCalc
        {
            get
            {
                SetConditionalStrings(
                    new bool[]
                    {
                        (FACText == "R" && AAModeR) || (FACText == "L" && AAModeL),
                        (FACText == "L" && AAModeR) || (FACText == "R" && AAModeL)
                    },
                    new string[] { "A", "P" },
                    ref _hACText);

                RaisePropertyChanged(nameof(HACText));

                return Math.Round(AAInput, 2);
            }
        }

        public double HAxCalc
        {
            get
            {
                SetConditionalStrings(
                    new bool[] { C2ModeR, C2ModeL },
                    new string[] { "R", "L" },
                    ref _hAxText);

                RaisePropertyChanged(nameof(HAxText));

                return Math.Round(C2Input, 2);
            }
        }

        // Orthospinology public properties
        public double HF
        {
            get
            {
                // Get signed versions of FA (FACalc) and FAS
                double faSigned = GetSignedFA();
                double fasSigned = GetSignedFAS();

                // First term: FA * 0.4, but now FA can be positive or negative
                double firstTerm = faSigned * 0.4;

                // Second term: same as before
                double secondTerm = (AInput - CInput) / 2.0;

                // Third term: same structure as before, but using signed values
                double thirdTerm = FACText != FASText
                    ? Math.Abs(faSigned - fasSigned) * 0.25
                    : (faSigned - fasSigned) * 0.25;

                return firstTerm + secondTerm + thirdTerm;
            }
        }

        public double RA => Math.Round(
            Math.Asin(Math.Sqrt(Math.Pow(HF, 2.0) + Math.Pow(HACCalc, 2.0)) / 20.0) * (180.0 / Math.PI),
            2);

        public string FACText => _fACText;

        public string FASText => _fASText;

        public string FAText => _fAText;

        public string HACText => _hACText;

        public string HAxText => _hAxText;

        public string ARText => CSTModeR ? "R" : "L";
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

        // Returns FA (FACalc) with its sign preserved, based on ILM/SLM and FACText
        private double GetSignedFA()
        {
            double sum = 0;

            // Same structure as FACalc, but WITHOUT Math.Abs and WITHOUT rounding
            if (ILMModeL)
                sum -= ILMInput;
            if (ILMModeR)
                sum += ILMInput;
            if (SLMModeR)
                sum += SLMInput;
            if (SLMModeL)
                sum -= SLMInput;

            sum /= 2.0;

            // Apply the FACText-based side convention (L makes it negative)
            if (!string.IsNullOrEmpty(FACText) && FACText == "L")
                sum *= -1.0;

            return sum;
        }

        // Returns FAS with its sign preserved (same sum logic as FASCalc, but no Math.Abs)
        private double GetSignedFAS()
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

            return sum;
        }

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
