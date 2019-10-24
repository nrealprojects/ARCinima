namespace NRToolkit.Record
{
    using UnityEngine;
    using UnityEngine.UI;

    public class NRRecordPreviewer : MonoBehaviour
    {
        public RawImage PreviewScreen;
        public Image StateIcon;

        public void SetData(Texture tex, bool isplaying)
        {
            PreviewScreen.texture = tex;
            StateIcon.color = isplaying ? Color.green : Color.red;
        }
    }
}
