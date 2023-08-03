using UnityEngine;

public class Language
{
	public static string[] abnormal = new string[25];

	public static string[] btn_back = new string[25];

	public static string[] btn_continue = new string[25];

	public static string[] btn_create_game = new string[25];

	public static string[] btn_credits = new string[25];

	public static string[] btn_default = new string[25];

	public static string[] btn_join = new string[25];

	public static string[] btn_LAN = new string[25];

	public static string[] btn_multiplayer = new string[25];

	public static string[] btn_option = new string[25];

	public static string[] btn_QUICK_MATCH = new string[25];

	public static string[] btn_quit = new string[25];

	public static string[] btn_ready = new string[25];

	public static string[] btn_refresh = new string[25];

	public static string[] btn_server_ASIA = new string[25];

	public static string[] btn_server_EU = new string[25];

	public static string[] btn_server_JAPAN = new string[25];

	public static string[] btn_server_US = new string[25];

	public static string[] btn_single = new string[25];

	public static string[] btn_start = new string[25];

	public static string[] camera_info = new string[25];

	public static string[] camera_original = new string[25];

	public static string[] camera_tilt = new string[25];

	public static string[] camera_tps = new string[25];

	public static string[] camera_type = new string[25];

	public static string[] camera_wow = new string[25];

	public static string[] change_quality = new string[25];

	public static string[] choose_character = new string[25];

	public static string[] choose_map = new string[25];

	public static string[] choose_region_server = new string[25];

	public static string[] difficulty = new string[25];

	public static string[] game_time = new string[25];

	public static string[] hard = new string[25];

	public static string[] invert_mouse = new string[25];

	public static string[] key_set_info_1 = new string[25];

	public static string[] key_set_info_2 = new string[25];

	public static string[] max_player = new string[25];

	public static string[] max_Time = new string[25];

	public static string[] mouse_sensitivity = new string[25];

	public static string[] normal = new string[25];

	public static string[] port = new string[25];

	public static string[] select_titan = new string[25];

	public static string[] server_ip = new string[25];

	public static string[] server_name = new string[25];

	public static string[] soldier = new string[25];

	public static string[] titan = new string[25];

	public static int type = -1;

	public static string[] waiting_for_input = new string[25];

	public static string GetLang(int id)
	{
		switch (id)
		{
		case 1:
			return "简体中文";
		case 2:
			return "SPANISH";
		case 3:
			return "POLSKI";
		case 4:
			return "ITALIANO";
		case 5:
			return "NORWEGIAN";
		case 6:
			return "PORTUGUESE";
		case 7:
			return "PORTUGUESE_BR";
		case 8:
			return "繁體中文_台";
		case 9:
			return "繁體中文_港";
		case 10:
			return "SLOVAK";
		case 11:
			return "GERMAN";
		case 12:
			return "FRANCAIS";
		case 13:
			return "TÜRKÇE";
		case 14:
			return "ARABIC";
		case 15:
			return "Thai";
		case 16:
			return "Русский";
		case 17:
			return "NEDERLANDS";
		case 18:
			return "Hebrew";
		case 19:
			return "DANSK";
		default:
			return "ENGLISH";
		}
	}

	public static int GetLangIndex(string txt)
	{
		if (txt != "ENGLISH")
		{
			switch (txt)
			{
			case "SPANISH":
				return 2;
			case "POLSKI":
				return 3;
			case "ITALIANO":
				return 4;
			case "NORWEGIAN":
				return 5;
			case "PORTUGUESE":
				return 6;
			case "PORTUGUESE_BR":
				return 7;
			case "SLOVAK":
				return 10;
			case "GERMAN":
				return 11;
			case "FRANCAIS":
				return 12;
			case "TÜRKÇE":
				return 13;
			case "ARABIC":
				return 14;
			case "Thai":
				return 15;
			case "Русский":
				return 16;
			case "NEDERLANDS":
				return 17;
			case "Hebrew":
				return 18;
			case "DANSK":
				return 19;
			case "简体中文":
				return 1;
			case "繁體中文_台":
				return 8;
			case "繁體中文_港":
				return 9;
			}
		}
		return 0;
	}

