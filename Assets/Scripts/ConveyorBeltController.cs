using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltController : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;
    


    [SerializeField, HideInInspector]
    private ConveyorBikeList bikeList;

    [SerializeField]
    private Transform bikeMoverRoot = null;

    [SerializeField]
    private OnNewItemRequired onNewItemRequired;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onTrashed;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onTrucked;

    [SerializeField]
    private UnityEngine.Events.UnityEvent OnBikeSpawn;

    private Bike currentBike = null;

    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
        bikeList = FindObjectOfType<ConveyorBikeList>();
        StartCoroutine(RunConveyor());
    }

    private void OnNewItemRequiredCallback(Texture2D texture, int progress, int total)
    {
        onNewItemRequired?.Invoke(texture, progress, total);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBike != null)
        {
            float animationTransition = currentBike.complete ? 1f : 0f;
            animator.SetFloat("Item Movement Blend", animationTransition);

            animator.speed = currentBike.complete?1f: gameState.currentSpeed;
        }else
            animator.speed = gameState.currentSpeed;
    }
    

    private IEnumerator RunConveyor()
    {
        yield return new WaitForSeconds(0.5f);
        Bike prototype = bikeList.GetRandomBike();
        if (prototype == null)
        { // we'll try again with a new proto
            StartCoroutine(RunConveyor());
            yield break;
        }


        GameObject currentBikeGo = GameObject.Instantiate<GameObject>(prototype.gameObject);


        currentBikeGo.transform.parent = bikeMoverRoot;
        currentBikeGo.transform.localPosition = Vector3.zero;
        currentBikeGo.transform.rotation = bikeMoverRoot.transform.rotation;
        currentBike = currentBikeGo.GetComponent<Bike>();
        currentBike.onNewBikeComponentNeeded += OnNewItemRequiredCallback;
        currentBike.Setup();
        animator.speed = gameState.currentSpeed;

        animator.SetTrigger("Trigger New Item");
        yield return new WaitForSeconds(0.5f);
        OnBikeSpawn?.Invoke();

    }

    public void OnTrashed()
    {
        currentBike.onNewBikeComponentNeeded -= OnNewItemRequiredCallback;
        Destroy(currentBike.gameObject);
        StartCoroutine(RunConveyor());
        onTrashed?.Invoke();
    }

    public void OnTrucked()
    {
        currentBike.onNewBikeComponentNeeded -= OnNewItemRequiredCallback;
        Destroy(currentBike.gameObject);
        StartCoroutine(RunConveyor());
        onTrucked?.Invoke();
    }

    [System.Serializable]
    public class OnNewItemRequired : UnityEngine.Events.UnityEvent<Texture2D, int, int>
    {

    }
}
