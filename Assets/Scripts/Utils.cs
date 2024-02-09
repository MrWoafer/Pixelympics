using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public delegate void VoidDelegate();

    public void RunAfterSeconds(float time, VoidDelegate procedure)
    {
        StartCoroutine(RunAfterSecondsCoroutine(time, procedure));
    }
    private IEnumerator RunAfterSecondsCoroutine(float time, VoidDelegate procedure)
    {
        yield return new WaitForSeconds(time);
        procedure();
    }

    public delegate bool BoolDelegate();

    public void RunAfterConditionMet(BoolDelegate condition, VoidDelegate procedure)
    {
        StartCoroutine(RunAfterConditionMetCoroutine(condition, procedure));
    }
    private IEnumerator RunAfterConditionMetCoroutine(BoolDelegate condition, VoidDelegate procedure)
    {
        yield return new WaitUntil(() => condition());
        procedure();
    }
}
