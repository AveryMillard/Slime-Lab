using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public int health;
    public int numOfHearts;
    [SerializeField] public Image[] hearts;
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Sprite emptyHeart;
    private void Start()
    {

        health = PlayerManager.lives;
        numOfHearts = health;
    }

    public void TakeLives(int lives)
    {
        if (health-lives < 1)
            PlayerManager.GameOver();

        health -= lives;
    }

    private void Update()
    {

        if (health > numOfHearts) health = numOfHearts;

        for(int i=0; i<hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

}
