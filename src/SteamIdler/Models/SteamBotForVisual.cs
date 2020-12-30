using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Constants;

namespace SteamIdler.Models
{
    public class SteamBotForVisual : Bindable
    {
        private SteamBot _steamBot;
        private CodeType? _codeType;
        private string _code;
        private bool _isCodeRequired;
        private bool _hasError;
        private string _errorMessage;

        public SteamBot SteamBot
        {
            get => _steamBot;
            set => SetValue(ref _steamBot, value);
        }

        public CodeType? CodeType
        {
            get => _codeType;
            set
            {
                SetValue(ref _codeType, value);
                IsCodeRequired = value != null;
            }
        }

        public string Code
        {
            get => _code;
            set => SetValue(ref _code, value);
        }

        public bool IsCodeRequired
        {
            get => _isCodeRequired;
            set => SetValue(ref _isCodeRequired, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetValue(ref _hasError, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetValue(ref _errorMessage, value);
        }
    }
}
