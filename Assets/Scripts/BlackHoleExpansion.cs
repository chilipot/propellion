using UnityEngine;

public class BlackHoleExpansion : MonoBehaviour
{

    public float expansionRate = 10f;
    public float pullDistance = 20f;
    public Transform pullField, consumptionField;

    private Renderer pullFieldRenderer, consumptionFieldRenderer;

    private void Start()
    {
        pullFieldRenderer = pullField.gameObject.GetComponent<Renderer>();
        consumptionFieldRenderer = consumptionField.gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        consumptionField.localScale += Time.deltaTime * expansionRate * Vector3.one;
        var pullFieldCurrSize = pullFieldRenderer.bounds.size.z;
        var pullFieldNewSize = consumptionFieldRenderer.bounds.size.z + pullDistance;
        pullField.localScale = pullFieldNewSize * pullField.localScale / pullFieldCurrSize;
    }

}
