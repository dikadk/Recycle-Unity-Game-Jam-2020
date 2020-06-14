using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;
using ABXY.AssetLink;
using System.Linq;
public class Bike : MonoBehaviour
{
    [Reorderable,SerializeField]
    private ReorderableBikeComponents bikeComponents = new ReorderableBikeComponents();

    private GameState gameState;

    public System.Action<Texture2D, int, int> onNewBikeComponentNeeded = null;

    private int numMissingParts = 0;
    private int numAddedParts = 0;

    [SerializeField]
    private Texture2D completeTexture = null;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onPartAdded;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onPartRejected;

    public bool complete
    {
        get
        {
            bool isComplete = true;
            foreach(BikeComponent component in bikeComponents)
            {
                if (component.hidden)
                    isComplete = false;
            }
            return isComplete;
        }
    }

    private void Awake()
    {
        gameState = FindObjectOfType<GameState>();
    }

    private void Start()
    {
        

    }

    public void Setup()
    {
        numMissingParts = gameState.numItemsMissingFromBikes;
        for (int index = 0; index < gameState.numItemsMissingFromBikes; index++)
        {
            BikeComponent component = GetRandomNonDisabledComponent();
            component.hidden = true;
            Bin targetBin = GetFreeBin();
            if (targetBin == null)
                targetBin = GetOldestBin();
            targetBin.SpawnObject(component.objectPrefab);
        }
        NotifyNextBikeComponent();
    }

    private BikeComponent GetNextBikeComponent()
    {
        foreach (BikeComponent component in bikeComponents)
            if (component.hidden)
                return component;
        return null;
    }

    private Bin GetFreeBin()
    {
        Bin[] bins = RScene.Bins.GetAll<Bin>();
        Random random = new Random();

        //randomizing
        bins = bins.OrderBy(x => Random.Range(0f, float.MaxValue)).ToArray();
        
        Bin selectedBin = null;
        foreach(Bin bin in bins)
        {
            if (!bin.hasObjectCurrently)
                selectedBin = bin;
        }
        return selectedBin;
    }

    private Bin GetOldestBin()
    {
        Bin[] bins = RScene.Bins.GetAll<Bin>();
        Bin oldestBin = null;
        foreach(Bin bin in bins)
        {
            if (oldestBin == null)
                oldestBin = bin;
            else if (oldestBin.timeOfLastSpawn > bin.timeOfLastSpawn)
                oldestBin = bin;
        }
        return oldestBin;
    }

    private BikeComponent GetRandomNonDisabledComponent()
    {
        if (bikeComponents.Count == 0)
            return null;
        BikeComponent component = bikeComponents[Random.Range(0, bikeComponents.Count)];
        if (component.hidden && bikeComponents.Count >= 2)
            return GetRandomNonDisabledComponent();
        else
            return component;
    }

    [System.Serializable]
    public class BikeComponent
    {
        public GameObject objectToHide;
        public GameObject objectPrefab;
        public Texture2D objectTexture;
        private bool _hidden;
        public bool hidden
        {
            get { return _hidden; }
            set
            {
                objectToHide.SetActive(!value);
                _hidden = value;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BikeComponentPickup pickup = other.GetComponent<BikeComponentPickup>();
        if (pickup != null)
        {
            bool held = pickup.GetComponent<Item>().isHeld;
            pickup.transform.parent = null;
            pickup.GetComponent<Rigidbody>().isKinematic = false;

            // this is ugly
            BikeComponent matchingComponent = new List<BikeComponent>(bikeComponents.ToArray()).Find(x => x.hidden &&
            x.objectPrefab.GetComponent<BikeComponentPickup>().componentType == pickup.componentType);
            if (matchingComponent != null && held)
            {
                matchingComponent.hidden = false;
                Destroy(other.gameObject);
                onPartAdded?.Invoke();
                NotifyNextBikeComponent();
                numAddedParts++;

            }
            else
            {
                onPartRejected?.Invoke();
            }
        }
    }

    private void NotifyNextBikeComponent()
    {
        BikeComponent next = GetNextBikeComponent();
        if (next != null)
        {
            onNewBikeComponentNeeded?.Invoke(next.objectTexture, numAddedParts, numMissingParts);
        }else
            onNewBikeComponentNeeded?.Invoke(completeTexture, numAddedParts, numMissingParts);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    [System.Serializable]
    private class ReorderableBikeComponents : ReorderableArray<BikeComponent> { }
}
