using UnityEngine;
using TMPro;


// Generated some of this code with AI
public class Tip : MonoBehaviour
{
    public float fadeDuration = 6f; // Adjust fade duration as needed
    public float delayBeforeFade = 0f; // Delay before fading starts

    public TMP_Text textComponent;
    private float fadeTime;
    bool activated = false;

    void Start()
    {
        var textComponents = GetComponents<TMP_Text>();
        foreach(TMP_Text text in textComponents)
        {
            if (text.name == "Tip")
            {
                textComponent = text;
            }
        }

        fadeTime = delayBeforeFade;
        textComponent.enabled = false;
        setText("Welcome to the LAB, little one!");
    }

    public void setText(string S)
    {
        textComponent.enabled = true;
        textComponent.text = string.Format(S);
        fadeDuration = 6f;
        fadeTime = 0f;
        activated = true;
    }

    

    void FixedUpdate()
    {
        if (activated==true &&  fadeTime!=fadeDuration)
        {
            fadeTime += Time.deltaTime;
            float fadeProgress = (fadeTime / fadeDuration);
            //Debug.Log("Fade progress:" + fadeProgress);
            // Clamp fade progress to prevent negative values
            fadeProgress = Mathf.Clamp(fadeProgress, 0f, 1f);

            // Calculate the alpha value for the text color
            Color textColor = textComponent.color;
            textColor.a = 1f - fadeProgress;
            textComponent.color = textColor;

            // Destroy the object when fully faded
            if (fadeDuration<=fadeTime)
            {
                activated = false;
                textComponent.enabled=false;
            }
        }
    }
}