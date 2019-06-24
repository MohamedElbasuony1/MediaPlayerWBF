using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WBF_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ListViewItem> lsts = new List<ListViewItem>();
        OpenFileDialog ofd;
        int? selectindex=null;
        bool mediaPlayerIsPlaying = false;
        bool userIsDraggingSlider = false;

        public MainWindow()
        {
            InitializeComponent();
            VolumeControl.Value = 0.5;
            ofd = new OpenFileDialog();
            ofd.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((Media.Source != null) && (Media.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                SeekbarControl.Minimum = 0;
                SeekbarControl.Maximum = Media.NaturalDuration.TimeSpan.TotalSeconds;
                SeekbarControl.Value = Media.Position.TotalSeconds;
            }
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                ListViewItem track = new ListViewItem();
                track.Tag = new FileInfo(ofd.FileName);
                lsts.Add(track);
                track.Content = lsts.Count + "- " + new FileInfo(ofd.FileName).Name;
                LoadList();
                if (selectindex!=null)
                {
                    Playlist.SelectedIndex = (int)selectindex;
                }
            }
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    ListViewItem track = new ListViewItem();
                    track.Tag = new FileInfo(ofd.FileNames[i]);
                    lsts.Add(track);
                    track.Content = lsts.Count + "- " + new FileInfo(ofd.FileNames[i]).Name;
                    LoadList();
                    if (selectindex != null)
                    {
                        Playlist.SelectedIndex = (int)selectindex;
                    }
                }
            }
        }

        void LoadList()
        {
            Playlist.Items.Clear();
            foreach (ListViewItem item in lsts)
            {
                Playlist.Items.Add(item);
            }
        }

        private void skip_previous_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source!=null&&selectindex!=0)
            {
                mediaPlayerIsPlaying = false;
                Media.Source = new Uri(((FileInfo)(lsts[((int)selectindex) - 1]).Tag).FullName);
                selectindex--;
                play_Click(null, null);                       
            }
        }
        private void SeekbarControl_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void SeekbarControl_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            Media.Position = TimeSpan.FromSeconds(SeekbarControl.Value);
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (!mediaPlayerIsPlaying)
            {
                Media.Play();
                Media.Volume = VolumeControl.Value;
                Playlist.SelectedIndex = (int)selectindex;
                PlayImg.Source = new BitmapImage(new Uri(@"G:\WBF-Project\Project\bin\Debug\pause.png"));
                if (Media.HasAudio)
                {
                    this.Icon = new BitmapImage(new Uri(@"G:\WBF-Project\Project\bin\Debug\Audio.png"));
                }
                else if(Media.HasVideo)
                {
                    this.Icon = new BitmapImage(new Uri(@"G:\WBF-Project\Project\bin\Debug\Video.png"));
                }
            }
            else
            {
                Media.Pause();
                PlayImg.Source = new BitmapImage(new Uri(@"G:\WBF-Project\Project\bin\Debug\play.png"));
            }
            mediaPlayerIsPlaying = !mediaPlayerIsPlaying;   
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerIsPlaying==true)
            {
                Media.Stop();
            }
            mediaPlayerIsPlaying = !mediaPlayerIsPlaying;  
        }

        private void skip_next_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source != null && selectindex != lsts.Count-1)
            {
                mediaPlayerIsPlaying = false;
                Media.Source = new Uri(((FileInfo)(lsts[((int)selectindex) + 1]).Tag).FullName);
                selectindex++;
                play_Click(null, null);
            }
        }

        private void shuffle_Click(object sender, RoutedEventArgs e)
        {
            Media.Position = TimeSpan.FromSeconds(0);
        }

        private void SeekbarControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Current_duration.Text = TimeSpan.FromSeconds(SeekbarControl.Value).ToString(@"hh\:mm\:ss");
            if (TimeSpan.FromSeconds(SeekbarControl.Value).TotalSeconds==Media.NaturalDuration.TimeSpan.TotalSeconds)
            {
                if (selectindex!=lsts.Count-1)
                {
                    mediaPlayerIsPlaying = false;
                    Media.Source = new Uri(((FileInfo)(lsts[((int)selectindex)+1]).Tag).FullName);
                    selectindex++;
                    play_Click(null, null);
                }
            }
        }

        private void VolumeControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.Volume=e.NewValue;
        }

        private void Playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mediaPlayerIsPlaying = false;
            Media.Source = new Uri(((FileInfo)((ListViewItem)Playlist.SelectedItem).Tag).FullName);
            selectindex = Playlist.SelectedIndex;         
            play_Click(null, null);
        }

        private void FastForward_Click(object sender, RoutedEventArgs e)
        {
            Media.Position = TimeSpan.FromSeconds(SeekbarControl.Value).Add(new TimeSpan(5000));
        }

    }
}
