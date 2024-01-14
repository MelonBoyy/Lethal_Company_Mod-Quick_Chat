using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace QuickChat.RadialMenu
{
	public class RadialMenu
	{
		public GameObject gameObject;
		public Transform transform;

		public List<RadialButton> radialButtons = new List<RadialButton>();
		public float radialOffset = UnitCircleOffset.BOTTOM;

		public string name = "Menu";

		public float outwards = 175f;

		private bool created = false;

		public void AddRadialButton(RadialButton button)
		{
			radialButtons.Add(button);
		}

		public void UpdateRadialButtons(List<RadialButton> radialButtons)
		{
			if (!created) return;

			this.radialButtons.ForEach(button => button.QuoteOnQuoteDestroyRadialButton());
			this.radialButtons.Clear();

			this.radialButtons = radialButtons;
			this.radialButtons.ForEach(button => button.SetupRadialButton(this));
			EvenlySplitOnUnitCircle();
		}

		public void UpdateRadialButtons()
		{
			if (!created) return;

			radialButtons.ForEach(button => button.SetupRadialButton(this));
			EvenlySplitOnUnitCircle();
		}

		public void RemoveRadialButton(int i)
		{
			radialButtons.RemoveAt(i);
		}

		internal void EvenlySplitOnUnitCircle()
		{
			for (int i = 0; i < radialButtons.Count; i++)
			{
				RadialButton radialButton = radialButtons[i];

				float point = GetPointOnUnitCircle(i);
				Vector2 position = (Vector2)RadialMenuHUD.RadialMenuHUDCamera.ScreenToWorldPoint(RadialMenuHUD.Center) + GetPositionOnUnitCircle(point) * outwards;

				radialButton.rectTransform.anchoredPosition = radialButton.positionOverride != Vector2.zero ? radialButton.positionOverride : position;
			}

			float GetPointOnUnitCircle(int i)
			{
				return (2 * Mathf.PI) * ((float)i / radialButtons.Count) + radialOffset;
			}

			Vector2 GetPositionOnUnitCircle(float x)
			{
				return new Vector2(Mathf.Cos(x), Mathf.Sin(x));
			}
		}

		internal void CreateRadialMenu(Transform parentTransform)
		{
			if (created) return;
			created = true;

			gameObject = new GameObject()
			{
				name = name
			};
			transform = gameObject.transform;
			transform.SetParent(parentTransform, false);

			radialButtons.ForEach(button => button.SetupRadialButton(this));
			EvenlySplitOnUnitCircle();

			gameObject.SetActive(false);
		}

		internal void RemoveRadialMenu()
		{
			created = false;
			radialButtons.ForEach(button => button.QuoteOnQuoteDestroyRadialButton());
		}

		public class RadialButton
		{
			public GameObject gameObject;
			public Transform transform;
			public RectTransform rectTransform;

			public Sprite sprite;
			public string text = string.Empty;
			public string displayText = string.Empty;

			public Vector2 positionOverride = Vector2.zero;
			public Vector2 sizeDelta = Vector2.one * 75;

			public float textSize = 12f;
			public Color textColor = Color.black;
			public TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;

			public RadialMenu parentRadialMenu;
			public Func<RadialMenu> connectingRadialMenu = null;

			public Action<RadialMenu, RadialButton> preRadialButtonClicked = null;
			public Action<RadialMenu, RadialButton> postRadialButtonClicked = null;

			private string expressText => displayText == string.Empty ? $"\"{text}\"" : displayText;

			public delegate void RadialButtonClicked(RadialMenu radialMenu, RadialButton radialButton);
			public static event RadialButtonClicked PreRadialButtonClicked;
			public static event RadialButtonClicked PostRadialButtonClicked;

			private bool created = false;

			public void QuoteOnQuoteDestroyRadialButton()
			{
				if (!created) return;

				created = false;
			}

			internal void CreateTextField()
			{
				GameObject gameObject = new GameObject()
				{
					name = expressText
				};
				Transform textFieldTransform = gameObject.transform;
				textFieldTransform.SetParent(transform, false);

				TextMeshProUGUI textField = gameObject.AddComponent<TextMeshProUGUI>();
				textField.text = displayText == string.Empty ? $"\"{text}\"" : displayText;
				textField.fontSize = textSize;
				textField.color = textColor;
				textField.alignment = textAlignment;
				textField.rectTransform.sizeDelta = sizeDelta;

				textField.enableWordWrapping = true;
			}

			internal void SetupRadialButton(RadialMenu parentRadialMenu)
			{
				if (created) return;
				created = true;

				gameObject = new GameObject()
				{
					name = expressText
				};
				transform = gameObject.transform;
				transform.SetParent(parentRadialMenu.transform, false);

				this.parentRadialMenu = parentRadialMenu;

				CreateTextField();

				Image radialButtonImage = gameObject.AddComponent<Image>();
				radialButtonImage.sprite = sprite;

				Button radialButton = gameObject.AddComponent<Button>();
				radialButton.onClick.AddListener(OnClick);

				rectTransform = radialButton.transform as RectTransform;
				rectTransform.sizeDelta = sizeDelta;
			}

			internal void OnClick()
			{
				preRadialButtonClicked?.Invoke(parentRadialMenu, this);
				PreRadialButtonClicked?.Invoke(parentRadialMenu, this);

				RadialMenu connectingRadialMenu = this.connectingRadialMenu?.Invoke();

				if (text != string.Empty) RadialMenuManager.AddChatText(text);

				if (connectingRadialMenu == null || connectingRadialMenu.radialButtons.Count <= 0) RadialMenuManager.SendChatText();
				else RadialMenuManager.SetCurrentMenu(connectingRadialMenu);

				RadialMenuManager.RefreshMenu();

				postRadialButtonClicked?.Invoke(parentRadialMenu, this);
				PostRadialButtonClicked?.Invoke(parentRadialMenu, this);
			}

		}

		public class UnitCircleOffset
		{
			public const float RIGHT = 0;
			public const float TOP_RIGHT = Mathf.PI / 4;
			public const float TOP = Mathf.PI / 2;
			public const float TOP_LEFT = 3 * Mathf.PI / 4;
			public const float LEFT = Mathf.PI;
			public const float BOTTOM_LEFT = 5 * Mathf.PI / 4;
			public const float BOTTOM = 3 * Mathf.PI / 2;
			public const float BOTTOM_RIGHT = 7 * Mathf.PI / 4;
		}
	}
}
