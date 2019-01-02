using System;
using System.IO;

namespace DungeonMaster
{
    public sealed class DiscordToken
    {
        public string Value { get; }

        public DiscordToken(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator string(DiscordToken token)
        {
            return token.Value;
        }

        public static DiscordToken FromFile(string filePath)
        {
            string tokenText = File.ReadAllText(filePath);
            return new DiscordToken(tokenText);
        }
    }
}
