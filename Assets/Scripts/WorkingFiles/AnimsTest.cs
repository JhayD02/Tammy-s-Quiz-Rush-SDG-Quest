using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimatedButton : MonoBehaviour
{
    public Animator animator;
    private Button button;

    // Track which animation should play next
    private bool playDance = true;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
    }

    void OnButtonPressed()
    {
        if (playDance)
        {
            animator.SetTrigger("Move1");
        }
        else
        {
            animator.SetTrigger("Move2");
        }

        // Flip the toggle for next press
        playDance = !playDance;
    }
}
