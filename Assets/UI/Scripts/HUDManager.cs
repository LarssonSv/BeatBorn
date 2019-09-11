#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    [Header("Connections:")]
    [SerializeField] Image LeftCoreHpImage;
    [SerializeField] Image RightCoreHpImage;
    [SerializeField] GameObject playerHPDecal;
    [SerializeField] GameObject BeatVFX;
    [SerializeField] public float HealthBarColorChangeTime;
    [SerializeField] public Sprite LeftHurtImage;
    [SerializeField] public Sprite RightHurtImage;
    [SerializeField] public Sprite LeftOgImage;
    [SerializeField] public Sprite RightOgImage;

    private Vector3 defaultSizePlayerHp;

    private static HUDManager instanceHolder;
    public static HUDManager Instance {
        get {
            if(!instanceHolder) {
                instanceHolder = FindObjectOfType<HUDManager>();
            }
            if(!instanceHolder) {
                GameObject go = GameObject.Find("Canvas");
                if(go) {
                    instanceHolder = go.AddComponent<HUDManager>();
                }
            }
            if(!instanceHolder) {
                instanceHolder = new GameObject("HUDManager").AddComponent<HUDManager>();
            }
            return instanceHolder;
        }
    }

    private void Awake() {
        if(instanceHolder && instanceHolder != this) {
            Destroy(gameObject);
        }
        
    }

    public void Start() {
        //    playerHPUI.Init(3);
        //PlayerEngine.OnLifeChanged += UpdatePlayerHPUI;
    }

    public void UpdateNexusHPUI(int maxHp, int hp)
    {
        float x = (float)hp / maxHp;
        LeftCoreHpImage.sprite = LeftHurtImage;
        RightCoreHpImage.sprite = RightHurtImage;

        LeftCoreHpImage.fillAmount = x;
        RightCoreHpImage.fillAmount = x;

        //StartCoroutine(HurBar(LeftCoreHpImage, RightCoreHpImage, HealthBarColorChangeTime));
        Invoke("RevertBarsColor", HealthBarColorChangeTime);

    }

    public void UpdatePlayerHPUI(int maxHp, int hp)
    {
        //playerHPDecal.transform.localScale = (Vector3.one * ((float)hp / (float)maxHp));
    }

    public void SetHudEnabled(bool enabled)
    {
        BeatVFX.SetActive(enabled);

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(enabled);
    }

    void RevertBarsColor()
    {
        LeftCoreHpImage.sprite = LeftOgImage;
        RightCoreHpImage.sprite = RightOgImage;
    }



   /* private IEnumerator HurBar(Image bar1, Image bar2, float redTime, bool left = true, bool right = true)
    {
        Sprite og1Sprite = bar1.sprite;
        Sprite og2Sprite = bar2.sprite;
        bar1.sprite =  left ? LeftHurtImage : RightHurtImage;
        bar2.sprite = right ? LeftHurtImage : RightHurtImage;
        while (redTime > 0)
        {
            redTime -= Time.deltaTime;
            yield return null;
        }

        bar1.sprite = og1Sprite;
        bar2.sprite = og2Sprite;
    }*/

}