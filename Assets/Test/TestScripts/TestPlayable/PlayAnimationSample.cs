using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayAnimationSample : MonoBehaviour
{
    public AnimationClip clip;
    PlayableGraph playableGraph;

    private void Start()
    {
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
        var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
        playableOutput.SetSourcePlayable(clipPlayable);
        playableGraph.Play();
    }

    private void OnDisable()
    {
        playableGraph.Destroy();
    }
}
