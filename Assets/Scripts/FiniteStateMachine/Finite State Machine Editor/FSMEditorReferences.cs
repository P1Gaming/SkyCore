using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FSMEditorReferences : MonoBehaviour
{
    [field: SerializeField]
    public GameObject UIToggle { get; private set; }
    [field: SerializeField]
    public GameObject SelectedStateEditorToggle { get; private set; }
    [field: SerializeField]
    public Transform LinesFolder { get; private set; }
    [field: SerializeField]
    public RectTransform Workspace { get; private set; }
    [field: SerializeField]
    public RectTransform NonZoomedAreaLeftForSeeingWorkspace { get; private set; }
    [field: SerializeField]
    public GameObject StateVisualPrefab { get; private set; }
    [field: SerializeField]
    public GameObject TransitionVisualPrefab { get; private set; }
    [field: SerializeField]
    public TMPro.TMP_InputField StateTitleInputField { get; private set; }

    [field: SerializeField]
    public GameObject SelectedTransitionEditorToggle { get; private set; }
    [field: SerializeField]
    public TMPro.TextMeshProUGUI TransitionTitleText { get; private set; }
    [field: SerializeField]
    public Toggle TransitionDisableToggle { get; private set; }
    [field: SerializeField]
    public Toggle TransitionLogFailureReasonToggle { get; private set; }

    [field: SerializeField]
    public TMPro.TMP_InputField TransitionMinTimeIn1stStateInputField { get; private set; }
    [field: SerializeField]
    public TMPro.TMP_Dropdown TransitionMinTimeParameterSelectionDropdown;
    [SerializeField]
    public TMPro.TMP_Dropdown TransitionMinTimeParameterOrValueDropdown;

    [field: SerializeField]
    public GameObject ConditionVisualPrefab { get; private set; }
    [field: SerializeField]
    public Transform ConditionsParent { get; private set; }
    [field: SerializeField]
    public ToggleGroup ConditionsToggleGroupForSelectingOne { get; private set; }
    [field: SerializeField]
    public GridLayoutGroup ConditionsGridLayoutGroup { get; private set; }
    [field: SerializeField]
    public RectTransform ConditionsScrolledContentRect { get; private set; }

    [field: SerializeField]
    public GameObject ParameterVisualPrefab { get; private set; }
    [field: SerializeField]
    public Transform ParametersParent { get; private set; }
    [field: SerializeField]
    public ToggleGroup ParametersToggleGroupForSelectingOne { get; private set; }
    [field: SerializeField]
    public GridLayoutGroup ParametersGridLayoutGroup { get; private set; }
    [field: SerializeField]
    public RectTransform ParametersScrolledContentRect { get; private set; }
    [field: SerializeField]
    public ScrollRect ParametersScrollRect { get; private set; }
    


}
