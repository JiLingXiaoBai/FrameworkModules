using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class CustomAnimationController : MonoBehaviour
{
    private PlayableGraph m_Graph;
    private AnimationPlayableOutput m_Output;
    private AnimationMixerPlayable m_Mixer;
    public AnimationClip clip0;
    public AnimationClip clip1;
    [Range(0, 1)]
    public float weight = 0;


    private void Start()
    {
        m_Graph = PlayableGraph.Create("CustomAnimationController");
        GraphVisualizerClient.Show(m_Graph);
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        m_Mixer = AnimationMixerPlayable.Create(m_Graph, 2);
        AnimationClipPlayable clipPlayable0 = AnimationClipPlayable.Create(m_Graph, clip0);
        AnimationClipPlayable clipPlayable1 = AnimationClipPlayable.Create(m_Graph, clip1);
        m_Graph.Connect(clipPlayable0, 0, m_Mixer, 0);
        m_Graph.Connect(clipPlayable1, 0, m_Mixer, 1);
        m_Mixer.SetInputWeight(0, 1);
        m_Mixer.SetInputWeight(1, 0);

        m_Output = AnimationPlayableOutput.Create(m_Graph, "Animation", GetComponent<Animator>());
        m_Output.SetSourcePlayable(m_Mixer);
        m_Graph.Play();
    }

    private void Update()
    {
        m_Mixer.SetInputWeight(0, 1 - weight);
        m_Mixer.SetInputWeight(1, weight);
    }

    private void OnDestroy()
    {
        m_Graph.Destroy();
    }


}
