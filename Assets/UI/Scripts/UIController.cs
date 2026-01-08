using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
    {
        private Player player = Player.Player1;

        private Label l;
        private UIDocument _doc;

        public static UIController Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            GeneralCommands.InitIcons();
            _doc = GetComponent<UIDocument>();

            Button b = _doc.rootVisualElement.Q<Button>("MainMenu");

            b.clicked += () => SceneManager.LoadScene("MainMenu");

            l = _doc.rootVisualElement.Q<Label>("Resource1Text");
            
            MarkSelection();
        }

        private void Update()
        {
            l.text = player.Resource1Amount.ToString();

            if (selection != null)
            {
                UpdateSingleUnitStats();
                
            } else

            if (multiSelection != null)
            {
                UpdateMultiUnitStats();
            }
            else
            {
                _doc.rootVisualElement.Q("SingleUnitInfo").style.display = DisplayStyle.None;
                _doc.rootVisualElement.Q("MultiUnitInfo").style.display = DisplayStyle.None;
            }
            
            UpdateCommandCard();
        }

        private void UpdateMultiUnitStats()
        {
            
            
        }

        private Unit selection;
        private List<Unit> multiSelection;

        public void MarkSelection(Unit u)
        {
            _doc.rootVisualElement.Q("SingleUnitInfo").style.display = DisplayStyle.Flex;
            _doc.rootVisualElement.Q("MultiUnitInfo").style.display = DisplayStyle.None;

            selection = u;
            multiSelection = null;

        }

        public void UpdateSingleUnitStats()
        {
            var infoEl = _doc.rootVisualElement.Q("SingleUnitInfo");

            infoEl.Q<Label>("UnitName").text = selection.Typename;
            infoEl.Q<Label>("UnitHP").text = selection.HP.ToString();
            infoEl.Q<Label>("UnitMaxHP").text = selection.MaxHp.ToString();
            infoEl.Q<Label>("DamageVal").text = "not implemented";
            infoEl.Q<Label>("RangeVal").text = "not implemented";
            infoEl.Q<Label>("AttackSpeedVal").text = "not implemented";
            infoEl.Q<Label>("MoveSpeedVal").text = "not implemented";
            infoEl.Q<Label>("ArmorVal").text = "not implemented";

            
            
            
        }

        public void UpdateCommandCard()
        {
            
            var commandCardVisEl = _doc.rootVisualElement.Q("Commands");
            var commandCard = UnitSelectionManager.Instance.getCommandCard();
            
            commandCardVisEl.Q("Q").style.backgroundImage = commandCard?.Q?.Icon;
            commandCardVisEl.Q("W").style.backgroundImage = commandCard?.W?.Icon;
            commandCardVisEl.Q("E").style.backgroundImage = commandCard?.E?.Icon;
            commandCardVisEl.Q("R").style.backgroundImage = commandCard?.R?.Icon;
            commandCardVisEl.Q("T").style.backgroundImage = commandCard?.T?.Icon;
            commandCardVisEl.Q("A").style.backgroundImage = commandCard?.A?.Icon;
            commandCardVisEl.Q("S").style.backgroundImage = commandCard?.S?.Icon;
            commandCardVisEl.Q("D").style.backgroundImage = commandCard?.D?.Icon;
            commandCardVisEl.Q("F").style.backgroundImage = commandCard?.F?.Icon;
            commandCardVisEl.Q("G").style.backgroundImage = commandCard?.G?.Icon;
            commandCardVisEl.Q("Z").style.backgroundImage = commandCard?.Z?.Icon;
            commandCardVisEl.Q("X").style.backgroundImage = commandCard?.X?.Icon;
            commandCardVisEl.Q("C").style.backgroundImage = commandCard?.C?.Icon;
            commandCardVisEl.Q("V").style.backgroundImage = commandCard?.V?.Icon;
            commandCardVisEl.Q("B").style.backgroundImage = commandCard?.B?.Icon;
        }
        
        public void MarkSelection(List<Unit> u)
        {
            if (u.Count == 0)
            {
                MarkSelection();
                return;
            };
            
            if (u.Count == 1)
            {
                MarkSelection(u[0]);
                return;
            }
            
            _doc.rootVisualElement.Q("SingleUnitInfo").style.display = DisplayStyle.None;
            _doc.rootVisualElement.Q("MultiUnitInfo").style.display = DisplayStyle.Flex;
            
            selection = null;
            multiSelection = u;
        }

        public void MarkSelection()
        {
            
            selection = null;
            multiSelection = null;
                
            _doc.rootVisualElement.Q("SingleUnitInfo").style.display = DisplayStyle.None;
            _doc.rootVisualElement.Q("MultiUnitInfo").style.display = DisplayStyle.None;
        }
    }
