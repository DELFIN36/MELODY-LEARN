using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CustomProgrammingLanguage : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public Button executeButton;
    private SoundGen soundGen;

    void Start()
    {
        soundGen = FindObjectOfType<SoundGen>();
        executeButton.onClick.AddListener(ExecuteCode);
    }

    IEnumerator PlayNoteRepeatedly(string noteStatement, int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            ParseAndExecute(noteStatement); // Ejecutar la instrucción de tocar la nota
            yield return new WaitForSeconds(0.5f); // Pausa de 0.5 segundos entre cada reproducción
        }
    }

    void ExecuteCode()
    {
        string code = codeInputField.text;
        ParseAndExecute(code);
    }

    void ParseAndExecute(string code)
{
    List<string> notes = new List<string>{"do", "re", "mi", "fa", "sol", "la", "si", "do#", "re#", "mi#", "fa#", "sol#", "la#", "si#"};
    List<string> commands = new List<string>(); // Lista para almacenar las instrucciones de TOCAR y REPETIR

    string[] statements = code.Split(';');
    
    foreach (string statement in statements)
    {
        string trimmedStatement = statement.Trim();
        
        if (!string.IsNullOrEmpty(trimmedStatement))
        {
            commands.Add(trimmedStatement);
        }
    }

    // Procesar las instrucciones en el orden correcto
    foreach (string command in commands)
    {
        // Verificar si la instrucción es REPETIR
        if (command.StartsWith("REPETIR(") && command.EndsWith(")"))
        {
            int firstParenthesisIndex = command.IndexOf('(');
            int lastParenthesisIndex = command.LastIndexOf(')');
            string innerStatement = command.Substring(firstParenthesisIndex + 1, lastParenthesisIndex - firstParenthesisIndex - 1).Trim();

            int commaIndex = innerStatement.IndexOf(',');
            if (commaIndex != -1)
            {
                string noteStatement = innerStatement.Substring(0, commaIndex).Trim();
                string repeatCountString = innerStatement.Substring(commaIndex + 1).Trim();

                if (int.TryParse(repeatCountString, out int repeatCount))
                {
                    // Ejecutar la instrucción TOCAR(nota) 'repeatCount' veces
                    for (int i = 0; i < repeatCount; i++)
                    {
                        ParseAndExecute(noteStatement);
                    }
                }
                else
                {
                    Debug.LogWarning("Invalid repeat count: " + repeatCountString);
                }
            }
            else
            {
                Debug.LogWarning("Invalid repeat statement: " + innerStatement);
            }
        }
        // Verificar si la instrucción es TOCAR
        else if (command.StartsWith("TOCAR(") && command.EndsWith(")"))
        {
            int startIndex = command.IndexOf('(');
            int endIndex = command.LastIndexOf(')');
            if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
            {
                string note = command.Substring(startIndex + 1, endIndex - startIndex - 1).ToLower();
                if (notes.Contains(note))
                {
                    int midiNote = notes.IndexOf(note) + 60;
                    soundGen.OnKey(midiNote); // Tocar la nota
                }
                else
                {
                    Debug.LogWarning("Note not recognized: " + note);
                }
            }
            else
            {
                Debug.LogWarning("Invalid statement: " + command);
            }
        }
        else
        {
            Debug.LogWarning("Invalid statement: " + command);
        }
    }
}

}
