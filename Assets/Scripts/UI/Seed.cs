using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Seed : MonoBehaviour
{
    TMP_Text seedText;
    [Range(0,1)] public float fadeAlpha;
    // Start is called before the first frame update
    void Start()
    {
        var textComponents = GetComponents<TMP_Text>();
        foreach (TMP_Text text in textComponents)
        {
            if (text.name == "Seed")
            {
                seedText = text;
            }
        }

        int seed = LevelGenManager.Instance.GetSeed();

        Color color = seedText.color;
        color.a = fadeAlpha;

        seedText.color = color;
        seedText.text = "Seed: " + seed;
    }
}
