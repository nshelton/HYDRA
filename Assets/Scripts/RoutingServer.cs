using Lasp;

public static class RoutingServer
{
    public static BeatDetector m_beat;
    public static AudioLevelTracker m_bass;
    public static AudioLevelTracker m_treble;
    public static AudioLevelTracker m_bypass;

    public static float SampleOscillator(OscillatorType type, int beats)
    {
        return m_beat.GetBeat(beats);
    }

    public static float SampleBass() { return m_bass.normalizedLevel; }
    public static float SampleBypass() { return m_bypass.normalizedLevel; }
    public static float SampleTreble() { return m_treble.normalizedLevel; }
}
