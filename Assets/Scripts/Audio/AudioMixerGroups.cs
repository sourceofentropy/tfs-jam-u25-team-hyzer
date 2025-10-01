using System.Collections.Generic;

public static class AudioMixerGroups
{
    public const string Master = "Master";
    public const string Soundtrack = "Soundtrack";
    public const string SoundFX = "SoundFX";

    public static IEnumerable<string> AllGroups => new[] { Master, Soundtrack, SoundFX };
}
