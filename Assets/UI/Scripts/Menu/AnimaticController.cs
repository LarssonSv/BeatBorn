using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class AnimaticController : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    private bool wantsToSkipp;
    // Start is called before the first frame update
    void Start()
    {
        _videoPlayer = GetComponentInChildren<VideoPlayer>();
        _videoPlayer.Prepare();
        _videoPlayer.transform.GetChild(0).gameObject.SetActive(false);
        _videoPlayer.transform.GetChild(1).gameObject.SetActive(false);
    }



    public void PlayAnimaticAndLoadSceene()
    {
        _videoPlayer.transform.GetChild(0).gameObject.SetActive(true);
        _videoPlayer.transform.GetChild(1).gameObject.SetActive(true);
        StartCoroutine(LoadAndPlay());
    }

    private IEnumerator LoadAndPlay()
    {
        AsyncOperation loadSceene = SceneManager.LoadSceneAsync(2);
        loadSceene.allowSceneActivation = false;
        _videoPlayer.Play();

        double timer = _videoPlayer.length;
        
        while (timer > 0 && !wantsToSkipp || loadSceene.progress < 0.9f)
        {
            timer -= Time.deltaTime;
            if(_videoPlayer.length - timer > 0.1f )
                wantsToSkipp = wantsToSkipp || Input.anyKeyDown;
            yield return null;
        }

        loadSceene.allowSceneActivation = true;
        Debug.Log(_videoPlayer.time);

        yield return null;

    }
}
