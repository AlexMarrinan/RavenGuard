using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapNodeProb", menuName = "OverworldMap/MapNodeProb", order = 1)]
public class MapNodeProbSO : ScriptableObject
{
    [Range(0.0f, 1.0f)]
    public List<float> typeProb;

    // Fix inconsistent probabilities, if they exist
    public void FixProbabilities() {
        float total = 0;
        foreach (float prob in typeProb) { // Get total probability
            total += prob;
        }
        for (int i = 0; i < typeProb.Count; i++) { // Fix probability issues
            typeProb[i] /= total;
        }
    }
}