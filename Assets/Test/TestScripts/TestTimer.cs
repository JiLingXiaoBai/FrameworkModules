using UnityEngine;

using JLXB.Framework.Timer;

public class TestTimer : MonoBehaviour
{
    // Start is called before the first frame update
    // Timer timer1;
    // Timer timer2;
    // Timer timer3;
    // Timer timer4;
    Timer timer5;
    Timer timer6;
    Timer timer7;
    // int count = 5;

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
            // timer1 = Timer.Register(1f, () => { ShowComplete("1"); });
            // timer2 = Timer.Register(2f, () => { ShowComplete("2"); }, secondsElapsed => { ShowUpdate("2 + " + secondsElapsed); });
            // if (timer3 == null)
            // {
            //     timer3 = Timer.Register(1.5f, () => { ShowComplete("3"); }, isLooped: true);
            // }
            // else if (timer3.isPaused)
            // {
            //     timer3.Resume();
            // }
            // else
            // {
            //     timer3.Pause();
            // }

            // timer4 = Timer.Register(1.5f, () =>
            // {
            //     ShowComplete("4");
            //     if (--count < 0)
            //     {
            //         ShowComplete("4 DONE");
            //         timer4.Cancel();
            //     }
            // }, isLooped: true);

            timer5 = TimerUtils.TimerOnce(2, () => { ShowComplete("5"); });
            timer6 = TimerUtils.TimerLoop(2, () => { ShowUpdate("6"); });
            timer7 = TimerUtils.TimerLoop(2, () => { ShowUpdate("7"); }, 5, () => { ShowComplete("7"); });
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
