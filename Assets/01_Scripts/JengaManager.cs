using UnityEngine;
using System.Collections.Generic;

public class JengaManager : MonoBehaviour
{
    public static JengaManager Instance;

    [SerializeField]
    private Transform jenga;

    [SerializeField]
    private int currentTopFloor;

    [SerializeField]
    private int blocksOnTopFloor;

    [SerializeField]
    private float blockWidth = 1f;
    private float baseFloorHeight;
    private float floorSpacing;
    private Dictionary<int, List<Block>> floorBlocks = new Dictionary<int, List<Block>>();
    private bool isGameOver;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip blockPlaceSound;
    [SerializeField]
    private AudioClip towerFallSound;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AssignBlockFloors();
    }

    private void AssignBlockFloors()
    {
        Block[] blocks =
            jenga.GetComponentsInChildren<Block>();

        List<float> floorHeights = new List<float>();

        // OBTENER LAS ALTURAS DE LOS PISOS

        foreach (Block block in blocks)
        {
            float y = block.transform.localPosition.y;

            bool existingFloor = false;

            foreach (float floorHeight in floorHeights)
            {
                if (Mathf.Abs(y - floorHeight) < 0.01f)
                {
                    existingFloor = true;
                    break;
                }
            }

            if (!existingFloor)
            {
                floorHeights.Add(y);
            }
        }

        // Ordenar los pisos de abajo hacia arriba
        floorHeights.Sort();

        baseFloorHeight = floorHeights[0];
        if (floorHeights.Count > 1)
        {
            floorSpacing = floorHeights[1] - floorHeights[0];
        }

        // ASIGNAR PISO A CADA BLOQUE

        foreach (Block block in blocks)
        {
            float y = block.transform.localPosition.y;

            int floor = 0;

            for (int i = 0; i < floorHeights.Count; i++)
            {
                if (Mathf.Abs(
                    y - floorHeights[i]
                ) < 0.01f)
                {
                    floor = i + 1;
                    break;
                }
            }

            block.SetFloor(floor);

            if (!floorBlocks.ContainsKey(floor))
            {
                floorBlocks[floor] = new List<Block>();
            }
            floorBlocks[floor].Add(block);

            Debug.Log("BLOQUE: " + block.name + " | Y: " + y + " | PISO: " + floor);
        }

        // DETERMINAR PISO SUPERIOR

        currentTopFloor = floorHeights.Count;

        // CONTAR BLOQUES DEL PISO SUPERIOR

        blocksOnTopFloor = 0;

        foreach (Block block in blocks)
        {
            if (block.GetFloor() == currentTopFloor)
            {
                blocksOnTopFloor++;
            }
        }

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "PISO SUPERIOR: " +
            currentTopFloor
        );

        Debug.Log(
            "BLOQUES EN PISO SUPERIOR: " +
            blocksOnTopFloor
        );

        Debug.Log(
            "TOTAL DE PISOS: " +
            floorHeights.Count
        );

        Debug.Log(
            "=============================="
        );
    }

    public bool CanRemoveBlock(int blockFloor)
    {
        bool canRemove = blockFloor < currentTopFloor;

        Debug.Log(
            "COMPROBANDO BLOQUE | " +
            "Piso bloque: " +
            blockFloor +
            " | Piso superior: " +
            currentTopFloor +
            " | Puede retirar: " +
            canRemove
        );

        return canRemove;
    }

    public void MoveBlockToTop(Block block, int dragCollisions, float dragDrift)
    {
        // SI EL PISO SUPERIOR YA TIENE 3 BLOQUES
        // CREAR UN NUEVO PISO
        int oldFloor = block.GetFloor();
        int slotIndex = blocksOnTopFloor;

        if (blocksOnTopFloor >= 3)
        {
            currentTopFloor++;

            blocksOnTopFloor = 0;
            slotIndex = 0;

            Debug.Log( "NUEVO PISO CREADO: " + currentTopFloor);
        }

        if (floorBlocks.ContainsKey(oldFloor))
        {
            floorBlocks[oldFloor].Remove(block);
            if (floorBlocks[oldFloor].Count == 0)
            {
                CollapseFrom(oldFloor);
            }
        }

        // ASIGNAR EL BLOQUE AL NUEVO PISO

        block.SetFloor(currentTopFloor);


        block.transform.SetParent(jenga);
        // --- NUEVO: qué tan torpe fue el arrastre (0 = perfecto, 1 = muy torpe) ---
        float sloppiness = Mathf.Clamp01(dragCollisions * 0.15f + dragDrift * 3f);

        Vector3 basePosition = CalculateTopPosition(currentTopFloor, slotIndex);
        Quaternion baseRotation = CalculateTopRotation(currentTopFloor);

        // desvío aleatorio en X/Z, proporcional a la torpeza (hasta ~35% del ancho del bloque)
        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ) * (sloppiness * blockWidth * 0.35f);

        // pequeño giro aleatorio (hasta ~12°), también proporcional a la torpeza
        Quaternion wobble = Quaternion.Euler(0f, Random.Range(-12f, 12f) * sloppiness, 0f);

        block.transform.localPosition = basePosition + randomOffset;
        block.transform.localRotation = baseRotation * wobble;
        if (audioSource != null && blockPlaceSound != null)
        {
            audioSource.PlayOneShot(blockPlaceSound);
        }

        if (!floorBlocks.ContainsKey(currentTopFloor))
        {
            floorBlocks[currentTopFloor] = new List<Block>();
        }
        floorBlocks[currentTopFloor].Add(block);

        blocksOnTopFloor++;

        Debug.Log( "BLOQUE " + block.name + " AHORA PERTENECE AL PISO " + currentTopFloor);

        Debug.Log("BLOQUES EN EL PISO SUPERIOR: " + blocksOnTopFloor);

        Debug.Log(
    "BLOQUE " + block.name +
    " | TORPEZA: " + sloppiness.ToString("F2") +
    " | AHORA PERTENECE AL PISO " + currentTopFloor
);
    }

    private void CollapseFrom(int emptyFloor)
    {
        Debug.Log("PISO " + emptyFloor + " SIN BLOQUES. LA TORRE PIERDE SOPORTE DESDE AHÍ.");

        foreach (KeyValuePair<int, List<Block>> kvp in floorBlocks)
        {
            if (kvp.Key > emptyFloor)
            {
                foreach (Block b in kvp.Value)
                {
                    b.EnablePhysics();
                }
            }
        }

        TowerFell();
    }

    public void TowerFell()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("GAME OVER: LA TORRE SE CAYÓ.");
        if (audioSource != null && towerFallSound != null)
        {
            audioSource.PlayOneShot(towerFallSound);
        }
    }

    private Vector3 CalculateTopPosition(int floor, int slotIndex)
    {
        float y = baseFloorHeight + (floor - 1) * floorSpacing;
        float offset = (slotIndex - 1) * blockWidth;

        bool rotated = (floor % 2 == 0);
        return rotated
            ? new Vector3(offset, y, 0f)
            : new Vector3(0f, y, offset);
    }
    private Quaternion CalculateTopRotation(int floor)
    {
        bool rotated = (floor % 2 == 0);
        return rotated ? Quaternion.Euler(0f,90f,0f): Quaternion.identity;
    }

    public void CollapseTower()
    {
        if (isGameOver) return;

        Debug.Log("EXTRACCIÓN A MEDIAS: LA TORRE PIERDE ESTABILIDAD POR COMPLETO.");

        foreach (KeyValuePair<int, List<Block>> kvp in floorBlocks)
        {
            foreach (Block b in kvp.Value)
            {
                b.EnablePhysics();
            }
        }

        TowerFell();
    }
}