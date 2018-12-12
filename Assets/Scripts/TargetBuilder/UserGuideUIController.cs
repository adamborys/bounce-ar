using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserGuideUIController : MonoBehaviour
{
	public static bool IsActive;
    public Button prevButton, nextButton;
	public Text prevButtonLabel, nextButtonLabel;
    public GameObject first, second, third, fourth;
	int iterator;

    void Start()
    {
        prevButton.onClick.AddListener(delegate { PrevClick(); });
        nextButton.onClick.AddListener(delegate { NextClick(); });
        second.SetActive(false);
        third.SetActive(false);
        fourth.SetActive(false);
		IsActive = true;
		iterator = 1;
    }
    private void PrevClick()
    {
		switch(iterator)
		{
			case 1:
				gameObject.SetActive(false);
				TargetBuilderUIController.calibrateButton.interactable = true;
				TargetBuilderUIController.buildButton.interactable = true;
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
				TargetBuilderUIController.calibrateButton.interactable = true;
				TargetBuilderUIController.buildButton.interactable = true;
				IsActive = false;
				break;
		}
    }
}
