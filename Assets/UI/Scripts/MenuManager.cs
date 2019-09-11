using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Video;

public class MenuManager: MonoBehaviour {
    [Header("Main menu")]
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private ThreatMenu threatMenu;
    [SerializeField] private CreditsMenu creditsMenu;
    [SerializeField] private SoundEffectsVolumeMenu soundEffectsMenu;
    [SerializeField] private MusicVolumeMenu musicVolumeMenu;
    [SerializeField] private ControlSchemeMenu controlsMenu;
    [SerializeField] private RumbleMenu rumbleMenu;
    [SerializeField] private QualityMenu qualityMenu;
    [SerializeField] private HighscoreMenu highscoreMenu;

    [Header("Pause menu")]
    [SerializeField] private PauseMenu pauseMenu;
    [Header("Game Over menu")]
    [SerializeField] private GameOverMenu gameOverMenu;
    [SerializeField] private bool keepBaseMenuUp = true;
    [SerializeField] private bool showMainMenuOnStartUp = true;
    private Stack<BaseMenu> menuStack = new Stack<BaseMenu>();
    public Action OnAllMenusClosed;

    public static MenuManager Instance { get; set; }

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
  
        if(showMainMenuOnStartUp) {
            MainMenu.Show();
        }
    }

    private void Update() {
        int index = keepBaseMenuUp ? 1 : 0;
        if(menuStack.Count < 1) {
            return;
        }

        if(Input.GetButtonDown("Super") && menuStack.Count > index) {
            menuStack.Peek().Back();
        }
    }

    public void Create<T>() where T : BaseMenu {
        var prefab = GetPrefab<T>();
        if(menuStack.Count > 0) {
            menuStack.Peek().gameObject.SetActive(false);
        }
        menuStack.Push(Instantiate(prefab, transform));
    }

    private T GetPrefab<T>() where T : BaseMenu {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;
        FieldInfo[] info = GetType().GetFields(flags);
        foreach(var field in info) {
            var prefab = field.GetValue(this) as T;
            if(prefab != null) {
                return prefab;
            }
        }
        throw new MissingReferenceException("Prefab not found in MenuManager Create for type " + typeof(T));
    }

    public void Close(BaseMenu menu) {
        int index = keepBaseMenuUp ? 1 : 0;
        if(menuStack.Count < index) {
            Debug.LogWarning("Can't close this menu. It is the last one.");
            return;
        }
        if(menuStack.Peek() != menu) {
            Debug.LogWarning("Menu you want to close is not currently showing");
            return;
        }
        CloseTopMenu();
    }

    private void CloseTopMenu() {
        BaseMenu menu = menuStack.Pop();
        Destroy(menu.gameObject);
        if(menuStack.Count > 0) {
            menuStack.Peek().gameObject.SetActive(true);
            return;
        }
        OnAllMenusClosed?.Invoke();
    }

    public void CloseAllMenus() {
        int index = menuStack.Count;
        for(int i = 0; i < index; i++) {
            BaseMenu menu = menuStack.Pop();
            Destroy(menu.gameObject);
        }
    }

    private void OnDestroy() {
        Instance = null;
    }
}