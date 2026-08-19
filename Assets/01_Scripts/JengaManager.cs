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

    public void MoveBlockToTop(Block block)
    {
        // SI EL PISO SUPERIOR YA TIENE 3 BLOQUES
        // CREAR UN NUEVO PISO

        if (blocksOnTopFloor >= 3)
        {
            currentTopFloor++;

            blocksOnTopFloor = 0;

            Debug.Log( "NUEVO PISO CREADO: " + currentTopFloor);
        }

        // ASIGNAR EL BLOQUE AL NUEVO PISO

        block.SetFloor(currentTopFloor);

        blocksOnTopFloor++;

        Debug.Log( "BLOQUE " + block.name + " AHORA PERTENECE AL PISO " + currentTopFloor);

        Debug.Log("BLOQUES EN EL PISO SUPERIOR: " + blocksOnTopFloor);
    }
}