	public static void init()
	{
		char[] separator = new char[1] { "\n"[0] };
		string[] array = ((TextAsset)Resources.Load("lang")).text.Split(separator);
		string text = string.Empty;
		int num = 0;
		string empty = string.Empty;
		string empty2 = string.Empty;
		foreach (string text2 in array)
		{
			if (text2.Contains("//"))
			{
				continue;
			}
			if (text2.Contains("#START"))
			{
				char[] separator2 = new char[1] { "@"[0] };
				text = text2.Split(separator2)[1];
				num = GetLangIndex(text);
			}
			else if (text2.Contains("#END"))
			{
				text = string.Empty;
			}
			else if (text != string.Empty && text2.Contains("@"))
			{
				char[] separator3 = new char[1] { "@"[0] };
				empty = text2.Split(separator3)[0];
				char[] separator4 = new char[1] { "@"[0] };
				empty2 = text2.Split(separator4)[1];
				switch (empty)
				{
				case "btn_single":
					btn_single[num] = empty2;
					break;
				case "btn_multiplayer":
					btn_multiplayer[num] = empty2;
					break;
				case "btn_option":
					btn_option[num] = empty2;
					break;
				case "btn_credits":
					btn_credits[num] = empty2;
					break;
				case "btn_back":
					btn_back[num] = empty2;
					break;
				case "btn_refresh":
					btn_refresh[num] = empty2;
					break;
				case "btn_join":
					btn_join[num] = empty2;
					break;
				case "btn_start":
					btn_start[num] = empty2;
					break;
				case "btn_create_game":
					btn_create_game[num] = empty2;
					break;
				case "btn_LAN":
					btn_LAN[num] = empty2;
					break;
				case "btn_server_US":
					btn_server_US[num] = empty2;
					break;
				case "btn_server_EU":
					btn_server_EU[num] = empty2;
					break;
				case "btn_server_ASIA":
					btn_server_ASIA[num] = empty2;
					break;
				case "btn_server_JAPAN":
					btn_server_JAPAN[num] = empty2;
					break;
				case "btn_QUICK_MATCH":
					btn_QUICK_MATCH[num] = empty2;
					break;
				case "btn_default":
					btn_default[num] = empty2;
					break;
				case "btn_ready":
					btn_ready[num] = empty2;
					break;
				case "server_name":
					server_name[num] = empty2;
					break;
				case "server_ip":
					server_ip[num] = empty2;
					break;
				case "port":
					port[num] = empty2;
					break;
				case "choose_map":
					choose_map[num] = empty2;
					break;
				case "choose_character":
					choose_character[num] = empty2;
					break;
				case "camera_type":
					camera_type[num] = empty2;
					break;
				case "camera_original":
					camera_original[num] = empty2;
					break;
				case "camera_wow":
					camera_wow[num] = empty2;
					break;
				case "camera_tps":
					camera_tps[num] = empty2;
					break;
				case "max_player":
					max_player[num] = empty2;
					break;
				case "max_Time":
					max_Time[num] = empty2;
					break;
				case "game_time":
					game_time[num] = empty2;
					break;
				case "difficulty":
					difficulty[num] = empty2;
					break;
				case "normal":
					normal[num] = empty2;
					break;
				case "hard":
					hard[num] = empty2;
					break;
				case "abnormal":
					abnormal[num] = empty2;
					break;
				case "mouse_sensitivity":
					mouse_sensitivity[num] = empty2;
					break;
				case "change_quality":
					change_quality[num] = empty2;
					break;
				case "camera_tilt":
					camera_tilt[num] = empty2;
					break;
				case "invert_mouse":
					invert_mouse[num] = empty2;
					break;
				case "waiting_for_input":
					waiting_for_input[num] = empty2;
					break;
				case "key_set_info_1":
					key_set_info_1[num] = empty2;
					break;
				case "key_set_info_2":
					key_set_info_2[num] = empty2;
					break;
				case "soldier":
					soldier[num] = empty2;
					break;
				case "titan":
					titan[num] = empty2;
					break;
				case "select_titan":
					select_titan[num] = empty2;
					break;
				case "camera_info":
					camera_info[num] = empty2;
					break;
				case "btn_continue":
					btn_continue[num] = empty2;
					break;
				case "btn_quit":
					btn_quit[num] = empty2;
					break;
				case "choose_region_server":
					choose_region_server[num] = empty2;
					break;
				}
			}
		}
	}
}
