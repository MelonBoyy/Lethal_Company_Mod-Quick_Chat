using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace QuickChat.RadialMenu
{
	public class RadialMenu
	{
		public GameObject gameObject { get; private set; } = null;
		public Transform transform { get; private set; } = null;

		private List<RadialButton> m_radialButtons = new List<RadialButton>();
		public List<RadialButton> radialButtons
		{
			get
			{
				return m_radialButtons;
			}
			set
			{
				m_radialButtons = value;
				UpdateRadialButtons(m_radialButtons);
			}
		}

		public string name = "Menu";
		public bool saveToHistory = true;

		private float m_radialOffset = UnitCircleOffset.BOTTOM;
		public float radialOffset
		{
			get { return m_radialOffset; }
			set
			{
				m_radialOffset = value;
				EvenlySplitOnUnitCircle();
			}
		}

		private float m_outwards = 175f;
		public float outwards
		{
			get { return m_outwards; }
			set
			{
				m_outwards = value;
				EvenlySplitOnUnitCircle();
			}
		}

		public bool created { get; private set; } = false;


		/// <summary>
		/// Adds a RadialButton at the end of the "radialButtons" list.
		/// </summary>
		/// <param name="button">The RadialButton to add.</param>
		public void AddRadialButton(RadialButton button)
		{
			radialButtons.Add(button);
		}

		/// <summary>
		/// Inserts a RadialButton at the specified index of the "radialButtons" list.
		/// </summary>
		/// <param name="button">The RadialButton to insert.</param>
		/// /// <param name="i">The index to insert the RadialButton at.</param>
		public void InsertRadialButton(RadialButton button, int i)
		{
			radialButtons.Insert(i, button);
		}

		/// <summary>
		/// Updates the "radialButtons" list, replacing all of them with a new list of RadialButtons, setting them up, and splitting them in a circular pattern.
		/// </summary>
		/// <param name="radialButtons">The new list of radialButtons to replace the old ones.</param>
		public void UpdateRadialButtons(List<RadialButton> radialButtons)
		{
			if (created) m_radialButtons.ForEach(button => button.QuoteOnQuoteDestroyRadialButton());
			m_radialButtons.Clear();

			m_radialButtons = radialButtons;
			UpdateRadialButtons();
		}

		/// <summary>
		/// Updates the "radialButtons" list, setting all of them up and evenly splitting them in a circular pattern.
		/// </summary>
		public void UpdateRadialButtons()
		{
			if (!created) return;

			radialButtons.ForEach(button => button.SetupRadialButton(this));
			EvenlySplitOnUnitCircle();
		}

		/// <summary>
		/// Removes a RadialButton from this menu at the index "i".
		/// </summary>
		/// <param name="i">Index to remove.</param>
		public void RemoveRadialButton(int i)
		{
			radialButtons.RemoveAt(i);
		}

		internal void EvenlySplitOnUnitCircle()
		{
			for (int i = 0; i < radialButtons.Count; i++)
			{
				RadialButton radialButton = radialButtons[i];

				if (radialButton.positionOverride != Vector2.zero)
				{
					radialButton.rectTransform.anchoredPosition = radialButton.positionOverride;
					continue;
				}

				float point = GetPointOnUnitCircle(i);
				Vector2 position = (Vector2)RadialMenuHUD.RadialMenuHUDCamera.ScreenToWorldPoint(RadialMenuHUD.Center) + GetPositionOnUnitCircle(point) * outwards;

				radialButton.rectTransform.anchoredPosition = position;
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

			UpdateRadialButtons();

			gameObject.SetActive(false);
		}

		internal void RemoveRadialMenu()
		{
			created = false;
			radialButtons.ForEach(button => button.QuoteOnQuoteDestroyRadialButton());
		}

		/// <summary>
		/// Creates a blank RadialMenu. Recommended use of an initializer {}
		/// </summary>
		/// <param name="autoRegister">If the RadialMenu should be automatically registered on construction.</param>
		public RadialMenu(bool autoRegister = true)
		{
			if (autoRegister) RadialMenuManager.RegisterRadialMenu(this);
		}

		/// <summary>
		/// Creates a blank RadialMenu with a name. Recommended use of an initializer {}
		/// </summary>
		/// <param name="name">Name of the RadialMenu.</param>
		/// <param name="autoRegister">If the RadialMenu should be automatically registered on construction.</param>
		public RadialMenu(string name, bool autoRegister = true)
		{
			this.name = name;

			if (autoRegister) RadialMenuManager.RegisterRadialMenu(this);
		}

		public class RadialButton
		{
			public GameObject gameObject { get; private set; } = null;
			public Transform transform { get; private set; } = null;
			public RectTransform rectTransform { get; private set; } = null;

			public Image image { get; private set; } = null;
			public Button button { get; private set; } = null;
			public TextMeshProUGUI textField { get; private set; } = null;

			private Sprite m_buttonSprite = null;
			public Sprite buttonSprite
			{
				get { return m_buttonSprite; }
				set
				{
					if (created && image != null) image.sprite = value;
					m_buttonSprite = value;
				}
			}

			private Color m_buttonColor = Color.white;
			public Color buttonColor
			{
				get { return m_buttonColor; }
				set
				{
					if (created && image != null) image.color = value;
					m_buttonColor = value;
				}
			}

			private string m_text = string.Empty;
			public string text
			{
				get { return m_text; }
				set
				{
					string trimmed = value.Trim();

					if (created && textField != null) textField.text = displayText == string.Empty ? $"\"{trimmed}\"" : displayText;
					m_text = trimmed;
				}
			}

			private string m_displayText = string.Empty;
			public string displayText
			{
				get { return m_displayText; }
				set
				{
					if (created && textField != null) textField.text = displayText == string.Empty ? $"\"{value}\"" : displayText;
					m_displayText = value;
				}
			}

			private Func<char> m_punctuation = () => '.';
			public Func<char> punctuation
			{
				get { return m_punctuation; }
				set { m_punctuation = value; }
			}

			private Vector2 m_positionOverride = Vector2.zero;
			public Vector2 positionOverride
			{
				get { return m_positionOverride; }
				set
				{
					m_positionOverride = value;
					if (created && parentRadialMenu != null) parentRadialMenu.EvenlySplitOnUnitCircle();
				}
			}

			private Vector2 m_sizeDelta = Vector2.one * 75;
			public Vector2 sizeDelta
			{
				get { return m_sizeDelta; }
				set
				{
					if (created && rectTransform != null) rectTransform.sizeDelta = value;
					m_sizeDelta = value;
				}
			}

			private float m_textSize = 12f;
			public float textSize
			{
				get { return m_textSize; }
				set
				{
					if (created && textField != null) textField.fontSize = value;
					m_textSize = value;
				}
			}

			private Color m_textColor = Color.black;
			public Color textColor
			{
				get { return m_textColor; }
				set
				{
					if (created && textField != null) textField.color = value;
					m_textColor = value;
				}
			}

			private TextAlignmentOptions m_textAlignment = TextAlignmentOptions.Center;
			public TextAlignmentOptions textAlignment
			{
				get { return m_textAlignment; }
				set
				{
					if (created && textField != null) textField.alignment = value;
					m_textAlignment = value;
				}
			}

			public RadialMenu parentRadialMenu;
			public Func<RadialMenu> connectingRadialMenu = null;

			public Action<RadialMenu, RadialButton> preRadialButtonClicked = null;
			public Action<RadialMenu, RadialButton> postRadialButtonClicked = null;

			private string expressText => displayText == string.Empty ? $"\"{text}\"" : displayText;

			public delegate void RadialButtonClicked(RadialMenu radialMenu, RadialButton radialButton);
			public event RadialButtonClicked PreRadialButtonClicked;
			public event RadialButtonClicked PostRadialButtonClicked;

			public bool saveToHistory = true;
			private bool created = false;

			internal void QuoteOnQuoteDestroyRadialButton()
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

				textField = gameObject.AddComponent<TextMeshProUGUI>();
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

				image = gameObject.AddComponent<Image>();
				image.sprite = buttonSprite;
				image.color = buttonColor;

				button = gameObject.AddComponent<Button>();
				button.image = image;
				button.onClick.AddListener(OnClick);

				text = text.Trim();

				rectTransform = button.transform as RectTransform;
				rectTransform.sizeDelta = sizeDelta;
			}

			internal void OnClick()
			{
				preRadialButtonClicked?.Invoke(parentRadialMenu, this);
				PreRadialButtonClicked?.Invoke(parentRadialMenu, this);

				RadialMenu connectingRadialMenu = this.connectingRadialMenu?.Invoke();
				RadialMenu oldMenu = RadialMenuManager.CurrentMenu;

				if (saveToHistory) RadialMenuManager.AddRadialButtonHistory(this);

				if (connectingRadialMenu == null || connectingRadialMenu.radialButtons.Count <= 0) RadialMenuManager.SendChatText();
				else RadialMenuManager.SetCurrentMenu(connectingRadialMenu, RadialMenuManager.LastMenu != oldMenu);

				RadialMenuManager.RefreshMenu(oldMenu, connectingRadialMenu);

				postRadialButtonClicked?.Invoke(parentRadialMenu, this);
				PostRadialButtonClicked?.Invoke(parentRadialMenu, this);
			}

			/// <summary>
			/// Creates a blank RadialButton. Recommended use of an initializer {}
			/// </summary>
			public RadialButton() {}

			/// <summary>
			/// Creates a RadialButton that connects to another specified menu, and with the name of the menu displayed on the button. Recommended use of an initializer {}
			/// </summary>
			/// <param name="connectingRadialMenu">The menu that the button should connect to.</param>
			public RadialButton(RadialMenu connectingRadialMenu)
			{
				this.displayText = connectingRadialMenu.name;
				this.connectingRadialMenu = () => connectingRadialMenu;
			}

			/// <summary>
			/// Creates a RadialButton that connects to another specified menu, with the name of the menu displayed on the button, and a specified color. Recommended use of an initializer {}
			/// </summary>
			/// <param name="connectingRadialMenu">The menu that the button should connect to.</param>
			/// <param name="buttonColor">The color that the button should be.</param>
			public RadialButton(RadialMenu connectingRadialMenu, Color buttonColor)
			{
				this.displayText = connectingRadialMenu.name;
				this.buttonColor = buttonColor;
				this.connectingRadialMenu = () => connectingRadialMenu;
			}

			/// <summary>
			/// Creates a RadialButton with specified text. Recommended use of an initializer {}
			/// </summary>
			/// <param name="text">The text for the button to have.</param>
			public RadialButton(string text)
			{
				this.text = text;
			}

			/// <summary>
			/// Creates a RadialButton with specified text, and a specified color. Recommended use of an initializer {}
			/// </summary>
			/// <param name="text">The text that the button should contain.</param>
			/// <param name="buttonColor">The color that the button should be.</param>
			public RadialButton(string text, Color buttonColor)
			{
				this.text = text;
				this.buttonColor = buttonColor;
			}

			/// <summary>
			/// Creates a RadialButton with specified text, and a specified punctuation mark. Recommended use of an initializer {}
			/// </summary>
			/// <param name="text">The text the button should contain.</param>
			/// <param name="punctuation">The punctuation mark the button should use.</param>
			public RadialButton(string text, char punctuation)
			{
				this.text = text;
				this.displayText = $"\"{text}{punctuation}\"";
				this.punctuation = () => punctuation;
			}

			/// <summary>
			/// Creates a RadialButton with specified text, and specified display text that should show on the button. Recommended use of an initializer {}
			/// </summary>
			/// <param name="text">The text that the button should contain.</param>
			/// <param name="displayText">The display text that should appear on the button.</param>
			public RadialButton(string text, string displayText)
			{
				this.text = text;
				this.displayText = displayText;
			}

			/// <summary>
			/// Creates a RadialButton with specified text, and another specified menu that the button should connect to. Recommended use of an initializer {}
			/// </summary>
			/// <param name="text">The text that the button should contain.</param>
			/// <param name="connectingRadialMenu">The menu that the button should connect to.</param>
			public RadialButton(string text, RadialMenu connectingRadialMenu)
			{
				this.text = text;
				this.connectingRadialMenu = () => connectingRadialMenu;
			}

			/// <summary>
			/// Creates a RadialButton with specified text, specified display text that should appear on the button, and another specified menu that the button should connect to. Recommended use of an initializer {}
			/// </summary>
			/// <param name="text">The text that the button should contain.</param>
			/// <param name="displayText">The display text that should appear on the button.</param>
			/// <param name="connectingRadialMenu">The menu that the button should connect to.</param>
			public RadialButton(string text, string displayText, RadialMenu connectingRadialMenu)
			{
				this.text = text;
				this.displayText = displayText;
				this.connectingRadialMenu = () => connectingRadialMenu;
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