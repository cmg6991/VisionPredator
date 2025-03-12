using UnityEngine;
using UnityEngine.UI;

public class DemoSceneManager : MonoBehaviour
{
    public bool DisplayFirstScene;

    public KeyCode NextButton, PreviousButton;
    public KeyCode NextButtonAlternative, PreviousButtonAlternative;

    public GameObject NextButtonDisplay, PreviousButtonDisplay;
    public GameObject[] Folders;
    public Image Siluethe;
    public Text Header;

    public float SwitchDelay, SiluetheSpeed = 2f;
    int CurrentlySelectedFolderIndex;

    bool _selectNext, _selectPrevious, _fadeOutSiluethe;

    private void Start()
    {
        PreviousButtonDisplay?.SetActive(false);

        if (DisplayFirstScene)
        {
            Header.text = Folders[CurrentlySelectedFolderIndex].gameObject.name;
            Folders[CurrentlySelectedFolderIndex].SetActive(true);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(NextButton) || Input.GetKeyDown(NextButtonAlternative))
        {
            if (CurrentlySelectedFolderIndex == Folders.Length - 1)
                return;

            if (_selectNext || _selectPrevious || _fadeOutSiluethe)
                return;

            _selectNext = true;
        }
        if (Input.GetKeyDown(PreviousButton) || Input.GetKeyDown(PreviousButtonAlternative))
        {
            if (CurrentlySelectedFolderIndex == 0)
                return;

            if (_selectNext || _selectPrevious || _fadeOutSiluethe)
                return;

            _selectPrevious = true;
        }

        if (_fadeOutSiluethe)
        {
            _fadeOutSiluethe = !SwitchSiluethe(false);
        }

        if (_selectNext)
        {
            if (SwitchSiluethe(true) == false)
                return;

            Folders[CurrentlySelectedFolderIndex].SetActive(false);
            CurrentlySelectedFolderIndex++;
            Folders[CurrentlySelectedFolderIndex].SetActive(true);

            Header.text = Folders[CurrentlySelectedFolderIndex].gameObject.name;

            if (CurrentlySelectedFolderIndex == Folders.Length - 1)
                NextButtonDisplay.SetActive(false);
            else
                NextButtonDisplay.SetActive(true);

            _fadeOutSiluethe = true;
            _selectNext = false;
        }

        if (_selectPrevious)
        {
            if (SwitchSiluethe(true) == false)
                return;

            Folders[CurrentlySelectedFolderIndex].SetActive(false);
            CurrentlySelectedFolderIndex--;
            Folders[CurrentlySelectedFolderIndex].SetActive(true);

            Header.text = Folders[CurrentlySelectedFolderIndex].gameObject.name;

            if (CurrentlySelectedFolderIndex == 0)
                PreviousButtonDisplay.SetActive(false);
            else
                PreviousButtonDisplay.SetActive(true);

            _fadeOutSiluethe = true;
            _selectPrevious = false;
        }
    }

    bool SwitchSiluethe(bool on)
    {
        if (on)
        {
            var color = new Color(Siluethe.color.r, Siluethe.color.g, Siluethe.color.b, Siluethe.color.a + Time.deltaTime * SiluetheSpeed);
            Siluethe.color = color;

            return Siluethe.color.a >= 1;
        }
        else
        {
            var color = new Color(Siluethe.color.r, Siluethe.color.g, Siluethe.color.b, Siluethe.color.a - Time.deltaTime * SiluetheSpeed);
            Siluethe.color = color;

            return Siluethe.color.a <= 0;
        }
    }
}
