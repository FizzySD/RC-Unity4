using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectHandler : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public AudioSource audioSource;

	public AudioClip mouseOverSound;

	private Button button;

	private void Start()
	{
		button = GetComponent<Button>();
		if (button == null)
		{
			Debug.LogWarning("Button component not found on the object: " + base.gameObject.name);
		}
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (audioSource != null && mouseOverSound != null)
		{
			audioSource.PlayOneShot(mouseOverSound);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
