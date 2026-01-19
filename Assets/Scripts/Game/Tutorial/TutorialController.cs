using System.Collections.Generic;

using UnityEngine;

public static class TutorialController
{
    private static Queue<TutorialPanel> panelQueue = new Queue<TutorialPanel>();
    private static TutorialPanel currentPanel = null;

    /// <summary>
    /// Called by TutorialPanel.Enqueue(). Adds a panel to the queue and opens it if it's the only one.
    /// </summary>
    public static void EnqueuePanel(TutorialPanel panel)
    {
        panelQueue.Enqueue(panel);
        if (currentPanel == null)
        {
            OpenNextPanel();
        }
    }

    /// <summary>
    /// Called by TutorialPanel.Close(). Removes the current panel and opens the next one if available.
    /// </summary>
    public static void OnPanelClosed(TutorialPanel panel)
    {
        if (currentPanel == panel)
        {
            currentPanel = null;
            OpenNextPanel();
        }
    }

    private static void OpenNextPanel()
    {
        if (panelQueue.Count > 0)
        {
            currentPanel = panelQueue.Dequeue();
            currentPanel.DoOpen();
        }
    }
}