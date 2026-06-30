using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public GameObject[] bars;

    private void Start()
    {
        GameManager.Instance.OnPowerChanged += Refresh;

        Refresh();
    }

    void Refresh()
    {
        int power =
            GameManager.Instance.actionPointsLeft;

        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].SetActive(i < power);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPowerChanged -= Refresh;
        }
    }
}