using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public Vfx[] VfxObjects;
    public List<GameObject> playingVfx = new List<GameObject>(100);
    public List<GameObject> notPlayingVfx = new List<GameObject>(100);
    public List<GameObject> activeSeeds = new List<GameObject>(100);
    public List<GameObject> inactiveSeeds = new List<GameObject>(100);

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Vfx p in VfxObjects)
        {
            GameObject vfxObject = Instantiate(p.particle, transform);
            vfxObject.AddComponent<VfxObject>();

            vfxObject.GetComponent<VfxObject>().id = p.name;
            vfxObject.GetComponent<VfxObject>()._particleSystem = vfxObject.GetComponent<ParticleSystem>();

            p.source = vfxObject.GetComponent<VfxObject>()._particleSystem;

            notPlayingVfx.Add(vfxObject);

            vfxObject.GetComponent<VfxObject>().VfxDone += () => playingVfx.Remove(vfxObject);
            vfxObject.GetComponent<VfxObject>().VfxDone += () => notPlayingVfx.Add(vfxObject);
            vfxObject.GetComponent<VfxObject>().VfxDone += () => vfxObject.SetActive(false);
            vfxObject.SetActive(false);
        }
    }

    public bool IsPlaying(string name)
    {
        foreach (var vfx in playingVfx)
        {
            if (vfx.GetComponent<VfxObject>().id == name)
            {
                return true;
            }
        }

        return false;
    }

    public void PlayFx(string name, Vector3 position)
    {
        Vfx v = Array.Find(VfxObjects, vfx => vfx.name == name);
        if (v != null)
        {
            GameObject NewVfxObject = null;
            foreach (var vfxObject in notPlayingVfx)
            {
                if (v.name == vfxObject.GetComponent<VfxObject>().id && !vfxObject.GetComponent<ParticleSystem>().isEmitting)
                {
                    vfxObject.SetActive(true);
                    notPlayingVfx.Remove(vfxObject);
                    playingVfx.Add(vfxObject);
                    NewVfxObject = vfxObject;
                    break;
                }
            }

            if (NewVfxObject == null)
            {
                NewVfxObject = Initialize(v);
            }

            if (v == null)
            {
                Debug.LogError("Vfx :" + name + " doesn't exist");
                return;
            }

            NewVfxObject.transform.position = position;
            v.source.Play();
            StartCoroutine(NewVfxObject.GetComponent<VfxObject>().VfxPlayed());
        }
        else
        {
            Debug.LogError("No vfx by the name '" + name + "' found");
        }
    }
    public void PlayFx(string name, Vector3 position, Quaternion rotation)
    {
        Vfx v = Array.Find(VfxObjects, vfx => vfx.name == name);
        if (v != null)
        {
            GameObject NewVfxObject = null;
            foreach (var vfxObject in notPlayingVfx)
            {
                if (v.name == vfxObject.GetComponent<VfxObject>().id && !vfxObject.GetComponent<ParticleSystem>().isEmitting)
                {
                    vfxObject.SetActive(true);
                    notPlayingVfx.Remove(vfxObject);
                    playingVfx.Add(vfxObject);
                    NewVfxObject = vfxObject;
                    break;
                }
            }

            if (NewVfxObject == null)
            {
                NewVfxObject = Initialize(v);
            }

            if (v == null)
            {
                Debug.LogError("Vfx :" + name + " doesn't exist");
                return;
            }

            NewVfxObject.transform.position = position;
            NewVfxObject.transform.rotation = rotation;
            v.source.Play();
            StartCoroutine(NewVfxObject.GetComponent<VfxObject>().VfxPlayed());
        }
        else
        {
            Debug.LogError("No vfx by the name '" + name + "' found");
        }
    }
    public void PlayFx(string name, Vector3 position, Transform parent, bool followRotation)
    {
        Vfx v = Array.Find(VfxObjects, vfx => vfx.name == name);
        if (v != null)
        {
            GameObject NewVfxObject = null;
            foreach (var vfxObject in notPlayingVfx)
            {
                if (v.name == vfxObject.GetComponent<VfxObject>().id && !vfxObject.GetComponent<ParticleSystem>().isEmitting)
                {
                    vfxObject.SetActive(true);
                    vfxObject.transform.position = position;
                    if (followRotation)
                        vfxObject.transform.rotation = parent.rotation;
                    vfxObject.transform.parent = parent;
                    
                    notPlayingVfx.Remove(vfxObject);
                    playingVfx.Add(vfxObject);
                    NewVfxObject = vfxObject;
                    break;
                }
            }

            if (NewVfxObject == null)
            {
                NewVfxObject = Initialize(v);
            }

            if (v == null)
            {
                Debug.LogError("Vfx :" + name + " doesn't exist");
                return;
            }
            v.source.Play();
            StartCoroutine(NewVfxObject.GetComponent<VfxObject>().VfxPlayed());
        }
        else
        {
            Debug.LogError("No vfx by the name '" + name + "' found");
        }
    }

    public void PlayRandomRange(string[] names, Vector3 position)
    {
        string seedName = "";
        foreach (string n in names)
        {
            seedName += n;
        }

        if (seedName != "")
        {
            GameObject newSeedObject = null;
            foreach (var seedObject in inactiveSeeds)
            {
                if (seedName == seedObject.GetComponent<RandomSeed>().SeedName)
                {
                    seedObject.SetActive(true);
                    inactiveSeeds.Remove(seedObject);
                    activeSeeds.Add(seedObject);
                    newSeedObject = seedObject;
                    break;
                }
            }

            if (newSeedObject == null)
            {
                newSeedObject = new GameObject("randomSeed");
                newSeedObject.AddComponent<RandomSeed>();
                newSeedObject.GetComponent<RandomSeed>().SeedName = seedName;
                activeSeeds.Add(newSeedObject);
            }

            PlayFx(names[newSeedObject.GetComponent<RandomSeed>().SeedValue], position);
            var newNewSeed = UnityEngine.Random.Range(0, names.Length);
            while (newNewSeed == newSeedObject.GetComponent<RandomSeed>().SeedValue)
            {
                newNewSeed = UnityEngine.Random.Range(0, names.Length);
            }
            newSeedObject.GetComponent<RandomSeed>().SeedValue = newNewSeed;
            newSeedObject.SetActive(false);
            activeSeeds.Remove(newSeedObject);
            inactiveSeeds.Add(newSeedObject);
        }
    }

    public void PlayTrail(string name, Transform position)
    {
        Vfx v = Array.Find(VfxObjects, vfx => vfx.name == name);
        if (v != null)
        {
            GameObject NewVfxObject = null;
            foreach (var vfxObject in notPlayingVfx)
            {
                if (v.name == vfxObject.GetComponent<VfxObject>().id && !vfxObject.GetComponent<ParticleSystem>().isEmitting)
                {
                    vfxObject.SetActive(true);
                    vfxObject.transform.parent = position;
                    vfxObject.transform.position = position.position;
                    vfxObject.transform.rotation = position.rotation;
                    notPlayingVfx.Remove(vfxObject);
                    playingVfx.Add(vfxObject);
                    NewVfxObject = vfxObject;
                    break;
                }
            }

            if (NewVfxObject == null)
            {
                NewVfxObject = Initialize(v);
            }

            if (v == null)
            {
                Debug.LogError("Vfx :" + name + " doesn't exist");
                return;
            }

            if (IsPlaying(name))
                v.source.Clear();
                v.source.Play();

            StartCoroutine(NewVfxObject.GetComponent<VfxObject>().VfxPlayed());
        }
        else
        {
            Debug.LogError("No vfx by the name '" + name + "' found");
        }
    }

    public void StopParticle(string name)
    {
        Vfx v = Array.Find(VfxObjects, vfx => vfx.name == name);
        if (v == null)
        {
            Debug.LogError("Vfx :" + name + " doesn't exist");
            return;
        }
        v.source.Stop();
        v.source.gameObject.GetComponent<VfxObject>().VfxDone?.Invoke();
        if (v.source.gameObject.transform.parent != transform)
        {
            v.source.gameObject.transform.parent = transform;
        }
    }

    private GameObject Initialize(Vfx p)
    { 
        GameObject vfxObject = Instantiate(p.particle, transform);
        vfxObject.AddComponent<VfxObject>();

        vfxObject.GetComponent<VfxObject>().id = p.name;
        vfxObject.GetComponent<VfxObject>()._particleSystem = vfxObject.GetComponent<ParticleSystem>();

        p.source = vfxObject.GetComponent<VfxObject>()._particleSystem;

        notPlayingVfx.Add(vfxObject);

        vfxObject.GetComponent<VfxObject>().VfxDone += () => playingVfx.Remove(vfxObject);
        vfxObject.GetComponent<VfxObject>().VfxDone += () => notPlayingVfx.Add(vfxObject);
        vfxObject.GetComponent<VfxObject>().VfxDone += () => vfxObject.SetActive(false);

        return vfxObject;
    }
}
