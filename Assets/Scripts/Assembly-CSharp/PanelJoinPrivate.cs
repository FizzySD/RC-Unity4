using UnityEngine;

public class PanelJoinPrivate : MonoBehaviour
{
	public GameObject label_back;

	public GameObject label_ip;

	public GameObject label_join;

	public GameObject label_port;

	private int lang = -1;

	private void OnEnable()
	{
	}

	private void showTxt()
	{
		if (lang != Language.type)
		{
			lang = Language.type;
			label_ip.GetComponent<UILabel>().text = Language.server_ip[Language.type];
			label_port.GetComponent<UILabel>().text = Language.port[Language.type];
			label_join.GetComponent<UILabel>().text = Language.btn_join[Language.type];
			label_back.GetComponent<UILabel>().text = Language.btn_back[Language.type];
		}
	}

	private void Update()
	{
		showTxt();
	}
}
