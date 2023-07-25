using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class CustomAnimationController : MonoBehaviour
{
    private PlayableGraph m_Graph;
    public AnimationClip[] clipsToPlay;


    private void Start()
    {
        m_Graph = PlayableGraph.Create("CustomAnimationController");
        GraphVisualizerClient.Show(m_Graph);
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var custPlayable = ScriptPlayable<CustomAnimationControllerPlayable>.Create(m_Graph);
        var playQueue = custPlayable.GetBehaviour();
        playQueue.Initialize(clipsToPlay, custPlayable, m_Graph);
        var playableOutput = AnimationPlayableOutput.Create(m_Graph, "Animation", GetComponent<Animator>());
        playableOutput.SetSourcePlayable(custPlayable, 0);

        m_Graph.Play();
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {
        m_Graph.Destroy();
    }


}
