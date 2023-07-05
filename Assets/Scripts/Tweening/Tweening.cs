using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TweenSequence {
    [Header("---  Options  ---")]
    [SerializeField] bool returnToBeginning;
    [SerializeField] [Min(0.01f)] float tweenTimeMultiplier;

    [Header("---  Easing  ---")]
    [SerializeField] LeanTweenType easeCurve;
    [SerializeField] bool useAnimationCurve;
    [SerializeField] AnimationCurve animationCurve;

    [Header("---  Events List  ---")]
    [SerializeField] public List<TweenEvent> events;
    LTSpline spline;

    public void Add(TweenEvent tweenEvent) {
        events.Add(new TweenEvent(tweenEvent));
    }

    public LTDescr DoTween(GameObject gameObject) {
        if (events == null || events.Count == 0) { return null; }

        // Get the objects information
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector2 position = copy(rect.localPosition);
        Vector3 scale = copy(rect.localScale);
        Quaternion angle = copy(rect.rotation);
        CanvasGroup canv = GetCanvas(gameObject);

        // Create a copy of the events for modification
        List<TweenEvent> tweens = copy(events);
        if (returnToBeginning) {
            tweens.Add(new TweenEvent(position, scale, copyToVector3(angle), canv.alpha, LeanTweenType.easeInOutCubic, events[0].timeMultiplier, 0f));
        }

        // Setup path and spline
        Vector3[] path = getPositionPath(gameObject, tweens);
        spline = new LTSpline(path, false);

        float eventSize = 1f / (float)events.Count;
        int eventIndices = events.Count - 1;
        int currentEventIndex = 0;
        TweenEvent currentEvent = tweens[currentEventIndex];
        currentEvent.SetInitialConditions(gameObject);

        LTDescr tween = LeanTween.moveLocal(gameObject, spline, tweenTimeMultiplier * TotalTime())
        .setOnStart(() => {
            currentEvent.SetInitialState();
        })
        .setOnUpdate((float value) => {
            int eventNum = Mathf.Min(Mathf.FloorToInt(value / eventSize), eventIndices);
            if (currentEventIndex != eventNum) {
                currentEventIndex = eventNum;
                currentEvent = tweens[currentEventIndex];
                currentEvent.SetInitialConditions(gameObject);
            }
            float eventProgress = (value - (float)eventNum * eventSize) / eventSize;
            currentEvent.SetState(eventProgress, false);
        })
        .setOnComplete(() => {
            currentEvent.SetEndState(false);
        });

        return useAnimationCurve ? tween.setEase(animationCurve) : tween.setEase(easeCurve);
    }


    // ----- UTILITIES -----

    List<TweenEvent> copy(List<TweenEvent> toCopy) {
        List<TweenEvent> copy = new List<TweenEvent>();
        foreach (TweenEvent tween in toCopy) {
            copy.Add(new TweenEvent(tween));
        }
        return copy;
    }

    public float TotalTime() {
        float time = 0f;
        foreach (TweenEvent tween in events) {
            time += tween.delay + tween.timeMultiplier * Settings.animationTime;
        }
        time += events[0].timeMultiplier;
        return time;
    }

    public void InvertAlpha() {
        if (events == null) { return; }
        for (int i = 0; i < events.Count; ++i) {
            events[i].alpha = 1f - events[i].alpha;
        }
    }

    public void TurnOffOnEnd() {
        if (events == null) { return; }
        if (events.Count != 0) {
            events[events.Count-1].turnOffOnEnd = true;
        }
    }


    // ---- POSITION AND PATHS -----

    Vector3[] getPositionPath(GameObject gameObject, List<TweenEvent> events) {
        List<Vector3> points = new List<Vector3>();

        points.Add(localPostiion(gameObject));
        points.Add(localPostiion(gameObject));
        foreach (var tween in events) {
            points.Add(tween.localPosition(gameObject));
        }
        points.Add(localPostiion(gameObject));

        Vector3[] array = new Vector3[points.Count];
        System.Array.Copy(points.ToArray(), array, points.Count);
        return array;
    }
    Vector3[] getPositionPath(GameObject gameObject) {
        return getPositionPath(gameObject, this.events);
    }

    Vector3 localPostiion(GameObject gameObject) {
        return copy(gameObject.GetComponent<RectTransform>().localPosition);
    }

    CanvasGroup GetCanvas(GameObject obj) {
        CanvasGroup canv = obj.GetComponent<CanvasGroup>();
        if (canv == null) {
            canv = obj.AddComponent<CanvasGroup>();
        }
        return canv;
    }
    Vector2 copy(Vector2 toCopy) {
        return new Vector2(toCopy.x, toCopy.y);
    }
    Vector3 copy(Vector3 toCopy) {
        return new Vector3(toCopy.x, toCopy.y, toCopy.z);
    }
    Vector3 copyToVector3(Quaternion toCopy) {
        return new Vector3(toCopy.x, toCopy.y, toCopy.z);
    }
    Quaternion copy(Quaternion toCopy) {
        return new Quaternion(toCopy.x, toCopy.y, toCopy.z, toCopy.w);
    }

    // ----- CONSTRUCTORS -----

    // Copy another sequence
    public TweenSequence(List<TweenEvent> tweenEvents, float timeMultiplier, bool returnToBeginning) {
        this.events = new List<TweenEvent>();
        foreach (TweenEvent tween in tweenEvents) {
            events.Add(new TweenEvent(tween));
        }
        this.tweenTimeMultiplier = timeMultiplier;
        this.returnToBeginning = returnToBeginning;
    }
    public TweenSequence(TweenSequence toCopy) {
        this.events = new List<TweenEvent>();
        foreach (TweenEvent tween in toCopy.events) {
            events.Add(new TweenEvent(tween));
        }
        this.tweenTimeMultiplier = toCopy.tweenTimeMultiplier;
        this.easeCurve = toCopy.easeCurve;
        this.useAnimationCurve = toCopy.useAnimationCurve;
        this.animationCurve = toCopy.animationCurve;
        this.returnToBeginning = toCopy.returnToBeginning;
    }
}



