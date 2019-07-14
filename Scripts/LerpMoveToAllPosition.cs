using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMoveToAllPosition : MonoBehaviour
{

    [SerializeField]
    private float m_time;

    [SerializeField]
    private Vector3[] m_positions;

    private Coroutine m_coroutine;

    public void OnStartLerpMove()
    {
        if (m_coroutine == null)
            m_coroutine = StartCoroutine(LerpMove());
    }

    private void MovePosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private IEnumerator LerpMove()
    {
        int currentPoint = 0;
        // 當走過所有點就離開
        while(m_positions.Length - 1 > currentPoint)
        {
            // 每到一個點需要花費的時間
            float animaClipTime = m_time / m_positions.Length;
            float animaCurrentClipTime = 0;
            while (animaClipTime > animaCurrentClipTime)
            {
                //每個點花費的時間做正規化
                MovePosition(Vector3.Lerp(m_positions[currentPoint], m_positions[currentPoint + 1], animaCurrentClipTime / animaClipTime));
                animaCurrentClipTime += Time.deltaTime;
                yield return null;
            }

            // 到達下個點代表完成一個座標
            currentPoint += 1;
            yield return null;
        }

        // 完成所有點
        m_coroutine = null;
    }
}
