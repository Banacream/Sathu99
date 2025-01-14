using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenesControllerButton : MonoBehaviour
{
	enum TargetScene
	{
		Next,
		Previous,
		MainMenu
	}

	[SerializeField] TargetScene targetScene;
	Button button;

	void Start()
	{


		button = GetComponent<Button>();

		button.onClick.RemoveAllListeners();
		switch (targetScene)
		{
			case TargetScene.MainMenu:
				button.onClick.AddListener(() => ScenesController.LoadMainScene());
				break;

			case TargetScene.Next:
				button.onClick.AddListener(() => ScenesController.LoadNextScene());
				break;

			case TargetScene.Previous:
				button.onClick.AddListener(() => ScenesController.LoadPreviousScene());
				break;
		}

	}
}
