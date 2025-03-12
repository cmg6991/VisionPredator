using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RedFlags : MonoBehaviour
{
    private Light morseLight;

    public string message;  // 입력 메시지

    private float dotDuration = 0.2f;        // 점 길이
    private float dashDuration = 0.6f;       // 선 길이
    private float gapDuration = 0.2f;        // 점과 선 사이의 간격
    private float letterGapDuration = 1.0f;  // 문자 사이의 간격

    private string redFlagsLetter = default;
    private bool isPlaying;

    private void Start()
    {
        morseLight = GetComponent<Light>();

        if (message != null)
        {
            foreach (char letter in message)
            {
                redFlagsLetter += MessageLetter(letter);
                redFlagsLetter += " ";
            }

            if (morseLight == null)
                Debug.Log("None Light");
            else
                StartCoroutine(PlayMorseCode(redFlagsLetter));
        }
    }


    private IEnumerator PlayMorseCode(string morseCode)
    {
        isPlaying = true;
        foreach (char symbol in morseCode)
        {
            if (symbol == '.')
            {
                yield return BlinkLight(dotDuration);
            }
            else if (symbol == '-')
            {
                yield return BlinkLight(dashDuration);
            }
            else if (symbol == ' ')
            {
                yield return new WaitForSeconds(letterGapDuration);
            }

            yield return new WaitForSeconds(gapDuration);
        }

        yield return new WaitForSeconds(1.5f);
        isPlaying = false;
        morseLight.enabled = true;

    }

    private IEnumerator BlinkLight(float duration)
    {
        morseLight.enabled = true;
        yield return new WaitForSeconds(duration);
        morseLight.enabled = false;
    }


    private string MessageLetter(char _message)
    {
        string output;

        switch (_message)
        {
            case 'A':
            case 'a':
                output = ".-";
                break;
            case 'B':
            case 'b':
                output = "-..";
                break;
            case 'C':
            case 'c':
                output = "-.-.";
                break;
            case 'D':
            case 'd':
                output = "-..";
                break;
            case 'E':
            case 'e':
                output = ".";
                break;
            case 'F':
            case 'f':
                output = "..-.";
                break;
            case 'G':
            case 'g':
                output = "--.";
                break;
            case 'H':
            case 'h':
                output = "....";
                break;
            case 'I':
            case 'i':
                output = "..";
                break;
            case 'J':
            case 'j':
                output = ".---";
                break;
            case 'K':
            case 'k':
                output = "-.-";
                break;
            case 'L':
            case 'l':
                output = ".-..";
                break;
            case 'M':
            case 'm':
                output = "--";
                break;
            case 'N':
            case 'n':
                output = "-.";
                break;
            case 'O':
            case 'o':
                output = "---";
                break;
            case 'P':
            case 'p':
                output = ".--.";
                break;
            case 'Q':
            case 'q':
                output = "--.-";
                break;
            case 'R':
            case 'r':
                output = ".-.";
                break;
            case 'S':
            case 's':
                output = "...";
                break;
            case 'T':
            case 't':
                output = "-";
                break;
            case 'U':
            case 'u':
                output = "..-";
                break;
            case 'V':
            case 'v':
                output = "...-";
                break;
            case 'W':
            case 'w':
                output = ".--";
                break;
            case 'X':
            case 'x':
                output = "-..-";
                break;
            case 'Y':
            case 'y':
                output = "-.--";
                break;
            case 'Z':
            case 'z':
                output = "--..";
                break;
            case '1':
                output = ".----";
                break;
            case '2':
                output = "..---";
                break;
            case '3':
                output = "...--";
                break;
            case '4':
                output = "....-";
                break;
            case '5':
                output = ".....";
                break;
            case '6':
                output = "-....";
                break;
            case '7':
                output = "--...";
                break;
            case '8':
                output = "---..";
                break;
            case '9':
                output = "----.";
                break;
            default:
                output = "";
                break;
        }
        return output;
    }
}
