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
    public GameObject GameOverScreenStateObject;



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

    // Deactivate every game state as default
    private void DeactivateAllStates()
    {
        TitleScreenStateObject.SetActive(false);
        MainMenuScreenStateObject.SetActive(false);
        OptionsScreenStateObject.SetActive(false);
        CreditsScreenStateObject.SetActive(false);
        GameplayStateObject.SetActive(false);
        GameOverScreenStateObject.SetActive(false);
        VictoryScreenStateObject.SetActive(false);
    }

    public void ActivateTitleScreen()
    {
        // Deactivate all states
        DeactivateAllStates();

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(false);
        //}

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = true;
        //}

        // Activate the title screen
        TitleScreenStateObject.SetActive(true);
    }

    public void ActivateGameplayScreen()
    {
        // Deactivate all states
        DeactivateAllStates();

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = false;
        //}

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(true);
        //}

        //Time.timeScale = 1f;
        //gameEnded = false;
        //startingNewGame = true;
        //score = 0;

        //if (playerLives != null)
        //{
        //    playerLives.CancelInvoke(nameof(PlayerLives.RespawnPlayer));        // Cancel any delayed respawn from the previous game

        //    playerLives.ClearExistingPlayerShips();                             // Remove old player ships

        //    playerLives.lives = 3;                                              // Reset lives
        //    playerLives.UpdateLivesUI();

        //    playerLives.RespawnPlayer();                                        // Spawn the new player ship
        //}

        //if (levelData != null)                                                  // Reset the entities spawned by LevelData
        //{
        //    levelData.ResetEntities();

        //    if (playerController != null && playerController.pawn != null)      // Make the movement limit follow the new ship
        //    {
        //        levelData.player = playerController.pawn.transform;
        //    }
        //}

        // Activate the Gameplay screen
        GameplayStateObject.SetActive(true);
    }

    public void ActivateMainMenuScreen()
    {
        // Deactivate all states
        DeactivateAllStates();

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(false);
        //}

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = true;
        //}

        // Activate the Main Menu screen
        MainMenuScreenStateObject.SetActive(true);
    }

    public void ActivateOptionsScreen()
    {
        // Deactivate all states
        DeactivateAllStates();

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(false);
        //}

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = true;
        //}

        // Activate the Options screen
        OptionsScreenStateObject.SetActive(true);
    }

    public void ActivateCreditsScreen()
    {
        // Deactivate all states
        DeactivateAllStates();

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(false);
        //}

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = true;
        //}

        // Activate the Credits screen
        CreditsScreenStateObject.SetActive(true);
    }

    public void ActivateGameOverScreen()
    {
        //UpdateHighScore();

        //// Mark the game as ended
        //gameEnded = true;

        //// Deactivate all states
        //DeactivateAllStates();

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(false);
        //}

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = true;
        //}

        // Activate the Game Over screen
        GameOverScreenStateObject.SetActive(true);
    }
    public void ActivateVictoryScreen()
    {
        //UpdateHighScore();

        //// Deactivate all states
        //DeactivateAllStates();

        //if (gameplayAudioListener != null)
        //{
        //    gameplayAudioListener.SetActive(false);
        //}

        //if (cameraAudioListener != null)
        //{
        //    cameraAudioListener.enabled = true;
        //}

        // Activate the Victory screen
        VictoryScreenStateObject.SetActive(true);
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
