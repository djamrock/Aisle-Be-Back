using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public HeliController heliController;

    // Game States
    public GameObject TitleScreenStateObject;
    public GameObject MainMenuScreenStateObject;
    public GameObject OptionsScreenStateObject;
    public GameObject CreditsScreenStateObject;
    public GameObject GameplayStateObject;
    public GameObject VictoryScreenStateObject;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