[System.Serializable]
public class TweenEvent {
    [SerializeField] string name;

    [Header("Rect Transform Stuff")]
    public Vector2 positionOffsetMultiplier = new Vector2(0f, 0f);
    public Vector3 scale = new Vector3(1f, 1f, 1f);
    public Vector3 rotationAngle = new Vector3(0f, 0f, 0f);
    [Range(0f, 1f)] public float alpha = 1f;

    [Header("Tween Options")]
    public LeanTweenType easeCurve = LeanTweenType.linear;
    [Min(0.01f)] public float timeMultiplier = 1f;
    [Min(0f)] public float delay = 0f;
    public bool turnOnOnStart = false, turnOffOnEnd = false;


    // ----- METHODS -----

    public LTDescr GetTweenInSequence(TweenEvent previousEvent) {
        if (rect == null) {
            Debug.LogError("Rect transform on tween event \"" + name.ToUpper() + "\" is not set. Not tweening.");
            return null;
        }
        return GetTweenInSequence(rect.gameObject, previousEvent);
    }
    public LTDescr GetTweenInSequence(GameObject gameObject, TweenEvent previousEvent) {
        SetInitialConditions(gameObject, previousEvent);
        return GetTween();
    }
    public LTDescr GetTween() {
        if (rect == null || !CheckInitialConditions()) {
            Debug.LogError("Initial conditions for tween event \"" + name.ToUpper() + "\" are not good. Not tweening.");
            return null;
        }
        GameObject gameObject = rect.gameObject;

        LTDescr tween = LeanTween.value(gameObject, 0f, 1f, Settings.animationTime * timeMultiplier)
        .setDelay(delay * timeMultiplier)
        .setEase(easeCurve)
        .setOnStart(() => {
            SetInitialState();
        })
        .setOnUpdate((float value) => {
            SetState(value);
        })
        .setOnComplete(() => {
            SetEndState();
        });

        return tween;
    }
    public LTDescr GetTween(GameObject gameObject) {
        SetInitialConditions(gameObject);
        return GetTween();
    }


    // ----- INITIAL CONDITIONS -----

    RectTransform rect;
    Vector2 positionStart, positionEnd;
    Vector3 scaleStart, scaleEnd;
    Vector3 angleStart, angleEnd;
    CanvasGroup canv;
    float alphaStart, alphaEnd;

    public void SetInitialState() {
        rect.localPosition = positionStart;
        rect.localScale = scaleStart;
        rect.localEulerAngles = copy(angleStart);
        canv.alpha = alphaStart;
        if (turnOnOnStart) {
            rect.gameObject.SetActive(true);
        }
    }
    public void SetState(float progressPercentage) {
        SetState(progressPercentage, true);
    }
    public void SetState(float progressPercentage, bool updateLocalPosition) {
        if (updateLocalPosition) {
            rect.localPosition = progress(positionStart, positionEnd, progressPercentage);
        }
        rect.localScale = progress(scaleStart, scaleEnd, progressPercentage);
        rect.localEulerAngles = progress(angleStart, angleEnd, progressPercentage);
        canv.alpha = progress(alphaStart, alphaEnd, progressPercentage);
    }
    public void SetEndState() {
        rect.localPosition = positionEnd;
        rect.localScale = scaleEnd;
        rect.localEulerAngles = copy(angleEnd);
        canv.alpha = alphaEnd;
        if (turnOffOnEnd) {
            rect.gameObject.SetActive(false);
        }
    }
    public void SetEndState(bool updateLocalPosition) {
        if (updateLocalPosition) {
            rect.localPosition = positionEnd;
        }
        rect.localScale = scaleEnd;
        rect.localEulerAngles = copy(angleEnd);
        canv.alpha = alphaEnd;
        if (turnOffOnEnd) {
            rect.gameObject.SetActive(false);
        }
    }

