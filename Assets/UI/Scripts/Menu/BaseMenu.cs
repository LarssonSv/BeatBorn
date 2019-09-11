using UnityEngine;

public class BaseMenu<T> : BaseMenu where T: BaseMenu<T>{
    [SerializeField] private bool showInputVisualizer = true;
    public static T Instance { get; private set; }
    private void Awake() {
        Instance = (T)this;
        Canvas canvas = Instance.GetComponent<Canvas>();
        if(!showInputVisualizer) {
            return;
        }
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.planeDistance = 1;
        canvas.worldCamera = Camera.main;
    }

    public static void Show() {
        MenuManager.Instance.Create<T>();
    }

    public static void Close() {
        if(Instance == null) {
            Debug.LogWarning("Trying to close already closed window {0}." + typeof(T));
        }
        MenuManager.Instance.Close(Instance);
    }

    public override void Back() {
        Close();
    }

    private void OnDestroy() {
        Instance = null;
    }
}

public abstract class BaseMenu : MonoBehaviour {
    public abstract void Back();
}