using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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


/// *************************************
/// 17/10/2018 Created by Matan Atias @Provision-ISR 
/// 
/// *************************************
namespace desktestrecord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //TODO fix exception of exiting right after entering the program
        Process proc; 
        
        Thread recordthread1;
        Thread recordthread2;
        int filecounter = 1;
        int expcnt = 0;


        public MainWindow()
        {
            InitializeComponent();
            
            RecordLiveVideo.IsEnabled = false;
            Stop.IsEnabled = false;
       
        }
        private void recordToFile()
        {
            try
            {
                string outputFile = "testdesk" + filecounter + ".mp4";
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                // string arguments = "-i rtsp://admin:123456@141.226.94.68:554/profile3 -vcodec copy -r 2600 -y " + outputFile;
               
                string arguments = "-use_wallclock_as_timestamps 1 -rtsp_transport tcp -i rtsp://desk:123456@84.95.205.17:777/chID=4&streamType=sub&linkType=tcp -c:v copy " + outputFile;
                
                //run the process
                proc = new Process();
                proc.StartInfo.FileName = "ffmpeg.exe";
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = true;


                proc.EnableRaisingEvents = true;
                proc.Start();

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                Task.Delay(3000);

                proc.WaitForExit();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("A handled exception" + expcnt + " just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
                --expcnt;
            }

        }
        private void DemoTriggerEvent_Click(object sender, RoutedEventArgs e)
        {
            DemoTriggerEvent.IsEnabled = false;
            RecordLiveVideo.IsEnabled = true;

            recordthread1 = new Thread(recordToFile);
            recordthread1.Start();
           
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            stopRecordingHelper();
        }
      
        private void RecordLiveVideo_Click(object sender, RoutedEventArgs e)
        {
            RecordLiveVideo.IsEnabled = false;
            stopRecordingHelper();

            proc.Close();
         
            ++filecounter;

            recordthread2 = new Thread(recordToFile);
            recordthread2.Start();
            Stop.IsEnabled = true;
        }

        private void stopRecordingHelper()
        {
            try
            {
                StreamWriter inputWriter;
                inputWriter = proc.StandardInput;
                inputWriter.WriteLine("q");
               
                Stop.IsEnabled = false;
            }
            catch (Exception ex)
            {
                if (DemoTriggerEvent.IsEnabled == true)
                    return;
                MessageBox.Show("A handled exception1 just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Shutdown the application.
            //  if(proc !=null)
            //   stopRecordingHelper();  
            try
            {
             
                stopRecordingHelper();
                proc.Close();
            }
            catch (Exception ex)
            {
                if (DemoTriggerEvent.IsEnabled == true)
                    return;
                MessageBox.Show("A handled exception2 just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
          
            Application.Current.Shutdown();
            // OR You can Also go for below logic
            // Environment.Exit(0);
        }
       

    }
}
