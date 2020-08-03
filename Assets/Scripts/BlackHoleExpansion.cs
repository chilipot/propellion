using UnityEngine;

public class BlackHoleExpansion : MonoBehaviour
{

    public float expansionRate = 10f;
    public float pullDistance = 20f;
    public Transform pullField, consumptionField;

    private Renderer pullFieldRenderer, consumptionFieldRenderer;
    private AudioSource ambientSfx;
    private Vector2 ambientSfxStartDistance;

    private void Start()
    {
        pullFieldRenderer = pullField.gameObject.GetComponent<Renderer>();
        consumptionFieldRenderer = consumptionField.gameObject.GetComponent<Renderer>();
        ambientSfx = GetComponent<AudioSource>();
        ambientSfxStartDistance = new Vector2(ambientSfx.minDistance, ambientSfx.maxDistance);
    }

    private void Update()
    {
        if (!ProceduralGeneration.FinishedGenerating) return;
        var consumptionFieldNewScale = consumptionField.localScale + Time.deltaTime * expansionRate * Vector3.one;
        consumptionField.localScale = consumptionFieldNewScale;
        
        var pullFieldCurrSize = pullFieldRenderer.bounds.size.z;
        var pullFieldNewSize = consumptionFieldRenderer.bounds.size.z + pullDistance;
        
        var pullFieldNewScale = pullFieldNewSize * pullField.localScale / pullFieldCurrSize;
        pullField.localScale = pullFieldNewScale;

        ambientSfx.minDistance = ambientSfxStartDistance[0] * consumptionFieldNewScale.z;
        ambientSfx.maxDistance = ambientSfxStartDistance[1] * pullFieldNewScale.z;
    }

}
