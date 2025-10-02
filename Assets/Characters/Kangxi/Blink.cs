using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker_Player : MonoBehaviour
{

    [Header("Eyes Blinking Period")]
    [SerializeField] private int blink_seconds;

    [Header("Reference to the Chracter Animator")]
    [SerializeField] private Animator anim;

    private bool corrutineStarted;
    private bool isBlinking;

    // Update is called once per frame
    void Update()
    {
        if (!isBlinking)
        {
            if (!corrutineStarted)
            {
                StartCoroutine(BlinkSeconds(blink_seconds));
            }
        }
        else
        {
            StartCoroutine(MakeAnimation());
        }
    }

    IEnumerator BlinkSeconds(int blink_seconds)
    {
        corrutineStarted = true;
        yield return new WaitForSeconds(blink_seconds);

        isBlinking = true;
    }

    IEnumerator MakeAnimation()
    {
        anim.SetBool("Blink", true);

        yield return new WaitForSeconds(0.015f);

        isBlinking = false;
        corrutineStarted = false;

        anim.SetBool("Blink", false);
    }
}