    public void SetInitialConditions(GameObject gameObject) {
        rect = gameObject.GetComponent<RectTransform>();
        positionStart = copy(rect.localPosition);
        positionEnd = copy(positionStart) + copy(positionOffsetMultiplier) * new Vector2(rect.rect.width, rect.rect.height);

        scaleStart = copy(rect.localScale);
        scaleEnd = copy(scale);

        angleStart = copy(rect.localEulerAngles);
        angleEnd = copy(rotationAngle);

        canv = GetCanvas(gameObject);
        alphaStart = canv.alpha;
        alphaEnd = alpha;
    }
    void SetInitialConditions(GameObject gameObject, TweenEvent previousEvent) {
        rect = gameObject.GetComponent<RectTransform>();
        positionStart = localPosition(rect, copy(previousEvent.positionOffsetMultiplier));
        positionEnd = localPosition(rect, positionOffsetMultiplier);

        scaleStart = copy(previousEvent.scale);
        scaleEnd = copy(scale);

        angleStart = copy(previousEvent.rotationAngle);
        angleEnd = copy(rotationAngle);

        canv = GetCanvas(gameObject);
        alphaStart = previousEvent.alpha;
        alphaEnd = alpha;
    }

    public bool CheckInitialConditions() {
        if (rect == null) { return false; }
        if (positionStart == null) { return false; }
        if (positionEnd == null) { return false; }
        if (scaleStart == null) { return false; }
        if (scaleEnd == null) { return false; }
        if (angleStart == null) { return false; }
        if (angleEnd == null) { return false; }
        if (canv == null) { return false; }
        return true;
    }


    // ----- CONSTRUCTORS -----

    public TweenEvent(Vector2 positionOffsetMultiplier, Vector3 scale, Vector3 rotationAngle, float alpha, LeanTweenType easeCurve, float timeMultiplier, float delay) {
        this.positionOffsetMultiplier = copy(positionOffsetMultiplier);
        this.scale = copy(scale);
        this.rotationAngle = copy(rotationAngle);
        this.alpha = alpha;
        this.easeCurve = easeCurve;
        this.timeMultiplier = timeMultiplier;
        this.delay = delay;
    }
    public TweenEvent(TweenEvent toCopy) {
        this.positionOffsetMultiplier = copy(toCopy.positionOffsetMultiplier);
        this.scale = copy(toCopy.scale);
        this.rotationAngle = copy(toCopy.rotationAngle);
        this.alpha = toCopy.alpha;
        this.easeCurve = toCopy.easeCurve;
        this.timeMultiplier = toCopy.timeMultiplier;
        this.delay = toCopy.delay;
        this.turnOnOnStart = toCopy.turnOnOnStart;
        this.turnOffOnEnd = toCopy.turnOffOnEnd;
    }


    // ----- UTILITIES -----

    public Vector2 localPosition(GameObject obj) {
        RectTransform rect = obj.GetComponent<RectTransform>();
        return copy(positionOffsetMultiplier) * new Vector2(rect.rect.width, rect.rect.height);
    }
    Vector2 localPosition(RectTransform rect, Vector2 positionOffsetMultiplier) {
        return copy(positionOffsetMultiplier) * new Vector2(rect.rect.width, rect.rect.height);
    }

    CanvasGroup GetCanvas(GameObject obj) {
        CanvasGroup canv = obj.GetComponent<CanvasGroup>();
        if (canv == null) {
            canv = obj.AddComponent<CanvasGroup>();
        }
        return canv;
    }

    float progress(float start, float end, float value) {
        return start + value * (end - start);
    }
    Vector2 progress(Vector2 start, Vector2 end, float value) {
        return start + value * (end - start);
    }
    Vector3 progress(Vector3 start, Vector3 end, float value) {
        return start + value * (end - start);
    }

    Vector2 copy(Vector2 toCopy) {
        return new Vector2(toCopy.x, toCopy.y);
    }
    Vector3 copy(Vector3 toCopy) {
        return new Vector3(toCopy.x, toCopy.y, toCopy.z);
    }
}