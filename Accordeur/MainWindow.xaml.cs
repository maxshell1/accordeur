using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using libAccordeur;
using NAudio.CoreAudioApi;
using NAudio.Mixer;
using NAudio.Wave;

namespace Accordeur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const float eGrave = 82.4f;
        const float a = 110.0f;
        const float d = 146.8f;
        const float g = 196.0f;
        const float b = 246.9f;
        const float eAigu = 329.5f;

        AccordeurManager manager;
        WaveIn sourceStream;
        WaveInProvider wvProvider;
        DirectSoundOut waveOut;
        public MainWindow()
        {
            InitializeComponent();

            cboDevices.ItemsSource = AccordeurManager.getAllDevices();

       
        }
        private async void btnDemarrer_Click(object sender, RoutedEventArgs e)
        {
            if (cboDevices.SelectedValue != null) {
                await Task.Run(() =>
                {
                    demmarrerAsync();
                });
            }
        }
        public void demmarrerAsync() {
            this.Dispatcher.Invoke((Action)delegate
                {
                    manager = new AccordeurManager(44100, 1, (int)cboDevices.SelectedIndex);
                });
            
            manager.demarrer();
            while (manager.IsInProgress) {
                this.Dispatcher.Invoke((Action)delegate
                {
                    btnStop.IsEnabled = true;
                    lbFreq.Content = manager.frequencyInProgress;
                    lblNote.Content = manager.noteInProgress;
                });
            }
        
        } 
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            manager.Stop();
            btnStop.IsEnabled = false;
        }
    }
}
