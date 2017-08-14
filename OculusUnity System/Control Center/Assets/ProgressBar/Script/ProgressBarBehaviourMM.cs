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
        public override void SetFillerSize(float Width)
        {
            if (m_AttachedText)
                m_AttachedText.text = Mathf.Round(Width / FillerInfo.MaxWidth * 100).ToString() + " mm";

            m_FillRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, XOffset, Width);
        }
    }
}