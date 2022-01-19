using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomParticleSystem : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform test;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Material material;
    private ComputeBuffer buffer;
    private int kernelId;
    private int particleCount = 1000000;
    private int nbOfParticlesPerThread = 512;
    private int nbOfThreads;

    private bool emitting = false;
    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }

    private Vector3 pos = new Vector3();


    private void StartEmitting()
    {
        emitting = true;        
        nbOfThreads = Mathf.CeilToInt((float)particleCount / nbOfParticlesPerThread);
        Particle[] particleArray = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            
            float x = Random.value * 2 - 1;
            float y = Random.value * 2 - 1;
            float z = Random.value * 2 - 1;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize();
            xyz *= Random.value;
            xyz *= 10.5f;

            xyz = Vector3.zero;
            
            particleArray[i].position.x = xyz.x;
            particleArray[i].position.y = xyz.y;
            particleArray[i].position.z = xyz.z +0;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            // Initial life value
            particleArray[i].life = Random.value * 2.0f + 1.0f;
        }
        buffer = new ComputeBuffer(particleCount, 28);//nombre d'octets requis pour stocker Particle
        buffer.SetData(particleArray);
        kernelId = computeShader.FindKernel("CSParticle");
        computeShader.SetBuffer(kernelId,"particleBuffer", buffer);
        material.SetBuffer("particleBuffer",buffer);
    }
    
    private void OnGUI()
    {
        pos = cam.ScreenToWorldPoint(new Vector3(Event.current.mousePosition.x,Screen.height - Event.current.mousePosition.y, cam.nearClipPlane+10));
    }
    private void OnRenderObject()
    {
        material.SetPass(0);
        //Partie A1
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 4, particleCount);
    }
    void OnDestroy()
    {
        if (buffer != null)
            buffer.Release();
    }
    
    private void Update()
    {

        if (emitting)
        {
            float[] posInArray = { pos.x, pos.y };
            test.position = pos;
            computeShader.SetFloat("deltaTime", Time.deltaTime);
            computeShader.SetFloats("mousePos", posInArray);
            computeShader.Dispatch(kernelId,nbOfThreads,1,1);
        }else if (Input.GetMouseButtonDown(0))
        {
            StartEmitting();
            
        }
        
    }


    
}
