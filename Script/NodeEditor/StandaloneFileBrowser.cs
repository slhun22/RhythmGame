using Ookii.Dialogs;
using System.Windows.Forms;
using UnityEngine;

public class StandaloneFileBrowser : MonoBehaviour {
    private VistaOpenFileDialog m_OpenFileDialog
    = new VistaOpenFileDialog();

    public void OnButtonOpenFile() {
        SetOpenFileDialog();
        string m_filePath = FileOpen(m_OpenFileDialog)[0];
        EditorManager.instance.Load(m_filePath).Forget();
    }

    string[] FileOpen(VistaOpenFileDialog openFileDialog) {
        var result = openFileDialog.ShowDialog();
        var filenames = result == DialogResult.OK ?
            openFileDialog.FileNames :
            new string[0];
        openFileDialog.Dispose();
        return filenames;
    }

    void SetOpenFileDialog() {
        m_OpenFileDialog.Title = "채보 파일 열기";
        m_OpenFileDialog.Filter
            = "채보 파일 |*.vds" +
            "|모든 파일|*.*";
        m_OpenFileDialog.FilterIndex = 1;
        m_OpenFileDialog.Multiselect = false;
    }
}
