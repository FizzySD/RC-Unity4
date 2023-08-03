using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	internal class MainMenu : BaseMenu, ISelectHandler, IEventSystemHandler
	{
		public List<GameObject> _popupsGO = new List<GameObject>();

		public GameObject SinglePlayerPopup;

		private AudioSource MenuMusic;

		private AudioSource SoundManagerSource;

		private GUIStyle transparentButtonStyle;

		public BasePopup _singleplayerPopup;

		public BasePopup _multiplayerMapPopup;

		public BasePopup _settingsPopup;

		public BasePopup _toolsPopup;

		public BasePopup _multiplayerRoomListPopup;

		public BasePopup _editProfilePopup;

		public BasePopup _questsPopup;

		public GameObject canvasPrefab;

		private bool showGUI = true;

		private Vector2 scrollPosition = Vector2.zero;

		public Texture2D CursorTexture;

		public CursorMode cursorMode;

		protected Text _multiplayerStatusLabel;

		public override void Setup()
		{
			ObjectTracker objectTracker = new ObjectTracker();
			base.Setup();
			GameObject gameObject = GameObject.Find("Camera");
			Object.Destroy(gameObject.gameObject);
			Debug.Log("jwdf");
			CursorTexture = (Texture2D)FengGameManagerMKII.RRMassets.Load("Cursor1");
			Cursor.SetCursor(CursorTexture, new Vector2(CursorTexture.width / 3, 0f), cursorMode);
			GameObject original = FengGameManagerMKII.RRMassets.Load("TestMenu 2") as GameObject;
			GameObject gameObject2 = (GameObject)Object.Instantiate(original);
			GameObject original2 = FengGameManagerMKII.RRMassets.Load("Pino") as GameObject;
			SinglePlayerPopup = (GameObject)Object.Instantiate(original2);
			SinglePlayerPopup.AddComponent<BasePopup>();
			SinglePlayerPopup.SetActive(false);
			_popupsGO.Add(SinglePlayerPopup);
			SoundManagerSource = GameObject.Find("SoundHandler").GetComponent<AudioSource>();
			MenuMusic = GameObject.Find("Main Camera").GetComponent<AudioSource>();
			MenuMusic.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("a");
			MenuMusic.volume = 0.04f;
			MenuMusic.loop = true;
			MenuMusic.Play();
			SetupIntroPanel();
			SetupLabels();
		}

		public void ShowMultiplayerRoomListPopup()
		{
			HideAllPopups();
			_multiplayerRoomListPopup.Show();
		}

		public void ShowMultiplayerMapPopup()
		{
			HideAllPopups();
			_multiplayerMapPopup.Show();
		}

		protected override void SetupPopups()
		{
			base.SetupPopups();
			_singleplayerPopup = ElementFactory.CreateHeadedPanel<SingleplayerPopup>(base.transform).GetComponent<BasePopup>();
			_multiplayerMapPopup = ElementFactory.InstantiateAndSetupPanel<MultiplayerMapPopup>(base.transform, "MultiplayerMapPopup").GetComponent<BasePopup>();
			_editProfilePopup = ElementFactory.CreateHeadedPanel<EditProfilePopup>(base.transform).GetComponent<BasePopup>();
			_settingsPopup = ElementFactory.CreateHeadedPanel<SettingsPopup>(base.transform).GetComponent<BasePopup>();
			_toolsPopup = ElementFactory.CreateHeadedPanel<ToolsPopup>(base.transform).GetComponent<BasePopup>();
			_multiplayerRoomListPopup = ElementFactory.InstantiateAndSetupPanel<MultiplayerRoomListPopup>(base.transform, "MultiplayerRoomListPopup").GetComponent<BasePopup>();
			_questsPopup = ElementFactory.CreateHeadedPanel<QuestPopup>(base.transform).GetComponent<BasePopup>();
			_popups.Add(_singleplayerPopup);
			_popups.Add(_multiplayerMapPopup);
			_popups.Add(_editProfilePopup);
			_popups.Add(_settingsPopup);
			_popups.Add(_toolsPopup);
			_popups.Add(_multiplayerRoomListPopup);
			_popups.Add(_questsPopup);
		}

		private void SetupIntroPanel()
		{
			GameObject gameObject = ElementFactory.InstantiateAndBind2(base.transform, "IntroPanel");
			GameObject gameObject2 = GameObject.Find("SingleplayerButton");
			Button SinglePlayerbtn = gameObject2.GetComponent<Button>();
			gameObject2.AddComponent<ButtonSelectHandler>();
			gameObject2.GetComponent<ButtonSelectHandler>().audioSource = SoundManagerSource;
			gameObject2.GetComponent<ButtonSelectHandler>().mouseOverSound = (AudioClip)FengGameManagerMKII.RRMassets.Load("Click");
			gameObject2.name = "SingleplayerButton";
			SinglePlayerbtn.name = "SingleplayerButton";
			SinglePlayerbtn.onClick.AddListener(delegate
			{
				OnIntroButtonClick(SinglePlayerbtn.name);
			});
			GameObject gameObject3 = GameObject.Find("MultiplayerButton");
			Button Multiplayerbtn = gameObject3.GetComponent<Button>();
			gameObject3.AddComponent<ButtonSelectHandler>();
			gameObject3.GetComponent<ButtonSelectHandler>().audioSource = SoundManagerSource;
			gameObject3.GetComponent<ButtonSelectHandler>().mouseOverSound = (AudioClip)FengGameManagerMKII.RRMassets.Load("Click");
			gameObject3.name = "MultiplayerButton";
			Multiplayerbtn.name = "MultiplayerButton";
			Multiplayerbtn.onClick.AddListener(delegate
			{
				OnIntroButtonClick(Multiplayerbtn.name);
			});
			GameObject gameObject4 = GameObject.Find("ProfileButton");
			Button Profilebtn = gameObject4.GetComponent<Button>();
			gameObject4.AddComponent<ButtonSelectHandler>();
			gameObject4.GetComponent<ButtonSelectHandler>().audioSource = SoundManagerSource;
			gameObject4.GetComponent<ButtonSelectHandler>().mouseOverSound = (AudioClip)FengGameManagerMKII.RRMassets.Load("Click");
			gameObject4.name = "ProfileButton";
			Profilebtn.name = "ProfileButton";
			Profilebtn.onClick.AddListener(delegate
			{
				OnIntroButtonClick(Profilebtn.name);
			});
			GameObject gameObject5 = GameObject.Find("LeaderboardButton");
			Button LeaderboardBtn = gameObject5.GetComponent<Button>();
			gameObject5.AddComponent<ButtonSelectHandler>();
			gameObject5.GetComponent<ButtonSelectHandler>().audioSource = SoundManagerSource;
			gameObject5.GetComponent<ButtonSelectHandler>().mouseOverSound = (AudioClip)FengGameManagerMKII.RRMassets.Load("Click");
			gameObject5.name = "LeaderboardButton";
			LeaderboardBtn.name = "LeaderboardButton";
			LeaderboardBtn.onClick.AddListener(delegate
			{
				OnIntroButtonClick(LeaderboardBtn.name);
			});
			GameObject gameObject6 = GameObject.Find("QuitButton");
			Button Quitbtn = gameObject6.GetComponent<Button>();
			gameObject6.AddComponent<ButtonSelectHandler>();
			gameObject6.GetComponent<ButtonSelectHandler>().audioSource = SoundManagerSource;
			gameObject6.GetComponent<ButtonSelectHandler>().mouseOverSound = (AudioClip)FengGameManagerMKII.RRMassets.Load("Click");
			gameObject6.name = "QuitButton";
			Quitbtn.name = "QuitButton";
			Quitbtn.onClick.AddListener(delegate
			{
				OnIntroButtonClick(Quitbtn.name);
			});
			gameObject.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			GameObject gameObject7 = GameObject.Find("MenuBackGround");
			Object.Destroy(gameObject7.gameObject);
		}

		private void SetupLabels()
		{
			GameObject gameObject = ElementFactory.InstantiateAndBind(base.transform, "Aottg2DonateButton");
			ElementFactory.SetAnchor(gameObject, TextAnchor.UpperRight, TextAnchor.UpperRight, new Vector2(-20f, -20f));
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnIntroButtonClick("Donate");
			});
			_multiplayerStatusLabel = ElementFactory.CreateDefaultLabel(base.transform, ElementStyle.Default, string.Empty).GetComponent<Text>();
			ElementFactory.SetAnchor(_multiplayerStatusLabel.gameObject, TextAnchor.UpperLeft, TextAnchor.UpperLeft, new Vector2(20f, -20f));
			_multiplayerStatusLabel.color = Color.white;
		}

		private void OnGUI()
		{
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F5))
			{
				showGUI = !showGUI;
			}
			if (_multiplayerStatusLabel != null)
			{
				_multiplayerStatusLabel.text = PhotonNetwork.connectionStateDetailed.ToString();
				if (PhotonNetwork.connected)
				{
					Text multiplayerStatusLabel = _multiplayerStatusLabel;
					multiplayerStatusLabel.text = multiplayerStatusLabel.text + " ping:" + PhotonNetwork.GetPing();
				}
			}
		}

		private IEnumerator CloseMenu()
		{
			yield return new WaitForSeconds(0.3f);
			SinglePlayerPopup.SetActive(false);
		}

		private void OnIntroButtonClick(string name)
		{
			HideAllPopups();
			switch (name)
			{
			case "SingleplayerButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				if (!SinglePlayerPopup.active)
				{
					SinglePlayerPopup.SetActive(true);
					SinglePlayerPopup.GetComponent<MeshRenderer>().enabled = true;
					SinglePlayerPopup.GetComponent<Animator>().SetBool("chiudilegambe", false);
					StopAllCoroutines();
				}
				else
				{
					SinglePlayerPopup.GetComponent<Animator>().SetBool("chiudilegambe", true);
					StartCoroutine(CloseMenu());
				}
				break;
			case "MultiplayerButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				_multiplayerMapPopup.Show();
				break;
			case "ProfileButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				_editProfilePopup.Show();
				break;
			case "QuestsButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				_questsPopup.Show();
				break;
			case "SettingsButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				_settingsPopup.Show();
				break;
			case "ToolsButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				_toolsPopup.Show();
				break;
			case "QuitButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				Application.Quit();
				break;
			case "Donate":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				Application.OpenURL("https://www.patreon.com/aottg2");
				break;
			case "LeaderboardButton":
				SoundManagerSource.clip = (AudioClip)FengGameManagerMKII.RRMassets.Load("LeftClick");
				SoundManagerSource.Play();
				SinglePlayerPopup.SetActive(false);
				break;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			MenuMusic.Play();
		}
	}
}
