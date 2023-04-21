using UnityEngine;

using JTimer = JLXB.Framework.Timer;

public class TestTimer : MonoBehaviour
{
    // Start is called before the first frame update
    JTimer.Timer timer1;
    JTimer.Timer timer2;
    JTimer.Timer timer3;
    JTimer.Timer timer4;

    int count = 5;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     Timer.SetTimeout(1.1f, () => { ShowComplete("0"); });
        // }

        if (Input.GetKeyDown(KeyCode.T))
        {
            // timer1 = JTimer.Timer.Register(1f, () => { ShowComplete("1"); });
            // timer2 = JTimer.Timer.Register(2f, () => { ShowComplete("2"); }, secondsElapsed => { ShowUpdate("2 + " + secondsElapsed); });
            // if (timer3 == null)
            // {
            //     timer3 = JTimer.Timer.Register(1.5f, () => { ShowComplete("3"); }, isLooped: true);
            // }
            // else if (timer3.isPaused)
            // {
            //     timer3.Resume();
            // }
            // else
            // {
            //     timer3.Pause();
            // }

            timer4 = JTimer.Timer.Register(1.5f, () =>
            {
                ShowComplete("4");
                if (--count < 0)
                {
                    ShowComplete("4 DONE");
                    timer4.Cancel();
                }
            }, isLooped: true);
        }
    }


    void ShowComplete(string data)
    {
        Debug.Log("Timer Call Complete " + data);
    }

    void ShowUpdate(string data)
    {
        Debug.Log("Timer Call Update " + data);
    }
}
