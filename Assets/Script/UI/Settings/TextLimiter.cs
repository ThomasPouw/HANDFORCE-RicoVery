using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLimiter : MonoBehaviour
{
    public TMP_InputField colorInputField;
    private string lastString;
    private static readonly Regex hexColorRegex = new Regex("^#([0-9A-Fa-f]{6})$", RegexOptions.Compiled);

    public void isStringHexRBG(string input)
    {
        if(hexColorRegex.IsMatch(input))
        {
            lastString = input;
            return;
        }
        //colorInputField.text = lastString;

    }
}
