using TMPro;
using UnityEngine;

namespace View
{
    public class DebuggerCustom : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;


        public void Debug(string textDebug)
        {
            text.text += (textDebug + "\n");
        }
    }
}
