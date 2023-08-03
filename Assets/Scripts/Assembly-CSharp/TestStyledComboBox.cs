using UnityEngine;

public class TestStyledComboBox : MonoBehaviour
{
	public StyledComboBox comboBox;

	private void Start()
	{
		object[] list = new object[11]
		{
			"English", "简体中文", "繁體中文", "繁體中文", "繁體中文", "繁體中文", "繁體中文", "繁體中文", "繁體中文", "繁體中文",
			"繁體中文"
		};
		comboBox.AddItems(list);
	}
}
