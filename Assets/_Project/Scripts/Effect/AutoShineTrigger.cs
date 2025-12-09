using UnityEngine;
using Coffee.UIExtensions;
using System.Collections;   // Lưu ý: thư viện ShinyEffect nằm trong Coffee.UIEffects

[RequireComponent(typeof(ShinyEffectForUGUI))]
public class AutoShineTrigger : MonoBehaviour
{
    [SerializeField] private float delay = 5f;       // Thời gian chờ trước khi chạy Shine
    [SerializeField] private bool loop = true;         // Lặp lại Shine hay không

    private ShinyEffectForUGUI shiny;

    private void Awake()
    {
        shiny = GetComponent<ShinyEffectForUGUI>();
    }
    
    private void OnEnable()
    {
        StartCoroutine(PlayShineRoutine());
    }

    private void OnDisable()
    {
        shiny.location = 0;
    }

    private IEnumerator PlayShineRoutine()
    {
        do
        {
            yield return new WaitForSeconds(delay);

            if (shiny != null)
            {
                shiny.Play();  // GỌI HIỆU ỨNG SHINE
            }

        } while (loop);
    }
}
