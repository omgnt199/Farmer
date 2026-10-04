using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
public class ConstructionUnlockCounterView : MonoBehaviour
{
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private TMP_Text counterTimeTxt;
    private StringBuilder counterTimeSb;
    public void Show(float duration, Action onCounterEnd)
    {
        StartCoroutine(UnlockCounterCoroutine(duration, onCounterEnd));
    }

    IEnumerator UnlockCounterCoroutine(float duration, Action onCounterEnd)
    {
        float timer = duration;
        counterTimeSb = new StringBuilder();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateCounterText(timer);
            UpdateProgressBar(1 - timer / duration);
            yield return null;
        }
        onCounterEnd?.Invoke();
        Destroy(gameObject);
    }
    void UpdateCounterText(float value)
    {
        counterTimeSb.Clear();
        counterTimeSb.Append(value.ToString("F2"));
        counterTimeSb.Append("s");
        counterTimeTxt.text = counterTimeSb.ToString();
    }
    void UpdateProgressBar(float value)
    {
        progressBar.anchorMax = new Vector2(value, 1f);
    }
}