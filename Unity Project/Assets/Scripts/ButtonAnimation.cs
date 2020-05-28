using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ButtonAnimationEvent { PlayAnimationAndToMainScene, PlayAnimationAndToStartScene, PlayAnimation }

public class ButtonAnimation : MonoBehaviour
{
    public AnimationClip clip;
    Animation anim;

    AnimationEvent evt;
    public ButtonAnimationEvent buttonAnimationEvent;

    void Start()
    {
        evt = new AnimationEvent();

        switch (buttonAnimationEvent)
        {
            case ButtonAnimationEvent.PlayAnimation:
                break;

            case ButtonAnimationEvent.PlayAnimationAndToMainScene:
                evt.time = 0.7f;
                evt.functionName = "ToMainScene";
                clip.AddEvent(evt);
                break;

            case ButtonAnimationEvent.PlayAnimationAndToStartScene:
                evt.time = 0.7f;
                evt.functionName = "ToStartScene";
                clip.AddEvent(evt);
                break;
        }

        anim = GetComponent<Animation>();
        anim.clip = clip;
    }

    public void Play()
    {
        anim.Play();
    }

    public void ToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
