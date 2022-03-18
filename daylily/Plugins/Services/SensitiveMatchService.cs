using System.IO;
using daylily.ThirdParty.ToolGood.Words;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Loading;

namespace daylily.Plugins.Services
{
    [PluginIdentifier("13432984-2A24-4691-9C0C-DE98B7B70F9A", "敏感词屏蔽")]
    public class SensitiveMatchService : ServicePlugin
    {
        private readonly ILogger<SensitiveMatchService> _logger;

        public SensitiveMatchService(ILogger<SensitiveMatchService> logger)
        {
            _logger = logger;
        }

        private StringSearchEx3? _sharedSearch;

        protected override async Task OnInitialized()
        {
            var bin = Path.Combine(PluginHome, "sensitive.bin");
            var text = Path.Combine(PluginHome, "sensitive.txt");
            if (File.Exists(bin))
            {
                _sharedSearch = new StringSearchEx3();
                _sharedSearch.Load(bin);
            }
            else if (File.Exists(text))
            {
                var lines = File.ReadAllLines(text);
                var stringSearchEx3 = new StringSearchEx3();
                stringSearchEx3.SetKeywords(lines);
                stringSearchEx3.Save(bin);
            }
            else
            {
                _logger.LogWarning("Sensitive file not found: " + new FileInfo(text).FullName);
            }
        }

        public override async Task<bool> BeforeSend(PluginInfo pluginInfo, IResponse response)
        {
            if (_sharedSearch == null) return true;
            if (response.Message == null) return true;

            if (response.Message is Text text)
            {
                ReplaceMessage(text, _sharedSearch);
            }
            else if (response.Message is RichMessage richMessage)
            {
                foreach (var message in richMessage)
                {
                    if (message is Text text1)
                    {
                        ReplaceMessage(text1, _sharedSearch);
                    }
                }
            }

            return true;
        }

        private static void ReplaceMessage(Text text, StringSearchEx3 stringSearchEx3)
        {
            var txt = stringSearchEx3.Replace(text.Content, '\0');
            text.Content = txt.Replace("\0", "_");
        }
    }
}
