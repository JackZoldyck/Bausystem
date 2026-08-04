using UnityEngine;

public readonly struct VegetationSample
{
    public readonly float Height;
    public readonly float Slope;

    public readonly float Moisture;
    public readonly float Temperature;
    public readonly float Fertility;

    public readonly float ForestPotential;
    public readonly float OpenLandPotential;

    public VegetationSample(
        float height,
        float slope,
        float moisture,
        float temperature,
        float fertility,
        float forestPotential,
        float openLandPotential)
    {
        Height = Mathf.Clamp01(height);
        Slope = Mathf.Clamp01(slope);

        Moisture = Mathf.Clamp01(moisture);
        Temperature = Mathf.Clamp01(temperature);
        Fertility = Mathf.Clamp01(fertility);

        ForestPotential = Mathf.Clamp01(forestPotential);
        OpenLandPotential = Mathf.Clamp01(openLandPotential);
    }
}