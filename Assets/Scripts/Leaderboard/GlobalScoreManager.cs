using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class GlobalScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputScore;
    [SerializeField] private TMP_InputField firstName;
    // [SerializeField] private TMP_InputField lastName;

    public UnityEvent<string, int> submitScoreEvent;

    public void SubmitScore()
    {
        submitScoreEvent.Invoke(firstName.text, int.Parse(inputScore.text));
    }


}
