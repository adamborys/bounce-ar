using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserGuideController : MonoBehaviour
{

    Button repeatButton, skipButton;
    SpriteRenderer first, second, third;
    Color transparent, fullVisible;
    // Use this for initialization
    void Start()
    {
        skipButton = GameObject.Find("SkipButton").GetComponent<Button>();
        skipButton.onClick.AddListener(delegate { SkipClick(); });
        repeatButton = GameObject.Find("RepeatButton").GetComponent<Button>();
        repeatButton.onClick.AddListener(delegate { RepeatClick(); });
        first = GameObject.Find("1").GetComponent<SpriteRenderer>();
        second = GameObject.Find("2").GetComponent<SpriteRenderer>();
        third = GameObject.Find("3").GetComponent<SpriteRenderer>();
        transparent = new Color(1, 1, 1, 0);
        fullVisible = new Color(1, 1, 1, 1);
		StartCoroutine(FirstStep());
    }
    private void RepeatClick()
    {
        StopAllCoroutines();
		first.color = second.color = third.color = transparent;
		StartCoroutine(FirstStep());
    }
    private void SkipClick()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private IEnumerator FirstStep()
    {
		float elapsedTime = 0.0f;
		float totalTime = 1.0f;
        while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			Debug.Log(elapsedTime);
            first.color = Color.Lerp(transparent, fullVisible, elapsedTime/totalTime);
        	yield return null;
		}
		yield return new WaitForSeconds(8);
        StartCoroutine(SecondStep());
    }
    private IEnumerator SecondStep()
    {
		float elapsedTime = 0.0f;
		float totalTime = 1.0f;
        while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
            first.color = Color.Lerp(fullVisible, transparent, elapsedTime/totalTime);
            second.color = Color.Lerp(transparent, fullVisible, elapsedTime/totalTime);
        	yield return null;
		}
		yield return new WaitForSeconds(8);
        StartCoroutine(ThirdStep());
    }
    private IEnumerator ThirdStep()
    {
		float elapsedTime = 0.0f;
		float totalTime = 1.0f;
        while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
            second.color = Color.Lerp(fullVisible, transparent, elapsedTime/totalTime);
            third.color = Color.Lerp(transparent, fullVisible, elapsedTime/totalTime);
        	yield return null;
		}
		yield return new WaitForSeconds(8);
    }
}
