using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class AnimatorExtension {
    public static bool isPlayingOnLayer(this Animator animator, int fullPathHash, int layer) {
        return animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == fullPathHash;


    }

    public static double normalizedTime(this Animator animator, System.Int32 layer) {
        double time = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        return time > 1 ? 1 : time;
    }

}


[System.Serializable]
public class AnimationClipExtended {
    public Animator animator;
    public AnimationClip clip;
    public string animatorStateName;
    public int layerNumber;

    private int _totalFrames = 0;
    private int _animationFullNameHash;

    public void initialize() {
        _totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);

        if (animator.isActiveAndEnabled) {
            string name = animator.GetLayerName(layerNumber) + "." + animatorStateName;

            _animationFullNameHash = Animator.StringToHash(name);

        }

    }

    public bool itsOnLastFrame() {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage > percentageOnFrame(_totalFrames - 1));
    }

    public int totalFrames() {
        return _totalFrames;
    }


    public bool isActive() {
        return animator.isPlayingOnLayer(_animationFullNameHash, 0);

    }


    public bool biggerOrEqualThanFrame(int frameNumber) {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage >= percentageOnFrame(frameNumber));
    }

    public bool itsOnFrame(int frameNumber) {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage >= percentageOnFrame(frameNumber) && (percentage < percentageOnFrame(frameNumber + 1)));
    }

    double percentageOnFrame(int frameNumber) {
        return (double)frameNumber / (double)_totalFrames;
    }

}

public interface IFrameCheckHandler {
    void onHitFrameStart();
    void onHitFrameEnd();
    void onLastFrameStart();
    void onLastFrameEnd();
}


[System.Serializable]
public class FrameChecker {
    public int hitFrameStart;
    public int hitFrameEnd;
    public int totalFrames;

    private IFrameCheckHandler _frameCheckHandler;
    private AnimationClipExtended _extendedClip;
    private bool _checkedHitFrameStart;
    private bool _checkedHitFrameEnd;
    private bool _lastFrame;

    public void initialize(IFrameCheckHandler frameCheckHandler, AnimationClipExtended extendedClip) {
        _frameCheckHandler = frameCheckHandler;

        _extendedClip = extendedClip;

        totalFrames = extendedClip.totalFrames();

        initCheck();

    }


    public void initCheck() {
        _checkedHitFrameStart = false;

        _checkedHitFrameEnd = false;

        _lastFrame = false;

    }

    public void checkFrames() {
        if (_lastFrame) {

            _lastFrame = false;

            _frameCheckHandler.onLastFrameEnd();

        }


        if (!_extendedClip.isActive()) { return; }


        if (!_checkedHitFrameStart && _extendedClip.biggerOrEqualThanFrame(hitFrameStart)) {

            _frameCheckHandler.onHitFrameStart();

            _checkedHitFrameStart = true;

        }
        else if (!_checkedHitFrameEnd && _extendedClip.biggerOrEqualThanFrame(hitFrameEnd)) {

            _frameCheckHandler.onHitFrameEnd();

            _checkedHitFrameEnd = true;

        }

        if (!_lastFrame && _extendedClip.itsOnLastFrame()) {

            _frameCheckHandler.onLastFrameStart();

            _lastFrame = true;

        }

    }

}



#if UNITY_EDITOR
[CustomEditor(typeof(FrameChecker))]
[CanEditMultipleObjects]

public class FrameCheckerEditor : Editor {

}

#endif