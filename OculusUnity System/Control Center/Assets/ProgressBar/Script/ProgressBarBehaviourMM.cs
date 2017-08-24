using ProgressBar.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressBar
{
    /// <summary>
    /// This Script is directed at linearly progressing designs.
    /// </summary>
    public class ProgressBarBehaviourMM : ProgressBarBehaviour
    {
        /// <summary>
        /// This method is used to set the Filler's width and display mm
        /// </summary>
        /// <param name="Width">the new Filler's width</param>
        public override void SetFillerSize(float value)
        {

            if (value * 8 < FillerInfo.MaxWidth)
            {
                m_FillRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, XOffset, value * 8);
                if (m_AttachedText)
                {
                    m_AttachedText.text = Mathf.Round(value / FillerInfo.MaxWidth * 100).ToString() + " mm";
                }
            }
            else
            {
                m_FillRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, XOffset, FillerInfo.MaxWidth);
                if (m_AttachedText)
                {
                    m_AttachedText.text = "Invalid";
                }
            }
        }
    }
}