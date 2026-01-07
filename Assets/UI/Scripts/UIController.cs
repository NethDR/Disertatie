using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
    {
        private Player player = Player.Player1;

        private Label l;

        private void Start()
        {
            var doc = GetComponent<UIDocument>();

            Button b = doc.rootVisualElement.Q<Button>("MainMenu");

            b.clicked += () => SceneManager.LoadScene("MainMenu");

            l = doc.rootVisualElement.Q<Label>("Resource1Text");

            // StartCoroutine(IncrementResourceTest());

        }

        private void Update()
        {
            l.text = player.Resource1Amount.ToString();
        }

        // IEnumerator IncrementResourceTest()
        // {
        //     // TODO REMOVE AFTER TESTING
        //     
        //     while (true)
        //     {
        //         player.Resource1Amount++;
        //         yield return new WaitForSeconds(1);
        //     }
        // }
    }
