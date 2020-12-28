using SteamIdler.Infrastructure.Constants;

namespace SteamIdler.Infrastructure
{
    public class SteamBotWithCode : Bindable
    {
        private SteamBot _steamBot;
        private CodeType? _codeType;
        private string _code;
        private bool _isCodeRequired;

        public SteamBot SteamBot
        {
            get => _steamBot;
            set => SetValue(ref _steamBot, value);
        }

        public CodeType? CodeType
        {
            get => _codeType;
            set => SetValue(ref _codeType, value);
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
    }
}
