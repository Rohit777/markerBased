using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoMultipleManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] arObjectsToPlace;
    public GameObject spawnObject;
    [SerializeField]
    [Tooltip("Reference Image Library")]
    XRReferenceImageLibrary m_ImageLibrary;

    /// <summary>
    /// Get the <c>XRReferenceImageLibrary</c>
    /// </summary>
    public XRReferenceImageLibrary ImageLibrary
    {
        get => m_ImageLibrary;
        set => m_ImageLibrary = value;
    }

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(0.01f, 0.01f, 0.01f);

    [SerializeField]
    [Tooltip("Image manager on the AR Session Origin")]
    ARTrackedImageManager m_ImageManager;

    /// <summary>
    /// Get the <c>ARTrackedImageManager</c>
    /// </summary>
    public ARTrackedImageManager ImageManager
    {
        get => m_ImageManager;
        set => m_ImageManager = value;
    }

    [SerializeField]
    [Tooltip("Prefab for tracked 1 image")]
    GameObject m_OnePrefab;

    /// <summary>
    /// Get the one prefab
    /// </summary>
    public GameObject onePrefab
    {
        get => m_OnePrefab;
        set => m_OnePrefab = value;
    }

    GameObject m_SpawnedOnePrefab;

    /// <summary>
    /// get the spawned one prefab
    /// </summary>
    public GameObject spawnedOnePrefab
    {
        get => m_SpawnedOnePrefab;
        set => m_SpawnedOnePrefab = value;
    }

    static Guid s_FirstImageGUID;

    void Awake()
    {
        s_FirstImageGUID = m_ImageLibrary[0].guid;
        m_ImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        s_FirstImageGUID = m_ImageLibrary[0].guid;
        m_ImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_ImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.guid == s_FirstImageGUID)
            {
                m_SpawnedOnePrefab = Instantiate(m_OnePrefab, trackedImage.transform.position, trackedImage.transform.rotation);
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                if (trackedImage.referenceImage.guid == s_FirstImageGUID)
                {
                    m_SpawnedOnePrefab.SetActive(true);
                    m_SpawnedOnePrefab.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
                }
            }
            else
            {
                m_SpawnedOnePrefab.SetActive(false);
            }
                
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            if (trackedImage.referenceImage.guid == s_FirstImageGUID)
            {
                Destroy(m_SpawnedOnePrefab);
            }
        }
    }
}
