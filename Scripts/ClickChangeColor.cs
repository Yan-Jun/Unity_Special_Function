using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorData
{
    public Color32 color;
    public int clickTotal;
}

public class ClickChangeColor : MonoBehaviour
{
    [SerializeField]
    private Renderer m_targetRenderer;

    [SerializeField]
    private List<ColorData> m_colors;

    [SerializeField]
    private int m_clickCounter;

    [SerializeField]
    private int m_nextClickNum;

    [SerializeField]
    private string m_saveClickTotalKey = "ClickTotal";

    private Material m_material;

    // 切換顏色
    private int m_length;
    private int m_index;

    // 顏色漸變
    private Color m_tmpColor;
    private float m_CurClickNum;
    private float m_curClickMax;

    // 原始顏色
    private Color m_originColor;

	void Start () {

        Init();
    }

    private void Init()
    {
        m_material = m_targetRenderer.material;

        m_length = m_colors.Count;
        m_originColor = m_material.color;
        m_tmpColor = m_material.color;

        if (m_length > 0)
        {
            m_nextClickNum = m_colors[m_index].clickTotal;
            m_curClickMax = m_nextClickNum - m_clickCounter;
        }

        ReloadColor();
    }

    public void ReloadColor()
    {
        if (!PlayerPrefs.HasKey(m_saveClickTotalKey))
        {
            PlayerPrefs.SetInt(m_saveClickTotalKey, 0);
            PlayerPrefs.Save();
        }

        // 儲存的點擊次數
        m_clickCounter = PlayerPrefs.GetInt(m_saveClickTotalKey);
        m_index = 0;

        // 第一筆顏色紀錄
        if (m_clickCounter < m_colors[0].clickTotal)
        {
            m_tmpColor = m_originColor;
            m_CurClickNum = m_clickCounter;
            m_curClickMax = m_colors[0].clickTotal;
            m_material.color = Color.Lerp(m_tmpColor, m_colors[m_index].color, m_CurClickNum / m_curClickMax);
            m_nextClickNum = m_colors[0].clickTotal;
            return;
        }

        for (int i = 1; i < m_length; i++)
        {
            
            if(m_clickCounter >= m_colors[i].clickTotal)
            {
                m_nextClickNum = m_colors[i].clickTotal;
                m_material.color = m_colors[i].color;
                m_tmpColor = m_colors[i].color;
                m_index = i;
            }
            else if (m_clickCounter < m_colors[i].clickTotal)
            {
                m_index = i;
                m_nextClickNum = m_colors[m_index].clickTotal;

                // 前一顏色暫存
                m_tmpColor = m_colors[m_index - 1].color;

                // 兩個顏色之間的差
                m_curClickMax = m_nextClickNum - m_colors[m_index - 1].clickTotal;

                // 儲存的點擊做正規化
                m_CurClickNum = m_clickCounter - m_colors[m_index - 1].clickTotal;

                m_material.color = Color.Lerp(m_tmpColor, m_colors[m_index].color, m_CurClickNum / m_curClickMax);

                break;
            }
            else
            {
                Debug.Log("比較失敗");
            }
        }
    }
	
    public void OnClickChangeColor()
    {

        if (m_length <= m_index) return;

        m_clickCounter++;
        m_CurClickNum += 1f;

        Color lerpColor = Color.Lerp(m_tmpColor, m_colors[m_index].color, m_CurClickNum / m_curClickMax);
        m_material.color = lerpColor;

        if (m_clickCounter >= m_nextClickNum)
        {
            m_index++;
            if (m_length > m_index)
            {
                m_nextClickNum = m_colors[m_index].clickTotal;

                // 正規劃
                m_tmpColor = m_material.color;
                m_CurClickNum = 0f;
                m_curClickMax = m_nextClickNum - m_clickCounter;
            }
            
        }

        // 保存點擊次數
        PlayerPrefs.SetInt(m_saveClickTotalKey, m_clickCounter);
        PlayerPrefs.Save();
    }
}
