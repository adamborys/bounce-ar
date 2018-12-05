using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserGuideController : MonoBehaviour
{
	public static bool IsActive;
    Button prevButton, nextButton;
	Text prevButtonLabel, nextButtonLabel;
    GameObject first, second, third, fourth;
	int iterator;

    void Start()
    {
        prevButton = GameObject.Find("PrevButton").GetComponent<Button>();
        prevButton.onClick.AddListener(delegate { PrevClick(); });
        nextButton = GameObject.Find("NextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(delegate { NextClick(); });
		prevButtonLabel = prevButton.GetComponentInChildren<Text>();
		nextButtonLabel = nextButton.GetComponentInChildren<Text>();
        first = GameObject.Find("1");
        (second = GameObject.Find("2")).SetActive(false);
        (third = GameObject.Find("3")).SetActive(false);
        (fourth = GameObject.Find("4")).SetActive(false);
		IsActive = true;
		iterator = 1;
    }
    private void PrevClick()
    {
		switch(iterator)
		{
			case 1:
				gameObject.SetActive(false);
				UIController.calibrateButton.interactable = true;
				UIController.buildButton.interactable = true;
				IsActive = false;
				break;
			case 2:
				first.SetActive(true);
				second.SetActive(false);
				prevButtonLabel.text = "Skip";
				iterator--;
				break;
			case 3:
				second.SetActive(true);
				third.SetActive(false);
				iterator--;
				break;
			case 4:
				third.SetActive(true);
				fourth.SetActive(false);
				nextButtonLabel.text = "Next";
				iterator--;
				break;
		}
    }
    private void NextClick()
    {
		switch(iterator)
		{
			case 1:
				
				first.SetActive(false);
				second.SetActive(true);
				prevButtonLabel.text = "Prev";
				iterator++;
				break;
			case 2:
				second.SetActive(false);
				third.SetActive(true);
				iterator++;
				break;
			case 3:
				third.SetActive(false);
				fourth.SetActive(true);
				nextButtonLabel.text = "Close";
				iterator++;
				break;
			case 4:
				gameObject.SetActive(false);
				UIController.calibrateButton.interactable = true;
				UIController.buildButton.interactable = true;
				IsActive = false;
				break;
		}
    }
}
