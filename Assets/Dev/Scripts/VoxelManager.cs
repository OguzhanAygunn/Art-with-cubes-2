using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class VoxelManager : MonoBehaviour
{
    [Header("Inputs")]
    public Texture2D texture;
    public GameObject cubePrefab;
    public Transform planeTransform;
    public MeshRenderer planeMesh;
    public int cubeCount;
    public Material material;

    [Header("Settings")]
    [Space(6)]
    [SerializeField] int minRes;
    [SerializeField] int maxRes;
    [Range(0.1f, 1f)] public float resolution = 1;

    [Space(6)]
    [Header("Don't Touch")]
    [SerializeField] public float minScale = 0.1f;
    [SerializeField] public float maxScale=1f;
    [SerializeField] public float scaleMultiplier = 1f;

    private float scale = 1f;
    private Transform cubeParent;
    private int xRes;
    private int yRes;
    private Vector3 cubeScale;
    private List<Rigidbody> rigidies = new List<Rigidbody>();


    private void OnValidate()
    {
        if (planeMesh && texture)
        {
            if(planeMesh.material.mainTexture != texture)
                planeMesh.material.mainTexture = texture;
        }
    }

    void Start()
    {

        cubeParent = new GameObject("Cube Parent").transform;
        cubeParent.AddComponent<CubeParentController>();
        //Init();

        xRes = (int)Mathf.Lerp(minRes, maxRes, resolution);
        yRes = (int)Mathf.Lerp(minRes, maxRes, resolution);

        scale = Mathf.Lerp(minScale, maxScale, resolution);

        cubeScale = Vector3.one * scale;
        cubeScale *= scaleMultiplier;
        cubeScale.z = 1;
        
        GenerateVoxelsFromTexture();

        planeMesh.enabled = false;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FreeCubes();
        }
    }



    public Vector3 GetWorldPositionFromPixel(int px, int py)
    {
        float u = (float)px / texture.width;
        float v = (float)py / texture.height;

        Vector3 planeSize = planeTransform.localScale * 10f;

        Vector3 localPos = new Vector3(
            (u - 0.5f) * planeSize.x,
            0f,
            (v - 0.5f) * planeSize.z
        );

        return planeTransform.TransformPoint(localPos);
    }

    private void SpawnCube(Vector3 pos,Color color,float scaleDelay = 0)
    {
        MeshRenderer mr = Instantiate(cubePrefab, pos, Quaternion.identity,cubeParent).GetComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        mr.transform.localScale = Vector3.zero;
        mr.material.color = color;

        mr.transform.DOScale(cubeScale, 0.5f).SetEase(Ease.InOutExpo).SetDelay(scaleDelay);
        rigidies.Add(mr.GetComponent<Rigidbody>());
    }

    void GenerateVoxelsFromTexture()
    {
        int texWidth = texture.width;
        int texHeight = texture.height;

        Vector3 planeScale = planeTransform.localScale * 10f;
        float delay = 0.01f;
        for (int y = 0; y < texHeight; y+=xRes)
        {
            for (int x = 0; x < texWidth; x+=yRes)
            {

                Color pixelColor = texture.GetPixel(x, y);
                if (pixelColor.a < 0.1f) continue;

                Vector3 pos = GetWorldPositionFromPixel(x, y);
                SpawnCube(pos, pixelColor, scaleDelay: delay);

                cubeCount++;
            }
            delay += 0.06f;
        }


        cubeParent.rotation = Quaternion.Euler(0f, 0f, 180f);
        cubeParent.transform.position = planeTransform.position * 2;
        
    }

    public void FreeCubes()
    {
        rigidies.ForEach(rigid => {
            rigid.transform.parent = null;
            rigid.constraints = RigidbodyConstraints.None;
            rigid.AddForce(Vector3.one * UnityEngine.Random.Range(-2, 2), ForceMode.VelocityChange);
        });
    }
}
