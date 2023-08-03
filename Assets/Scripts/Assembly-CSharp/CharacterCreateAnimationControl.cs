using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterCreateAnimationControl : MonoBehaviour
{
	[CompilerGenerated]
	private static Dictionary<string, int> fswitchSmap0;

	private string currentAnimation;

	private float interval = 10f;

	private HERO_SETUP setup;

	private float timeElapsed;

	private void play(string id)
	{
		currentAnimation = id;
		base.animation.Play(id);
	}

	public void playAttack(string id)
	{
		switch (id)
		{
		case "mikasa":
			currentAnimation = "attack3_1";
			break;
		case "levi":
			currentAnimation = "attack5";
			break;
		case "sasha":
			currentAnimation = "special_sasha";
			break;
		case "jean":
			currentAnimation = "grabbed_jean";
			break;
		case "marco":
			currentAnimation = "special_marco_0";
			break;
		case "armin":
			currentAnimation = "special_armin";
			break;
		case "petra":
			currentAnimation = "special_petra";
			break;
		}
		base.animation.Play(currentAnimation);
	}

	private void Start()
	{
		setup = base.gameObject.GetComponent<HERO_SETUP>();
		currentAnimation = "stand_levi";
		play(currentAnimation);
	}

	public void toStand()
	{
		if (setup.myCostume.sex == SEX.FEMALE)
		{
			currentAnimation = "stand";
		}
		else
		{
			currentAnimation = "stand_levi";
		}
		base.animation.CrossFade(currentAnimation, 0.1f);
		timeElapsed = 0f;
	}

	private void Update()
	{
		if (currentAnimation == "stand" || currentAnimation == "stand_levi")
		{
			timeElapsed += Time.deltaTime;
			if (timeElapsed > interval)
			{
				timeElapsed = 0f;
				if (Random.Range(1, 1000) < 350)
				{
					play("salute");
				}
				else if (Random.Range(1, 1000) < 350)
				{
					play("supply");
				}
				else
				{
					play("dodge");
				}
			}
		}
		else if (base.animation[currentAnimation].normalizedTime >= 1f)
		{
			if (currentAnimation == "attack3_1")
			{
				play("attack3_2");
			}
			else if (currentAnimation == "special_sasha")
			{
				play("run_sasha");
			}
			else
			{
				toStand();
			}
		}
	}
}
