using UnityEngine;
using UnityEngine.UI;

public class SystemPanelScript : MonoBehaviour {
  private Image _systemImage;
  private Dropdown _pilotsDropdown;
  private Image _currentPilot;
  private Button _currentAction;

  // Start is called before the first frame update
  void Start() {
    _systemImage = transform.Find("SystemImage").GetComponent<Image>();
    _pilotsDropdown = transform.Find("PilotDropdown").GetComponent<Dropdown>();
    _currentPilot = transform.Find("PilotDropdown").GetComponent<Image>();
    _currentAction = transform.Find("ActionButton").GetComponent<Button>();
  }

  public void SetSystemState(SystemState state) {
    if (state is null) {
      gameObject.SetActive(false);
      return;
    }

    gameObject.SetActive(true);
    _systemImage.sprite = Resources.Load<Sprite>($"systems/{state.system.systemImageFilename}");
  }
}
