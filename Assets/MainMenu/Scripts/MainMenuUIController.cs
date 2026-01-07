using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var doc = GetComponent<UIDocument>();

        Button b = doc.rootVisualElement.Q<Button>("StartGame");
        Button q = doc.rootVisualElement.Q<Button>("Exit");
        
        b.clicked += BOnclicked;
        q.clicked += Application.Quit;
    }

    private void BOnclicked()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
        // TODO : Get Player List and Setup Players
    }
}
