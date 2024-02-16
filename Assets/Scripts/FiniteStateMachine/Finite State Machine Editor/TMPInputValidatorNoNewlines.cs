using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "InputValidator - NoNewlines.asset", menuName = "TextMeshPro/Input Validators/NoNewlines")]
public class TMPInputValidatorNoNewlines : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (ch != '\n')
        {
            //text += ch;
            text = text.Insert(pos, ch.ToString());
            pos += 1;
            return ch;
        }
        return (char)0;
    }
}
